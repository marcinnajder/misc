package day01_distances

import (
	"aoc/utils"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestLoadData(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	data := loadData(string(input))
	assert.Equal(t, []int{3, 4, 2, 1, 3, 3}, data.left)
	assert.Equal(t, []int{4, 3, 5, 3, 9, 3}, data.right)
}

func TestPuzzle1(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	result := Puzzle1(input)
	// fmt.Println(result)
	assert.Equal(t, "11", result)
}

func TestOccurrences(t *testing.T) {
	his := occurrences([]int{1, 5, 1, 10, 5, 5})
	assert.Equal(t, map[int]int{1: 2, 5: 3, 10: 1}, his)
}

func TestPuzzle2(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	result := Puzzle2(input)
	assert.Equal(t, "31", result)
}

func TestPuzzle2_(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	result := Puzzle2_(input)
	assert.Equal(t, "31", result)
}
