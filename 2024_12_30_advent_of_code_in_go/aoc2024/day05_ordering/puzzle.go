package day05_ordering

import (
	"aoc/utils"
	"fmt"
	"slices"
)

type Data struct {
	ordering map[string]struct{} // set
	numbers  [][]int
}

type Cmp func(item1, item2 int) int

func loadData(input string) Data {
	lines := utils.ParseLines(input)
	i := 0

	ordering := make(map[string]struct{})
	for line := lines[i]; i < len(lines) && line != ""; i, line = i+1, lines[i] {
		ordering[line] = struct{}{}
	}

	numbers := make([][]int, len(lines)-i)
	for j := 0; i < len(lines); i, j = i+1, j+1 {
		line := lines[i]
		numbers[j] = utils.ParseIntsWithSplit(line, ",")
	}

	return Data{ordering, numbers}
}

func createCmp(ordering map[string]struct{}) Cmp {
	return func(item1, item2 int) int {
		_, ok := ordering[fmt.Sprintf("%d|%d", item1, item2)]
		return utils.If(ok, -1, 1)
	}
}

func Puzzle(input string, middlefinder func(Cmp, []int) int) string {
	data := loadData(input)
	sum := 0
	cmp := createCmp(data.ordering)
	for _, numbers := range data.numbers {
		if middle := middlefinder(cmp, numbers); middle != -1 {
			sum += middle
		}
	}
	return fmt.Sprint(sum)
}

func Puzzle1(input string) string {
	return Puzzle(input, func(cmp Cmp, numbers []int) int {
		if slices.IsSortedFunc(numbers, cmp) {
			return numbers[len(numbers)/2]
		}
		return -1
	})
}

func Puzzle2(input string) string {
	return Puzzle(input, func(cmp Cmp, numbers []int) int {
		if !slices.IsSortedFunc(numbers, cmp) {
			slices.SortFunc(numbers, cmp)
			return numbers[len(numbers)/2]
		}
		return -1
	})
}
