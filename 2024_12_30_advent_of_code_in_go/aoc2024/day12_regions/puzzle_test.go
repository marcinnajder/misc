package day12_regions

import (
	"aoc/utils"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestLoadData(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	lines := loadData(input)
	// fmt.Println(lines)
	assert.Equal(t, 10, len(lines))
}

func TestCountJoinedIndexes(t *testing.T) {
	assert.Equal(t, 1, countJoinedIndexes([]int{4}))
	assert.Equal(t, 4, countJoinedIndexes([]int{4, 5, 6, 1, 2, 10, 11, 15}))
}

func TestPuzzle1(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle1(input)
	//fmt.Println(r)
	assert.Equal(t, "1930", r)
}

func TestPuzzle2(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle2(input)
	//fmt.Println(r)
	assert.Equal(t, "1206", r)
}
