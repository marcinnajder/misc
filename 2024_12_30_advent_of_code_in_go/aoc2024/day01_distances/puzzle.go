package day01_distances

import (
	"aoc/utils"
	"fmt"
	"math"
	"slices"
	"strings"
)

type Data struct {
	left  []int
	right []int
}

func loadData(input string) Data {
	lines := strings.Split(input, "\n")
	left := make([]int, len(lines))
	right := make([]int, len(lines))

	for i, line := range lines {
		numbers := utils.ParseIntsWithFields(line)
		left[i] = numbers[0]
		right[i] = numbers[1]
	}

	return Data{left, right}
}

func Puzzle1(input string) string {
	data := loadData(input)

	slices.Sort(data.left)
	slices.Sort(data.right)

	sum := 0.0
	for i := range len(data.left) {
		sum += math.Abs((float64(data.left[i] - data.right[i])))
	}

	return fmt.Sprint(int(sum))
}

func occurrences[T comparable](items []T) map[T]int {
	result := make(map[T]int)
	for _, item := range items {
		result[item] += 1 // nice trick :) map returns default value when there is no key
	}
	return result
}

func Puzzle2(input string) string {
	data := loadData(input)
	leftoccs := occurrences(data.left)
	rightoccs := occurrences(data.right)
	sum := 0

	for key, leftocc := range leftoccs {
		if rightocc, ok := rightoccs[key]; ok {
			sum += key * leftocc * rightocc
		}
	}
	return fmt.Sprint(sum)
}

func Puzzle2_(input string) string {
	data := loadData(input)
	cache := make(map[int]int)
	rightoccs := occurrences(data.right)
	sum := 0

	for _, l := range data.left {
		v, ok := cache[l]

		if !ok {
			if rightocc, ok := rightoccs[l]; ok {
				v = l * rightocc
			}
			cache[l] = v
		}

		sum += v
	}
	return fmt.Sprint(sum)
}
