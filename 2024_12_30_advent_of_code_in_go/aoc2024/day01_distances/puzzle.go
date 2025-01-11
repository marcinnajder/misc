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
	sum := 0.0

	slices.Sort(data.left)
	slices.Sort(data.right)

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
	leftO := occurrences(data.left)
	rightO := occurrences(data.right)
	sum := 0

	for key, leftV := range leftO {
		if rightV, ok := rightO[key]; ok {
			sum += key * leftV * rightV
		}
	}
	return fmt.Sprint(sum)
}

func Puzzle2_(input string) string {
	data := loadData(input)
	cache := make(map[int]int)
	rightO := occurrences(data.right)
	sum := 0

	for _, l := range data.left {
		v, ok := cache[l]

		if !ok {
			if rO, ok := rightO[l]; ok {
				v = l * rO
			}
			cache[l] = v
		}

		sum += v
	}
	return fmt.Sprint(sum)
}
