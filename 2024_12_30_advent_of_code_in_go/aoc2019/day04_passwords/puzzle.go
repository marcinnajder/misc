package day04_passwords

import (
	"aoc/utils"
	"strconv"
	"strings"
)

func loadData(input string) (minNums []int, maxNums []int) {
	parts := strings.Split(input, "-")
	return utils.ParseDigits(parts[0]), utils.ParseDigits(parts[1])
}

func numberToPassword(number int) []int {
	numberStr := strconv.Itoa(number)
	password := make([]int, len(numberStr))

	for i, n := range numberStr {
		password[i] = int(n - '0')
	}
	return password
}

// the simples and inefficient implementation scanning all numbers in rage
func puzzle(input string, isValid func([]int) bool) string {
	parts := strings.Split(input, "-")
	minNumsInt, _ := strconv.Atoi(parts[0])
	maxNumsInt, _ := strconv.Atoi(parts[1])

	sum := 0
	for number := minNumsInt; number < maxNumsInt; number++ {
		password := numberToPassword(number)
		if isValid(password) {
			sum++
		}
	}

	return strconv.Itoa(sum)
}

func isValid1(vals []int) bool {
	areAdjacent := false

	for i := 0; i < len(vals)-1; i++ {
		if vals[i+1] < vals[i] { // this check can be skipped in the efficient implementation
			return false
		}
		areAdjacent = areAdjacent || (vals[i] == vals[i+1])
	}

	return areAdjacent
}

func Puzzle1(input string) string {
	return puzzle(input, isValid1)
}

// additional explanation was necessary
// https://www.reddit.com/r/adventofcode/comments/e65jgt/2019_day_4_part_2_am_i_misunderstanding_the_given/
// - if there are more than 2 adjacents items the same, it is not treated as a "adjacents found"
func isValid2(vals []int) bool {
	areAdjacent := false

	for i := 0; i < len(vals)-1; i++ {
		if vals[i+1] < vals[i] { // this check can be skipped in the efficient implementation
			return false
		}

		areAdjacent = areAdjacent || ((vals[i] == vals[i+1]) && // next
			(i+2 >= len(vals) || (vals[i] != vals[i+2])) && // next next
			(i-1 < 0 || (vals[i] != vals[i-1]))) // prev
	}

	return areAdjacent
}

func Puzzle2(input string) string {
	return puzzle(input, isValid2)
}

// ** alternative implementation
// - do not scan all in rage, scan only the password with increasing digits.
// - execution times: 1033 (21.386625ms vs 11.875µs), 670 (19.0315ms vs 14.875µs)
func step(isValidFunc func([]int) bool, mins []int, maxs []int, from int, to int, fromIsMin bool, toIsMax bool, i int, password []int) int {
	sum := 0

	if i == len(password)-1 { // the last index in password
		for v := from; v <= to; v++ {
			password[i] = v

			if isValidFunc(password) {
				sum++
			}
		}
	} else {
		for v := from; v <= to; v++ {
			password[i] = v

			fromIsMin_ := fromIsMin && v == mins[i]
			toIsMax_ := toIsMax && v == maxs[i]
			from_ := utils.If(fromIsMin_, max(v, mins[i+1]), v)
			to_ := utils.If(toIsMax_, maxs[i+1], 9)

			sum += step(isValidFunc, mins, maxs, from_, to_, fromIsMin_, toIsMax_, i+1, password)
		}
	}

	return sum
}

func puzzle_(input string, isValid func([]int) bool) string {
	mins, maxs := loadData(input)
	password := make([]int, len(mins))
	sum := step(isValid, mins, maxs, mins[0], maxs[0], true, true, 0, password)
	return strconv.Itoa(sum)
}

func Puzzle1_(input string) string {
	return puzzle_(input, isValid1)
}

func Puzzle2_(input string) string {
	return puzzle_(input, isValid2)
}
