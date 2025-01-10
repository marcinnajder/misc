package day03_instructions

import (
	"aoc/utils"
	"testing"

	"github.com/stretchr/testify/assert"
)

var inputWithDosAndDonts = "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))"

func TestLoadData2(t *testing.T) {
	input := inputWithDosAndDonts
	data := loadData(input)
	//fmt.Println(data)
	ops := []Operation{
		{OperationTypeMul, []int{2, 4}},
		{Type: OperationTypeDont},
		{OperationTypeMul, []int{5, 5}},
		{OperationTypeMul, []int{11, 8}},
		{Type: OperationTypeDo},
		{OperationTypeMul, []int{8, 5}},
	}
	assert.Equal(t, ops, data)
}

func TestPuzzle1(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle1(input)
	// fmt.Println(r)
	assert.Equal(t, "161", r)
}

func TestPuzzle2(t *testing.T) {
	input := inputWithDosAndDonts
	r := Puzzle2(input)
	// fmt.Println(r)
	assert.Equal(t, "48", r)
}
