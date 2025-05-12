// go test ./...

package iterators

import (
	"fmt"
	"slices"
	"testing"

	// iterators "powerseq_in_go/iterators"

	"github.com/stretchr/testify/assert"
)

func IsPositive(number int) bool {
	return number >= 0
}

func ToCurrency(number int) string {
	return fmt.Sprintf("%d zł", number)
}

func TestRange(t *testing.T) {
	assert.Equal(t, []int{3, 4, 5, 6, 7, 8, 9, 10, 11, 12}, ToSlice(Range(3, 10)))
	assert.Equal(t, []int{}, ToSlice(Range(3, 0)))
}

func TestRepeatValuePull(t *testing.T) {
	assert.Equal(t, []string{"txt", "txt", "txt"}, ToSlice(RepeatValue("txt", 3)))
}

func TestFilter(t *testing.T) {
	numbers := Range(-5, 10)
	assert.Equal(t, []int{0, 1, 2, 3, 4}, ToSlice(Filter(numbers, IsPositive)))
}

func TestMap(t *testing.T) {
	numbers := Range(0, 2)
	assert.Equal(t, []string{"0 zł", "1 zł"}, ToSlice(Map(numbers, ToCurrency)))
}

func TestDocPullMapFilter(t *testing.T) {
	numbers := Range(-5, 10)
	positives := Filter(numbers, IsPositive)
	positiveCurrencies := Map(positives, ToCurrency)

	next := positiveCurrencies()

	// foreach()
	for value, hasValue := next(); hasValue; value, hasValue = next() {
		fmt.Println(value)
	}

	ForEach(positiveCurrencies, func(value string) bool {
		fmt.Println(value)
		return true
	})
}

func TestDocPushMapFilter(t *testing.T) {
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
}

func TestPullToPush(t *testing.T) {
	numbersPull := Range(0, 2)
	numbersPush := PullToPush(numbersPull)
	assert.Equal(t, []int{0, 1}, slices.Collect(numbersPush))
}

func TestPushToPull(t *testing.T) {

	numbersPush := Range_(0, 10)

	next, stop := PushToPull(numbersPush)

	value, hasValue := next()
	assert.Equal(t, true, hasValue)
	assert.Equal(t, 0, value)

	value, hasValue = next()
	assert.Equal(t, true, hasValue)
	assert.Equal(t, 1, value)

	stop()
	stop()

	value, hasValue = next()
	assert.Equal(t, false, hasValue)
	assert.Equal(t, 0, value)

}
