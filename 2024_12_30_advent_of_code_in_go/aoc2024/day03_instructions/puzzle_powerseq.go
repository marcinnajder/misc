//go:build powerseq

package day03_instructions

import (
	"fmt"
	"iter"
	"math"
	"regexp"
	"strconv"

	"github.com/marcinnajder/gopowerseq/seq"
	"github.com/marcinnajder/gopowerseq/seqs"
)

type OperationType int

const (
	OperationTypeDo OperationType = iota
	OperationTypeDont
	OperationTypeMul
)

type Operation struct {
	Type   OperationType
	Numers []int
}

func loadData(input string) []Operation {
	opsRegex, _ := regexp.Compile(`(don't\(\)|do\(\)|mul\(\d+,\d+\))`)
	numbersRegex, _ := regexp.Compile(`\d+`)
	ops := opsRegex.FindAllString(input, math.MaxInt)
	result := make([]Operation, len(ops))

	for i, op := range ops {
		switch op {
		case "do()":
			result[i].Type = OperationTypeDo
		case "don't()":
			result[i].Type = OperationTypeDont
		default:
			result[i].Type = OperationTypeMul
			numbers := numbersRegex.FindAllString(op, math.MaxInt)
			numer1, _ := strconv.Atoi(numbers[0])
			numer2, _ := strconv.Atoi(numbers[1])
			result[i].Numers = []int{numer1, numer2}
		}
	}

	return result
}

func Puzzle(input string, filterToMuls func([]Operation) iter.Seq[Operation]) string {
	ops := loadData(input)
	sum := seq.Pipe2(
		ops,
		filterToMuls,
		seq.SumFunc(func(op Operation) int { return op.Numers[0] * op.Numers[1] }))
	return fmt.Sprint(sum)
}

func Puzzle1(input string) string {
	return Puzzle(input, func(ops []Operation) iter.Seq[Operation] {
		return seqs.Filter(ops, func(op Operation) bool { return op.Type == OperationTypeMul })
	})
}

func Puzzle2(input string) string {
	return Puzzle(input, func(ops []Operation) iter.Seq[Operation] {
		return func(yield func(Operation) bool) {
			take := true
			for _, op := range ops {
				switch op.Type {
				case OperationTypeDo:
					take = true
				case OperationTypeDont:
					take = false
				case OperationTypeMul:
					if take && !yield(op) {
						return
					}
				}
			}
		}
	})
}
