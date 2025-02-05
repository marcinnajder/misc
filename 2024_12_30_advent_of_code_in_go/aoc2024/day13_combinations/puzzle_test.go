package day13_combinations

import (
	"aoc/utils"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestLoadData(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	games := loadData(input)
	// fmt.Println(games)
	assert.Equal(t, 4, len(games))
}

func TestPuzzle1_(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle1_(input)
	// fmt.Println(r)
	assert.Equal(t, "480", r)
}

func TestPuzzle1(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle1(input)
	// fmt.Println(r)
	assert.Equal(t, "480", r)
}
