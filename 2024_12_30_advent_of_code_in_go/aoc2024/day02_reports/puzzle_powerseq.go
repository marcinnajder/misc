//go:build powerseq

package day02_reports

import (
	"aoc/utils"
	"cmp"
	"fmt"

	"github.com/marcinnajder/gopowerseq/seqs"
)

func loadData(input string) [][]int {
	return utils.ParseLinesOfInts(input)
}

func findBrokenIndex(numbers []int) int {
	if len(numbers) > 1 {
		order := cmp.Compare(numbers[0], numbers[1]) // -1, 0, 1
		prev := numbers[0]
		for i := 1; i < len(numbers); i++ {
			current := numbers[i]
			diff := current - prev
			if diff == 0 || diff < -3 || diff > 3 || cmp.Compare(prev, current) != order { // if unsafe
				return i
			}
			prev = current
		}
	}
	return -1
}

func Puzzle(input string, isSafe func([]int) bool) string {
	data := loadData(input)
	count := seqs.CountFunc(data, isSafe)
	return fmt.Sprint(count)
}

func Puzzle1(input string) string {
	return Puzzle(input, func(numbers []int) bool {
		return findBrokenIndex(numbers) == -1
	})
}

func Puzzle2(input string) string {
	return Puzzle(input, func(numbers []int) bool {
		i := findBrokenIndex(numbers)
		if i == -1 {
			return true
		}

		for j := range min(3, i+1) { // retry 3 or 2 times with trimmed slices
			trimmed := utils.CopyOnRemove(numbers, i-j) // new slice without (pprev) item
			if findBrokenIndex(trimmed) == -1 {
				return true
			}
		}

		return false
	})
}

// najprostrza implementancja gdzie testujemy wszystkie kombinacje kodow po usunieciu kolejnych elementow
func Puzzle2_(input string) (string, [][]int) {
	results := make([][]int, 0)
	data := loadData(input)
	count := 0
	for _, numbers := range data {
		if findBrokenIndex(numbers) == -1 {
			count++
			results = append(results, numbers)
		} else {
			for i := range len(numbers) {
				if findBrokenIndex(utils.CopyOnRemove(numbers, i)) == -1 {
					count++
					results = append(results, numbers)
					break
				}
			}
		}
	}
	return fmt.Sprint(count), results
}
