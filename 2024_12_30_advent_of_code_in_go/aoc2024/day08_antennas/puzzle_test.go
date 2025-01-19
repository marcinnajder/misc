package day08_antennas

import (
	"aoc/utils"
	"slices"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestLoadData(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	data := loadData(input)
	//fmt.Println(data)
	assert.Equal(t, 2, len(data.antennas))
	assert.Equal(t, 4, len(data.antennas['0']))
	assert.Equal(t, 3, len(data.antennas['A']))
	assert.True(t, slices.Contains(data.antennas['0'], Point{8, 1}))
	assert.False(t, slices.Contains(data.antennas['0'], Point{8, 2}))
}

func TestPuzzle1(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle1(input)
	// fmt.Println(r)
	assert.Equal(t, "14", r)
}
func TestPuzzle2(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle2(input)
	// fmt.Println(r)
	assert.Equal(t, "34", r)
}
