# Programming with sequences in Go - introduction to gopowerseq

In [previous article](https://marcinnajder.github.io/) we talked about two kinds of iterators in the Go language: push and pull iterators. Built-in `iter.Seq[T]` interface implements a push iterator and can be used as a lazy sequence known from other programming languages. We have even implemented a few standard operations working with sequences like `Range, Filter, Map`. Let's look at them again and add a new operator called `Take`.

```go
type Func[T, R any] func(T) R

// operators implemented as functions
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
```

There are regular generic functions taking and returning `Seq[T]` interface. We can write a "query" over a sequence of items just by combining those functions. But this does not do much, it just creates many instances of `Seq[T]` linked together in memory. `Seq[T]` type is just an alias to the function type, its body is not executed until the start of iteration caused by `for/range` loop.

```go
func IsEven(n int) bool            { return n%2 == 0 }
func ToCurrency(number int) string { return fmt.Sprintf("%d zł", number) }

numbers := Range(0, 100)
evenNumbers := Filter_(numbers, IsEven)
evenCurrencies := Map_(evenNumbers, ToCurrency)
evenCurrencies5 := Take_(evenCurrencies, 5)

for value := range evenCurrencies5 {
	fmt.Println(value)
}
```

## Why powerseq in Go ?

A few years ago I built a JavaScript library written in TypeScript called [powerseq](https://github.com/marcinnajder/powerseq). It provides around 80 operators working with lazy sequences that are also available in other technologies like [C#, F#, Java, Kotlin, Clojure,...](https://github.com/marcinnajder/powerseq/blob/master/docs/mapping.md). If you are interested in how features like iterators and generators work in JS read [this series](https://marcinnajder.github.io/) of articles.

I started asking myself, what a similar library could look like in the Go language? In this article, we will try to answer this question, but first I want to clarify one thing. Programming in Go is imperative, I get it and even like it. We use loops, ifs, variables, and simple data structures like slices and maps everywhere in idiomatic Go code. Go has first-class support for function data types, anonymous functions with closures, and lazy sequences. These are fundamental building blocks of functional programming. But that said, I am not trying to say or convince anyone that we should write Go code using operators like `Map, Filter, Take, ...` instead of regular loops. Even for myself, using them in most cases just introduces unnecessary complexity to our code. Most Go developers are not familiar with advanced functional concepts. The Go language itself lacks a few crucial features like for example necessity of writing the full list of types for signature of anonymous function. We have to write `Map(people, func (p Person) string { return p.name })` instead of `Map(people, p => p.name)` known from the other programming languages. This leads to very verbose functional code.

Nevertheless, the implementation of gopowerseq library was a really fun side project that helped me answer a few questions:

- how to build, publish, and use a library implemented as a Go module?
- having Go features like generics, first-class functions, `Seq[T]` interface, ... is it possible to implement all powerseq operators and how the API natural for Go developers could look like?
- the library provides many built-in operators but it should be always possible to add custom operators easily, how to accomplish that ?
- let's say I already have some algorithmic code written in Go, I am just curious whether the existence of such a library would make a difference in terms of code readability

## Composability issue

Let's start with the "composability issue" first. We have tree functions `Range, Filter, Map` and we can use them to build a query that will be evaluated using the built-in `for/range` loop at the end. Each of the the operators is just a separate function, we compose the query by passing output of one call into the input to other call. The set of available operators is not closed, we can always implement yet another operator just by adding a new function. This is a really crucial feature of our design.

We can write the code building the query in two different ways. The first one is by using temporary variables like `numbers, positives, positiveCurrencies` above and that's quite inconvenient. The second one is by wrapping operator calls inside each other like `Map(Filter(Range(-5, 10), IsPositive), ToCurrency)`. Our particular the code does not look so terrible because we are not using any anonymous function as arguments, only named functions. In the second approach, the order in which we introduce new operators into the query is unnatural. Let's say we would like to add `Take(..., 5)` at the end of the query, now we have to surround the existing query with a new operator like `Take(Map(Filter, ...) ...) , 5)`.

In programming languages like C#, Java, Kotlin, JS, .. queries are written using "fluent interface API" (aka "method chaining") like `items.Filter(...).Map(...).Take(...)`. This code is very natural to read and write but it can potentially introduce one serious limitation. Let's say that variable `items` is of some type `Iterable[T]` and it defines instance methods like `Filter, Map, Take, ...` returning the same type `Iterable[T]`. In Go we can try to illustrate this approach like this:

```go
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

items := fromSeq(Range(0, 100)).Filter(IsEven).Take(5).ToSeq()
for item := range items {
	fmt.Println(item)
}
```

The problem is that there is no way to introduce a new method without changing the type definition. Java streams suffer from this limitation, but C# and Kotlin don't. Those languages have featured called "extension methods". We can think about them like a regular static method defined outside of `Iterable[T]` type itself but somehow extending it. In C# language `this` keyword is used in a specific way, it can be placed next to the first argument of static method `static IEnumerable<T> Where(this IEnumerable<T> ites, Func<T, bool> func) { ... }`. From that moment any type implementing `IEnumerable<T>` interface contains an additional method called `Where`. For instance, in .NET `string` type implements `IEnumerable<char>` interface so the fallowing piece of code `"extension methods".Where(c => c != ' ')` is valid.

Unfortunately, Go language is similar to Java in regards to objects and methods definition, we can define methods only within the package containing the type itself. There is no way to extend any existing type from the outside. The other problem is that methods can not be generic, only functions can be generic.

```go
// !compilation error! : method must have no type parameters
func (s Iterable[T]) Map[T any](f Func[T, R]) Iterable[R] {
	return fromSeq(Map_(s.ToSeq(), f))
}
```

So even if we introduce a type like `Iterable[T]` wrapping an existing type `iter.Seq[T]` and then define methods like `Filter, Take`, we still won't be able to implement `Map` method. This method maps a sequence of `T` items into sequence of any `R` items. This serious constraint forces us to use functions instead of methods in our design. Functions at least resolve "the composability issue", we can built a library containing a set of functions and any missing operator can be provided as an external functions.

Let's try to improve the way we write queries. As we mentioned, composing a simple query from many operators introduces many temporary variables or requires wrapping many method calls inside each other. We could steal one useful trick called "curried functions" from the functional languages. In some languages like Haskell or "ML family" languages like OCaml or F#, functions always take one argument and return one result. It sounds strange at first, but this simple convention can be easily implemented even in Go.

```go
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

result1 := Add(1, 10)
result2 := AddCurried(1)(10)
increment := AddCurried(1)
fmt.Println(result1, result2, increment(1000)) // 11 11 1001
TakesAndReturnsNothing(UnitV) // "hello"
```

The `Add` function takes two arguments, whereas `AddCurried` function takes only one argument and returns a function type result. Then instead of a typical function execution like `Add(1, 10)`, we have to write two function calls like `AddCurried(1)(10)`. This allows us to create a new functions from existing ones by using "function partial application", it's a calling function without specifying all arguments. In the example above, we were able to create a new function called `increment` just by calling `AddCurried` with only one argument `1`. The `TakesAndReturnsNothing` function shows how to handle a situation where function does not take or return any data. We had to introduce a type `Unit` with one singleton value `UnitV` to represent `void` as a correct Go data type.

But how all of this is related to our initial problem with simplifying queries? The first step is to implement our operators in "curried form". As a reminder, the operator is a function that takes `iter.Seq[T]` and returns other `iter.Seq[T]` or any other type like `int`, `bool`, or `slice`.

```go
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

var evenNumbers iter.Seq[int] = Filter(IsEven)(Range(0, 5))
for item := range evenNumbers {
	fmt.Println(item) // 0 2 4
}
```

Then let's add a new function called `Pipe` that allows us to compose multiple functions written in "curried form".

```go
func Pipe[T, R any](val T, f func(T) R) R {
	return f(val)
}

func Pipe2[T1, T2, T3 any](val T1, f1 func(T1) T2, f2 func(T2) T3) T3 {
	return f2(f1(val))
}

func Pipe3[T1, T2, T3, T4 any](val T1, f1 func(T1) T2, f2 func(T2) T3, f3 func(T3) T4) T4 {
	return f3(f2(f1(val)))
}

items := Pipe3(Range(0, 100), Filter(IsEven), Map(ToCurrency), Take[string](5))
for item := range items {
	fmt.Println(item)
}
```

The `Pipe` function calls the `f` function argument with the `val` value argument and returns the result. Functions `Pipe2, Pipe3, ...` work exactly the same way, the result from the preview call is passed into the next call as an argument. Such a chain of calls is possible only because all functions take and return one single value. The query composes multiple operator calls into one `iter.Seq[T]` object, then `for/range` loop starts the evaluation. Thanks to the lazy evaluation of operators, there is no need to create any slice or array collections for each of the intermediate steps. It's not a big deal, but sometimes Go type inference is not good enough so we have to manually specify the type argument like this `Take[string](5)`.

## Working with slices

The most commonly used types representing collections in Go are slices, arrays, and maps. Unfortunately, our operators work with different type - `iter.Seq[T]`. Go standard library provides helper function for conversion between collections and `iter.Seq[T]` . For slices, function `slices.Collect` creates a slice from the sequence, and `slices.Values` and `slices.All` create a sequence from the slice. `All` function returns `iter.Seq2[int, E]` interface representing pairs of values with indexes of items. Similarly, for maps package `maps` provider functions `Collect, All, Values`. Let's say, we would like to write a code taking and returning a slice using our set of operators. Such a query has to start with `slices.Values` and finish with `slices.Collect` function calls.

```go
sliceOfNumbers := []int{5, 10, 15, 50, 155}
sliceOfEvenNumbers := Pipe2(
	slices.Values(sliceOfNumbers),
	Filter(IsEven),
	slices.Collect[int])
```

Let's be honest, it's annoying. Especially, if we use only one single operator. We can try to simplify those scenarios by adding new versions of operators.

```go
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

func Of[T any](items ...T) iter.Seq[T] {
	return slices.Values(items)
}

func ToSlice[T any]() OperatorR[T, []T] {
	return func(s iter.Seq[T]) []T {
		return slices.Collect(s)
	}
}

var sliceOfEvenNumbers2 []int = Pipe(
	FilterS(sliceOfNumbers, IsEven),
	ToSlice[int]())

for item := range Filter(IsEven)(Of(5, 10, 15, 50, 155)) { // or Of(sliceOfNumbers...)
	fmt.Println(item)
}

var currencies []string = MapS(sliceOfNumbers, ToCurrency)
```

Functions `FilterS, TakeS, ...` call original logic from `Filter, Take, ...` but their signatures are not curried. Additionally, they take slices instead of `Seq[T]`. Those little changes simplify their usage a lot. Other helper function `ToSlice` is curried and wraps the standard function `slices.Collect`. It was introduced as a unification next to similar operators like `ToMap, ToTuples` in gopowerseq library. `Map` function is different, it not only takes slice but also returns a slice instead of `Seq[T]`. It's a small optimization. During the mapping process, we know exactly how many items the input slice contains so we can immediately create an output slice with the appropriate length avoiding intermediate `Seq[T]` object completely. The last function `Of` is useful because it allows us to create a sequence of fixed number of items in a very compact way `Of(5, 10, 15, 50, 155)`, we can also easily convert slice into a sequence calling `Of(sliceOfNumbers...)`.

## gopowerseq library

Now we know how and why the final API of [gopowerseq](https://github.com/marcinnajder/gopowerseq) library looks like. Let's take a look at a few examples from the official documentation:

```go
isEven := func(x int) bool { return x%2 == 0 }

var numbersSlice []int = []int{1, 2, 3, 4, 5}
var numbersIter iter.Seq[int] = seq.Of(1, 2, 3, 4, 5) // slices.Values(numbersSlice)

// 'seq' package - calling single operator taking input of type 'iter.Seq[T]'
var evenNumbers iter.Seq[int] = seq.Filter(isEven)(numbersIter)
for n := range evenNumbers {
	fmt.Println(n) // 2, 4
}

// 'seq' package - chaining many operators
var items []int = seq.Pipe3(
	seq.Range(0, math.MaxInt64),
	seq.Filter(func(x int) bool { return x%2 == 0 }),
	seq.Take[int](5),
	seq.ToSlice[int]())
fmt.Println(items) // [0 2 4 6 8]

// 'seqs' package - calling single operator taking input of type 'slice'
var chars = []string{"a", "b", "c"}
for n, c := range seqs.Zip(numbersSlice, chars) {
	fmt.Println(n, "-", c) // "1 - a", "2 - b", "3 - c"
}
```

Library provides around 50 functions that can grouped into categories:

- creation - Of, Empty, Entries, Range, Repeat, RepeatValue
- filtering - Filter, Skip, SkipWhile, Take, TakeWhile
- mapping - Map, FlatMap
- partitioning - Pairwise, Windowed, PartitionBy, Chunk, Combinations
- merging - Join, Zip, Interleave, Interpose
- grouping - GroupBy, GroupByV, CountBy
- set - Except, Intersect, Union, Distinct, DistinctBy, DistinctUntilChanged
- conversion - ToSlice, ToMap, ToMapV, ToTuples
- aggregation - Reduce, ReduceA, Average, Count, Sum
- quantifiers - All, Any
- concatenation - Concat
- equality - SequenceEqual
- element - First
- misc - Memoize, Share, Expand, Scan

In next article we will see how to use gopowerseq library in some real code.
