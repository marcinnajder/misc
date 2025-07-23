package day01_mass

import (
	"aoc/utils"
	"strconv"
)

func loadData(input string) []int {
	return utils.ParseIntsWithFields(input)
}

type FuelCounter func(int) int

func Puzzle(input string, counter FuelCounter) string {
	numbers := loadData(input)
	sum := 0
	for _, n := range numbers {
		val := counter(n)
		//fmt.Println(val)
		sum += val
	}
	return strconv.Itoa(sum)
}

func countOnce(n int) int {
	return n/3 - 2
}

func Puzzle1(input string) string {
	return Puzzle(input, countOnce)
}

func countUpToZero(n int) int {
	val := countOnce(n)
	if val <= 0 {
		return 0
	}
	return val + countUpToZero(val)
}

func Puzzle2(input string) string {
	return Puzzle(input, countUpToZero)
}
