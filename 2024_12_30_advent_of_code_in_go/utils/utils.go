package utils

import (
	"os"
	"slices"
)

func ReadTextFile(filepath string) string {
	if text, err := os.ReadFile(filepath); err != nil {
		panic(err)
	} else {
		return string(text)
	}
}

// domyslne dostepne usuwanie modyfikuje tablice wykorzystywana przez slice, ta implemenacja tworzy nowa tablice
func CopyOnRemove[T any](items []T, index int) []T {
	return slices.Concat(items[:index], items[index+1:])
}

func If[T any](cond bool, truevalue T, falsevalue T) T {
	if cond {
		return truevalue
	} else {
		return falsevalue
	}
}

type Tuple2[T1 any, T2 any] struct {
	Item1 T1
	Item2 T2
}

type Tuple3[T1 any, T2 any, T3 any] struct {
	Item1 T1
	Item2 T2
	Item3 T3
}
