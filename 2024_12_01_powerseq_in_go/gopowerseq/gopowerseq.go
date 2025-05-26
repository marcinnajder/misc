// go test ./...

package gopowerseq

import (
	"fmt"
	"iter"
	"slices"
)

// operators implemented as functions

type Func[T, R any] func(T) R

func Range(start, count int) iter.Seq[int] {
	return func(yield func(int) bool) {
		end := start + count
		for i := start; i < end; i++ {
			if !yield(i) {
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

func Take_[T any](s iter.Seq[T], count int) iter.Seq[T] {
	return func(yield func(T) bool) {
		i := count
		if i <= 0 {
			return
		}
		for v := range s {
			if !yield(v) {
				return
			}
			i = i - 1
			if i <= 0 {
				return
			}
		}
	}
}

// ** fluent api

type Iterable[T any] iter.Seq[T]

func fromSeq[T any](s iter.Seq[T]) Iterable[T] {
	return Iterable[T](s)
}

func (s Iterable[T]) ToSeq() iter.Seq[T] {
	return iter.Seq[T](s)
}

func (s Iterable[T]) Filter(f Func[T, bool]) Iterable[T] {
	return fromSeq(Filter_(s.ToSeq(), f))
}

func (s Iterable[T]) Take(count int) Iterable[T] {
	return fromSeq(Take_(s.ToSeq(), count))
}

// !compilation error! method must have no type parameters
// func (s Iterable[T]) Map[T any](f Func[T, R]) Iterable[R] {
// 	return fromSeq(Map_(s.ToSeq(), f))
// }

// ** currying

func Add(a, b int) int {
	return a + b
}

func AddCurried(a int) Func[int, int] {
	return func(b int) int {
		return a + b
	}
}

type Unit struct{}

var UnitV Unit = Unit(struct{}{})

func TakesAndReturnsNothing(_ Unit) Unit {
	fmt.Println("hello")
	return UnitV
}

// ** powerseq

// type Func[T, R any] func(T) R
// type Func2[T1, T2, R any] func(T1, T2) R
// type Func3[T1, T2, T3, R any] func(T1, T2, T3) R

// type inference does work correctly when helpers type 'Func' is used instead of typ 'func(T) R'

func Pipe[T, R any](val T, f func(T) R) R {
	return f(val)
}

func Pipe2[T1, T2, T3 any](val T1, f1 func(T1) T2, f2 func(T2) T3) T3 {
	return f2(f1(val))
}

func Pipe3[T1, T2, T3, T4 any](val T1, f1 func(T1) T2, f2 func(T2) T3, f3 func(T3) T4) T4 {
	return f3(f2(f1(val)))
}

func Pipe4[T1, T2, T3, T4, T5 any](val T1, f1 func(T1) T2, f2 func(T2) T3, f3 func(T3) T4, f4 func(T4) T5) T5 {
	return f4(f3(f2(f1(val))))
}

func Pipe5[T1, T2, T3, T4, T5, T6 any](val T1, f1 func(T1) T2, f2 func(T2) T3, f3 func(T3) T4, f4 func(T4) T5, f5 func(T5) T6) T6 {
	return f5(f4(f3(f2(f1(val)))))
}

func Pipe6[T1, T2, T3, T4, T5, T6, T7 any](val T1, f1 func(T1) T2, f2 func(T2) T3, f3 func(T3) T4, f4 func(T4) T5, f5 func(T5) T6, f6 func(T6) T7) T7 {
	return f6(f5(f4(f3(f2(f1(val))))))
}

type OperatorTR[T, R any] func(T) R
type OperatorR[T, R any] OperatorTR[iter.Seq[T], R]
type Operator[T, R any] OperatorTR[iter.Seq[T], iter.Seq[R]]

func Filter[T any](f Func[T, bool]) Operator[T, T] {
	return func(s iter.Seq[T]) iter.Seq[T] {
		return Filter_(s, f)
	}
}

func Map[T, R any](f Func[T, R]) Operator[T, R] {
	return func(s iter.Seq[T]) iter.Seq[R] {
		return Map_(s, f)
	}
}

func Take[T any](count int) Operator[T, T] {
	return func(s iter.Seq[T]) iter.Seq[T] {
		return Take_(s, count)
	}
}

func Of[T any](items ...T) iter.Seq[T] {
	return slices.Values(items)
}

func ToSlice[T any]() OperatorR[T, []T] {
	return func(s iter.Seq[T]) []T {
		return slices.Collect(s)
	}
}

// operators taking slice instead of Seq[T]

func TakeS[T any](s []T, count int) iter.Seq[T] {
	return Take[T](count)(slices.Values(s))
}

func FilterS[T any](s []T, f Func[T, bool]) iter.Seq[T] {
	return Filter(f)(slices.Values(s))
}

func MapS[T, R any](s []T, f Func[T, R]) []R {
	result := make([]R, len(s))
	for i, v := range s {
		result[i] = f(v)
	}
	return result
}
