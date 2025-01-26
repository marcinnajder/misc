package day11_blinks

import (
	"aoc/utils"
	"fmt"
	"strconv"
)

func loadData(input string) []int {
	return utils.ParseIntsWithFields(input)
}

func transform(val int) []int {
	if val == 0 {
		return []int{1}
	}

	text := strconv.Itoa(val)
	if len(text)%2 == 0 {
		middle := len(text) / 2
		v1, _ := strconv.Atoi(text[0:middle])
		v2, _ := strconv.Atoi(text[middle:])
		return []int{v1, v2}
	}

	return []int{val * 2024}
}

func Puzzle(input string, count int) string {
	numbers := loadData(input)

	his := make(map[int]int)
	for _, n := range numbers {
		his[n]++
	}

	for range count {
		nexthis := make(map[int]int)
		for n, o := range his {
			for _, oo := range transform(n) {
				nexthis[oo] += o
			}
		}
		his = nexthis
	}

	sum := 0
	for _, v := range his {
		sum += v
	}
	return fmt.Sprint(sum)
}

func Puzzle1(input string) string {
	return Puzzle(input, 25)
}

func Puzzle2(input string) string {
	return Puzzle(input, 75)
}
