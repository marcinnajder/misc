//go:build powerseq

package day11_blinks

import (
	"aoc/utils"
	"fmt"
	"maps"
	"strconv"

	"github.com/marcinnajder/gopowerseq/seq"
	"github.com/marcinnajder/gopowerseq/seqs"
	"github.com/marcinnajder/gopowerseq/sequ"
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

func Puzzle(input string, blinks int) string {
	numbers := loadData(input)

	his := seqs.CountBy(numbers, sequ.Identity)

	for range blinks {
		hisnext := make(map[int]int)
		for n, o := range his {
			for _, nn := range transform(n) {
				hisnext[nn] += o
			}
		}
		his = hisnext
	}

	sum := seq.Sum[int]()(maps.Values(his))

	return fmt.Sprint(sum)
}

func Puzzle1(input string) string {
	return Puzzle(input, 25)
}

func Puzzle2(input string) string {
	return Puzzle(input, 75)
}
