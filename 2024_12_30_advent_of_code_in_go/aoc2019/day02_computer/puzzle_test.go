package day02_computer

import (
	"aoc/utils"
	"fmt"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestLoadData(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	data := loadData(input)
	fmt.Println(data)
	assert.Equal(t, 12, len(data))
	// assert.Equal(t, []int{12, 14, 1969, 100756}, data)
}

func TestRunProgram(t *testing.T) {
	run := func(numbers []int) []int {
		runProgram(numbers) // array is mutated
		return numbers
	}

	assert.Equal(t, []int{2, 0, 0, 0, 99}, run([]int{1, 0, 0, 0, 99}))
	assert.Equal(t, []int{2, 3, 0, 6, 99}, run([]int{2, 3, 0, 3, 99}))
	assert.Equal(t, []int{2, 4, 4, 5, 99, 9801}, run([]int{2, 4, 4, 5, 99, 0}))
	assert.Equal(t, []int{30, 1, 1, 4, 2, 5, 6, 0, 99}, run([]int{1, 1, 1, 4, 99, 5, 6, 0, 99}))
}

// func TestPuzzle1(t *testing.T) {
// 	input := utils.ReadTextFile("./data__.txt")
// 	r := Puzzle1(input)
// 	fmt.Println(r)
// 	//assert.Equal(t, "34241", r)
// }

// func TestPuzzle2(t *testing.T) {
// 	input := utils.ReadTextFile("./data__.txt")
// 	r := Puzzle2(input)
// 	fmt.Println(r)
// 	//assert.Equal(t, "34241", r)
// }

// func TestPuzzle2(t *testing.T) {
// 	input := utils.ReadTextFile("./data_.txt")
// 	r := Puzzle2(input)
// 	//fmt.Println(r)
// 	assert.Equal(t, "51316", r)
// }
