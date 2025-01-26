package day11_blinks

import (
	"aoc/utils"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestLoadData(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	data := loadData(input)
	// fmt.Println(data)
	assert.Equal(t, []int{125, 17}, data)
}

func TestTransform2(t *testing.T) {
	assert.Equal(t, []int{1}, transform(0))
	assert.Equal(t, []int{1 * 2024}, transform(1))
	assert.Equal(t, []int{20, 24}, transform(2024))
	assert.Equal(t, []int{20, 0}, transform(2000))
	assert.Equal(t, []int{20001 * 2024}, transform(20001))
	assert.Equal(t, []int{125 * 2024}, transform(125))
	assert.Equal(t, []int{1, 7}, transform(17))
}

func TestPuzzle1(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle1(input)
	// fmt.Println(r)
	assert.Equal(t, "55312", r)
}

func TestPuzzle2(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle2(input)
	//fmt.Println(r)
	assert.Equal(t, "65601038650482", r)
}
