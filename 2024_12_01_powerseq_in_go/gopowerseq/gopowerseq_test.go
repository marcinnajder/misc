// go test ./...

package gopowerseq

import (
	"fmt"
	"iter"
	"slices"
	"testing"
)

// ** helpers
func IsEven(n int) bool            { return n%2 == 0 }
func ToCurrency(number int) string { return fmt.Sprintf("%d zł", number) }
func Plus(a, b int) int            { return a + b }
func GetLen(next string) int       { return len(next) }
func Identity[T any](val T) T      { return val }
func IsPositive(number int) bool   { return number >= 0 }

// **

func TestDocPushMapFilter(t *testing.T) {
	numbers := Range(0, 100)
	evenNumbers := Filter_(numbers, IsEven)
	evenCurrencies := Map_(evenNumbers, ToCurrency)
	evenCurrencies5 := Take_(evenCurrencies, 5)

	for value := range evenCurrencies5 {
		fmt.Println(value)
	}
}

func TestDocFluentApi(t *testing.T) {
	items := fromSeq(Range(0, 100)).Filter(IsEven). /*.Map(ToCurrency)*/ Take(5).ToSeq()
	for item := range items {
		fmt.Println(item)
	}
}

func TestDocCurrying(t *testing.T) {
	result1 := Add(1, 10)
	result2 := AddCurried(1)(10)
	increment := AddCurried(1)
	fmt.Println(result1, result2, increment(1000)) // 11 11 1001

	TakesAndReturnsNothing(UnitV) // "hello"
}

func TestDocPowerseq(t *testing.T) {
	var evenNumbers iter.Seq[int] = Filter(IsEven)(Range(0, 5))
	for item := range evenNumbers {
		fmt.Println(item) // 0 2 4
	}

	var evenNumbers2 iter.Seq[int] = Filter(IsEven)(Of(5, 10, 15))
	fmt.Println(evenNumbers2)

	items := Pipe3(Range(0, 100), Filter(IsEven), Map(ToCurrency), Take[string](5))
	for item := range items {
		fmt.Println(item)
	}
}

func TestDocSlices(t *testing.T) {

	var sliceOfNumbers []int = []int{5, 10, 15, 50, 155}

	var sliceOfEvenNumbers []int = Pipe2(
		slices.Values(sliceOfNumbers),
		Filter(IsEven),
		slices.Collect[int])

	var sliceOfEvenNumbers2 []int = Pipe(
		FilterS(sliceOfNumbers, IsEven),
		ToSlice[int]())

	for item := range Filter(IsEven)(Of(5, 10, 15, 50, 155)) { // or Of(sliceOfNumbers...)
		fmt.Println(item)
	}

	var currencies []string = MapS(sliceOfNumbers, ToCurrency)

	fmt.Println(sliceOfEvenNumbers, sliceOfEvenNumbers2, currencies)

	// numbers := []int{5, 10, 15, 50, 150}

	// var currencies []string = MapS(numbers, ToCurrency) // slice instead of Seq
	// fmt.Println(currencies)

	// var evenNumbers iter.Seq[int] = FilterS(numbers, IsEven)
	// fmt.Println(evenNumbers)

	// for _, item := range numbers[0:3] {
	// 	println(item)
	// }

	// for item := range TakeS(numbers, 3) {
	// 	println(item)
	// }

	// var evenNumbers iter.Seq[int] = Filter(IsEven)(Of(5, 10, 15))
	// for item := range evenNumbers {
	// 	fmt.Println(item) // 0 2 4
	// }

	// items := Pipe3(Range(0, 100), Filter(IsEven), Map(ToCurrency), Take[string](5))
	// for item := range items {
	// 	fmt.Println(item)
	// }
}

// func FuncA(str string) (result int) {
// 	defer func() {
// 		if err := recover(); err != nil {
// 			result = -1 // tutaj mozna ustawic wartosc zwracana
// 		}
// 	}()

// 	if len(str) == 0 {
// 		panic(errors.New("empty string"))
// 	}
// 	return len(str)
// }

// add to doc
// creatoes: range, repeat, of, empty, entries
//
// aggregators :
// predicate: all/any
// fitering:
// mapping:
// grouping:
// set operators
// collection operators
// persistence
// chunks: combinations
// Zip vs ZipP
// powerseq lib: api, operators overview , link to doc with table
// aoc
