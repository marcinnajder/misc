package day02_computer

import (
	"aoc/utils"
	"slices"
	"strconv"
)

func loadData(input string) []int {
	return utils.ParseIntsWithSplit(input, ",")
}

func runProgram(numbers []int) {
	for i := 0; i < len(numbers); i += 4 {
		switch numbers[i] {
		case 1:
			numbers[numbers[i+3]] = numbers[numbers[i+1]] + numbers[numbers[i+2]]
		case 2:
			numbers[numbers[i+3]] = numbers[numbers[i+1]] * numbers[numbers[i+2]]
		case 99:
			return
		}
	}
}

func runProgramWithNounAndVerb(numbers []int, noun, verb int) {
	numbers[1] = noun
	numbers[2] = verb
	runProgram(numbers)
}

func Puzzle1(input string) string {
	data := loadData(input)
	runProgramWithNounAndVerb(data, 12, 2)
	return strconv.Itoa(data[0])
}

func Puzzle2(input string) string {
	data := loadData(input)
	result := -1

Outer:
	for n := 0; n < 100; n++ {
		for v := 0; v < 100; v++ {
			numbers := slices.Clone(data)
			runProgramWithNounAndVerb(numbers, n, v)
			if numbers[0] == 19690720 {
				result = 100*n + v
				break Outer
			}
		}
	}

	return strconv.Itoa(result)
}
