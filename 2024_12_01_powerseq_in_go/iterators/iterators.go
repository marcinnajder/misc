// go test ./...

package iterators

import (
	"iter"
)

type Iterable[T any] interface {
	getIterator() Iterator[T]
}

type Iterator[T any] interface {
	next() (value T, hasValue bool)
}

// **** PULL

type SeqPull[T any] func() (next func() (value T, hasValue bool))

func Range(start, count int) SeqPull[int] {
	return func() func() (value int, hasValue bool) {
		end := start + count
		i := start - 1
		next := func() (value int, hasValue bool) {
			i++
			if i >= end {
				return 0, false
			}
			return i, true
		}
		return next
	}
}

func RepeatValue[T any](val T, count int) SeqPull[T] {
	return func() func() (value T, hasValue bool) {
		if count < 0 { // infinite
			return func() (value T, hasValue bool) {
				return val, true
			}
		}

		i := 0
		return func() (value T, hasValue bool) {
			if i < count {
				i++
				return val, true
			}
			var zero T
			return zero, false
		}
	}
}

type Func[T, R any] func(T) R

func Filter[T any](s SeqPull[T], f Func[T, bool]) SeqPull[T] {
	return func() func() (value T, hasValue bool) {
		snext := s()
		return func() (value T, hasValue bool) {
			for {
				if value, hasValue := snext(); !hasValue {
					var zero T
					return zero, false
				} else if f(value) {
					return value, true
				}
			}
		}
	}
}

func Map[T, R any](s SeqPull[T], f Func[T, R]) SeqPull[R] {
	return func() func() (value R, hasValue bool) {
		snext := s()
		return func() (value R, hasValue bool) {
			for {
				if value, hasValue := snext(); !hasValue {
					var zero R
					return zero, false
				} else {
					return f(value), true
				}
			}
		}
	}
}

func ForEach[T any](s SeqPull[T], f Func[T, bool]) {
	next := s()
	for value, hasValue := next(); hasValue; value, hasValue = next() {
		if !f(value) {
			return
		}
	}
}

func ToSlice[T any](s SeqPull[T]) []T {
	items := make([]T, 0)
	ForEach(s, func(value T) bool {
		items = append(items, value)
		return true
	})
	return items
}

// **** PUSH

func Range_(start, count int) iter.Seq[int] {
	return func(yield func(int) bool) {
		end := start + count
		for i := start; i < end; i++ {
			if !yield(i) {
				return
			}
		}
	}
}

func RepeatValue_[T any](value T, count int) iter.Seq[T] {
	if count < 0 { // infinite
		return func(yield func(T) bool) {
			for {
				if !yield(value) {
					return
				}
			}
		}
	}
	return func(yield func(T) bool) {
		for i := 0; i < count; i++ {
			if !yield(value) {
				return
			}
		}
	}
}

func Filter_[T any](s iter.Seq[T], f Func[T, bool]) iter.Seq[T] {
	return func(yield func(T) bool) {
		for v := range s {
			if f(v) {
				if !yield(v) {
					return
				}
			}
		}
	}
}

func Map_[T, R any](s iter.Seq[T], f Func[T, R]) iter.Seq[R] {
	return func(yield func(R) bool) {
		for v := range s {
			if !yield(f(v)) {
				return
			}
		}
	}
}

func PullToPush[T any](s SeqPull[T]) iter.Seq[T] {
	return func(yield func(T) bool) {
		ForEach(s, yield)
	}
}

func PushToPull[T any](s iter.Seq[T]) (next func() (value T, hasValue bool), stop func()) {
	var isRunning = false
	var isDone = false
	var values chan T
	var moveNext chan struct{}
	var zero T

	stop = func() {
		if isRunning && !isDone {
			isDone = true
			close(values)
			close(moveNext)
		}
	}

	next = func() (value T, hasValue bool) {
		if isDone {
			return zero, false
		}

		if !isRunning {
			isRunning = true
			values = make(chan T)
			moveNext = make(chan struct{})

			go func() {
				defer stop()

				<-moveNext
				s(func(value T) bool {
					values <- value
					_, ok := <-moveNext
					return ok || !isDone
				})
			}()
		}

		moveNext <- struct{}{}
		value, hasValue = <-values
		return value, hasValue
	}

	return next, stop
}

func Zip[T1, T2 any](s1 iter.Seq[T1], s2 iter.Seq[T2]) iter.Seq2[T1, T2] {
	return func(yield func(T1, T2) bool) {
		next1, stop1 := iter.Pull(s1)
		next2, stop2 := iter.Pull(s2)

		defer stop1()
		defer stop2()

		for {
			val1, ok1 := next1()
			if !ok1 {
				return
			}

			val2, ok2 := next2()
			if !ok2 {
				return
			}

			if !yield(val1, val2) {
				return
			}
		}
	}
}
