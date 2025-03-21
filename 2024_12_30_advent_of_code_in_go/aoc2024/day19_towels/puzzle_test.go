package day19_towels

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
	assert.Equal(t, 8, len(data.designs))
	assert.Equal(t, 8, len(data.towels))
}

func TestPuzzle1(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle1(input)
	// fmt.Println(r)
	assert.Equal(t, "6", r)
}

func TestPuzzle2(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle2(input)
	//fmt.Println(r)
	assert.Equal(t, "16", r)
}
