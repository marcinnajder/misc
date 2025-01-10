package utils

import (
	"fmt"
	"iter"
	"os"
	"slices"
	"strconv"
	"strings"

	"golang.org/x/exp/constraints"
)

func ReadTextFile(filepath string) string {
	if text, err := os.ReadFile(filepath); err != nil {
		panic(err)
	} else {
		return string(text)
	}
}

func ParseInts(text string) []int {
	fields := strings.Fields(text)
	numbers := make([]int, len(fields))
	for i, field := range fields {
		if number, err := strconv.Atoi(field); err != nil {
			panic(err)
		} else {
			numbers[i] = number
		}
	}
	return numbers
}

func ParseIntsLines(input string) [][]int {
	lines := strings.Split(input, "\n")
	linesNumbers := make([][]int, len(lines))

	for i, line := range lines {
		numbers := ParseInts(line)
		linesNumbers[i] = numbers
	}

	return linesNumbers
}

// domyslne dostepne usuwanie modyfikuje tablice wykorzystywana przez slice, ta implemenacja tworzy nowa tablice
func CopyOnRemove[T any](items []T, index int) []T {
	return slices.Concat(items[:index], items[index+1:])
}

// https://blog.devtrovert.com/p/go-what-is-flags-enum-and-how-to

func HasFlags[T constraints.Integer](value T, flags ...T) bool {
	for _, f := range flags {
		if value&f != f {
			return false
		}
	}
	return true
}

func AddFlags[T constraints.Integer](value T, flags ...T) T {
	for _, f := range flags {
		value = value | f
	}
	return value
}

func RemoveFlags[T constraints.Integer](value T, flags ...T) T {
	for _, f := range flags {
		value = value &^ f
	}
	return value
}

// func AllFlags[T constraints.Integer](maxflag T) T {
// 	var result int
// 	for i := 1; i <= int(maxflag); i = i * 2 {
// 		result = AddFlags(result, i)
// 	}
// 	return T(result)
// }

func AllFlags[T constraints.Integer](maxflag T) T {
	var result int
	for f := range ForeachFlag(maxflag) {
		result = AddFlags(result, int(f))
	}
	return T(result)
}

func ForeachFlag[T constraints.Integer](maxflag T) iter.Seq[T] {
	return func(yield func(f T) bool) {
		for i := 1; i <= int(maxflag); i = i * 2 {
			if !yield(T(i)) {
				return
			}
		}
	}
}

func PrintFlags[T constraints.Integer](flags ...T) {
	for _, f := range flags {
		fmt.Printf("%b\n", f)
	}
}
