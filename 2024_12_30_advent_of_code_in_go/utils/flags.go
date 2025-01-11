package utils

import (
	"fmt"
	"iter"

	"golang.org/x/exp/constraints"
)

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
