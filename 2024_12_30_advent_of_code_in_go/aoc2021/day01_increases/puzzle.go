package day01_increases

import (
	"fmt"
	"iter"
	"strconv"
	"strings"

	"github.com/marcinnajder/gopowerseq/seq"
	"github.com/marcinnajder/gopowerseq/seqs"
)

func loadData(input string) []int {
	lines := strings.Split(input, "\n")
	numbers := make([]int, len(lines))
	for i, line := range lines {
		if number, err := strconv.Atoi(line); err != nil {
			panic(err)
		} else {
			numbers[i] = number
		}
	}
	return numbers
}

func countIncreases(numbers iter.Seq[int]) int {
	return seq.Pipe3(
		numbers,
		seq.Pairwise[int](),
		seq.ToTuples[int, int](),
		seq.CountFunc(func(p seq.Tuple[int, int]) bool { return p.Item2 > p.Item1 }),
	)
}

func Puzzle1(input string) string {
	data := loadData(input)
	numbers := seq.Of(data...)
	result := countIncreases(numbers)
	return fmt.Sprint(result)
}

func Puzzle2(input string) string {
	data := loadData(input)
	numbers := seq.Pipe2(
		seq.Of(data...),
		seq.Windowed[int](3),
		seq.Map(seqs.Sum[int]))
	result := countIncreases(numbers)
	return fmt.Sprint(result)
}

// ** the simplest implementation without summing up numbers

func Puzzle_(input string, offset int) string {
	numbers := loadData(input)
	lastIndex := len(numbers) - offset
	counter := 0
	for i := 0; i < lastIndex; i++ {
		if numbers[i] < numbers[i+offset] {
			counter++
		}
	}
	return fmt.Sprint(counter)
}

func Puzzle1_(input string) string {
	return Puzzle_(input, 1)
}

func Puzzle2_(input string) string {
	return Puzzle_(input, 3)
}
