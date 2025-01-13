package day06_routes

import (
	"aoc/utils"
	"fmt"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestLoadData(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	data := loadData(input)
	assert.Equal(t, 10, len(data.lines))
	assert.Equal(t, Point{4, 6}, data.start)
}

func TestPuzzle1(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle1(input)
	//fmt.Println(r)
	assert.Equal(t, "41", r)
}

func TestWalk(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	data := loadData(input)

	outOrCycle1, points := walk(data, Point{-1, -1}) // no obstructions
	assert.True(t, outOrCycle1)                      // out of bounds
	assert.Equal(t, 41, len(points))

	outOrCycle2, _ := walk(data, Point{3, 6}) // obstruction
	assert.False(t, outOrCycle2)              // cycle

	outOrCycle3, _ := walk(data, Point{6, 7}) // obstruction
	assert.False(t, outOrCycle3)              // cycle
}

func TestPuzzle2(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle2(input)
	fmt.Println(r)
	assert.Equal(t, "41", r)
}
