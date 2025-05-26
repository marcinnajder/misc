# Programming with sequences in Go - introduction to iterators

## Pull iterators

[go1.23](https://go.dev/doc/go1.23#iterators) (2024-08-13) introduced [iterators](https://go.dev/blog/range-functions) (`iter.Seq[T]` interface) that can be treated as a lazy sequence known from other programming languages like C#, JS, F#, Clojure, Python,... In most programming languages, iterators are represented as two interfaces.

```go
type Iterable[T any] interface {
	Iterator() Iterator[T]
}

type Iterator[T any] interface {
	Next() (value T, hasValue bool)
}
```

Then, usually all standard collections like arrays, lists, and maps, ... implement an `Iterable` interface. Additionally, programming languages provide a special `foreach` loop for iterating over items in a convenient way. In Go, this type of iteration is called "pull iterators", the "consumer code" pulls values from the "producer code" by calling the `Next` function. Interestingly, the default way of representing the sequence in Go is "push iterator". This article explains in detail how both types of iterators work, how to implement them, and convert between them.

Both interfaces `Iterable` and `Iterator` contain only one method so we can simplify the type definition to just a single function.

```go
type SeqPull[T any] func() (next func() (value T, hasValue bool))
```

`SeqPull` type represents the function returning `next` function, the `next` function returns a pair of values `(value T, hasValue bool)`. Let's implement two simple functions `Range` and `RepeatValue` returning `SeqPull[T]`.

```go
func Range(start, count int) SeqPull[int] {
	return func() func() (value int, hasValue bool) {
		end := start + count
		i := start - 1
		next := func() (int, bool) {
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
			return func() (T, bool) {
				return val, true
			}
		}

		i := 0
		return func() (T, bool) {
			if i < count {
				i++
				return val, true
			}
			var zero T
			return zero, false
		}
	}
}
```

The functions above present the power of a sequence concept. The sequence is lazily evaluated on demand, so there is no need to create an Array or List object in memory. To iterate over values like `1,2,3,...,10000`, we just need to execute `Range(1,10000)` and that creates a recipe for generating those values instead of allocating a collection in memory. Sequence can also be infinite, execution of `RepeatValue("hi",-1)` creates an infinite sequence of values `"hi", "hi", ...`.

Now let's implement some standard operators like `Map`, `Filter`. There is nothing special in name "operator", it's just a regular function that takes and returns sequences.

```go
type Func[T, R any] func(T) R

func Filter[T any](s SeqPull[T], f Func[T, bool]) SeqPull[T] {
	return func() func() (value T, hasValue bool) {
		snext := s()
		return func() (T, bool) {
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
		return func() (R, bool) {
			if value, hasValue := snext(); !hasValue {
				var zero R
				return zero, false
			} else {
				return f(value), true
			}
		}
	}
}
```

The final step is to write some simple code using our functions and iterating over the results.

```go
func IsPositive(number int) bool {
	return number >= 0
}
func ToCurrency(number int) string {
	return fmt.Sprintf("%d zł", number)
}

numbers := Range(-5, 10)
positives := Filter(numbers, IsPositive)
positiveCurrencies := Map(positives, ToCurrency)

next := positiveCurrencies()
for value, hasValue := next(); hasValue; value, hasValue = next() {
	fmt.Println(value)
}
```

Go language does not provide any special loop statement for iteration over "pull iterators", that's way the standard `for` loop was used above. Iterating over items of sequence is such a typical piece of code that we can introduce helper function called `Foreach`. The other frequently used scenario is creating a Slice collection from the sequence, and that can be accomplished by using a new `ToSlice` function.

```go
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
```

There is one interesting detail in the implementation of the `ForEach` function. We pass a function as an argument that will be executed for each of the elements, and that function returns `bool`. From the perspective of the `ForEach` caller, it's not so obvious why we need to return anything at all. Especially if we know other technologies like C#, Java, JavaScript, ... where analogous code would look like this `[1,2,3].forEach(x => console.log(x))`. Here, the anonymous function passed into `forEach` does not return anything. The nice feature of regular loops is that we can always break them in the middle of execution by calling `break`. Our `ForEach` preserves the same behaviour, returning `false` from the anonymous function breaks the iteration.

## Push iterators

At this point we understand what exactly "pull iterators" are and how to work with them. But the default representation of sequence in Go is called "push iterator" and it's defined as type `Seq[T]`.

```go
type Seq[V any] func(yield func(V) bool)
```

If we look very closely at that type, we will see almost the same function signature as `ForEach` function. Now let's implement our previous functions `Range` and `RepeatValue`, but this time using `Seq[T]` instead of `SeqPull[T]`.

```go
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
```

The type definition of `Seq[T]` type can look strange at first, but the code of the functions above is natural and simple. For me personally, this implementation is even simpler than the previous ones using "pull iterators". If you are not convinced yet, let's take a look at operators like `Map` and `Filter`.

```go
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
```

We immediately see what is happening. They iterate over all items from input sequence generating items for the output sequence. This code looks very natural because in Go language we can use `for/range` loop for iterating over "push iterators" (but not for "pull iterators"). `Seq[T]` is just a function so we can always iterate over items by calling it manually.

```go
numbers := Range_(-5, 10)
positives := Filter_(numbers, IsPositive)
positiveCurrencies := Map_(positives, ToCurrency)

// iterate just by calling a function
positiveCurrencies(func(value string) bool {
	fmt.Println(value)
	return true
})

// iterate using built-in for/range loop
for value := range positiveCurrencies {
	fmt.Println(value)
}
```

Now, some questions might come to mind: Do we need two kinds of iterators? If so, in which scenarios each of them should be used? Is there any way to provide conversion in both directions between them? To help in answering those questions, let's implement yet another operator called `Zip`. This time it takes two sequences and iterates over them simultaneously, returning a new sequence of pairs.

```go
type Seq2[K, V any] func(yield func(K, V) bool) // built-in type

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
```

## Push to pull

We chose `Seq[T]` instead of `SeqPull[T]` because "push iterators" are the default ones in Go. This is a great example of a scenario where having only "push iterators" would not be enough. In case of `Zip` operator we have to pull the next items from both sequences at the same time, take the first items, then take the second items, and so on. "push iterators" are great if we want to iterate over a single sequence at once but not so useful for many sequences. Fortunately, Go standard library provides a built-in function `iter.Pull(...)` converting "push iterators" into "pull iterators". Once I encountered this function for the first time, I immediately started asking myself how such a function could be implemented. Knowing many other programming languages besides Go, my basic understanding was that converting "pull to push" should be easy, but the opposite direction not necessary. In some sense, I was right, because Go language provides unique features that allow us to implement it in a quite simple way.

```go
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
```

This is not the perfect implementation, it does not handle all corner cases. But, it works fine and shows how we can "synchronize and communicate" two different worlds. On the one hand side "push iterator" is throwing values at us, on the other side, we need to somehow expose it as a pulling function `next`. That function allows the "consumer" to decide when the next value should be generated. It looks like we have to stop and resume execution of `Seq[T]` each time the `next` function is called. To accomplish this effect, two features of Go were used: goroutines and channels.

The first version of Go was released in 2012, but support for iterators was added in 2024. In general Go language is all about imperative programming, and that's fine. But the same time, the ability to implement lazy sequences simply is a very useful extension to the language. Many other programming languages support "generators" feature, it is a function returning an "iterator" object where a special `yield` keyword can be used. Designers of Go accomplished the same goal without changing the syntax of the language, and the implementation of that feature looks extremely simple compared to the competition.
