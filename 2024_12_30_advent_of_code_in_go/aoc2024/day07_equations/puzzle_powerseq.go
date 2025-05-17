//go:build powerseq

package day07_equations

import (
	"aoc/utils"
	"fmt"
	"strconv"
	"strings"

	"github.com/marcinnajder/gopowerseq/seqs"
	"github.com/marcinnajder/gopowerseq/sequ"
)

type Entry struct {
	result  int
	numbers []int
}

type Op func(int, int) int

func plus(a, b int) int {
	return a + b
}

func mul(a, b int) int {
	return a * b
}

func concat(a, b int) int {
	n, _ := strconv.Atoi(fmt.Sprintf("%d%d", a, b))
	return n
}

var ops2 = []Op{plus, mul}
var ops3 = []Op{plus, mul, concat}

func loadData(input string) []Entry {
	lines := utils.ParseLines(input)
	data := make([]Entry, len(lines))
	for i, line := range lines {
		numbers := utils.ParseIntsWithFields(strings.Replace(line, ":", "", 1))
		data[i] = Entry{numbers[0], numbers[1:]}
	}
	return data
}

func countCombinations(index int, val int, entry Entry, ops []Op, anyOrAll bool) int {
	if index == len(entry.numbers) { // all numbers processed
		return utils.If(val == entry.result, 1, 0)
	}

	number := entry.numbers[index]
	sum := 0
	for _, op := range ops {
		nextval := op(val, number)

		if nextval <= entry.result { // go forward recursively only when value is lower than expected
			count := countCombinations(index+1, nextval, entry, ops, anyOrAll)
			if count > 0 && anyOrAll { // stop when first is found
				return 1
			}
			sum += count
		}
	}
	return sum
}

func Puzzle(input string, ops []Op) string {
	entries := loadData(input)
	sum := seqs.SumFunc(entries, func(entry Entry) int {
		return sequ.If(countCombinations(1, entry.numbers[0], entry, ops, true) > 0, entry.result, 0)
	})
	return fmt.Sprint(sum)
}

func Puzzle1(input string) string {
	return Puzzle(input, ops2)
}

func Puzzle2(input string) string {
	return Puzzle(input, ops3)
}
