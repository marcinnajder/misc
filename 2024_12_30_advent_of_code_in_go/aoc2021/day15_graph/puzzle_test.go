package day15_graph

import (
	"aoc/utils"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestLoadData(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	data := loadData(input)
	// fmt.Println(data)
	assert.Equal(t, 10, data.size)
	// assert.Equal(t, 10, data.sizey)
}

func TestPuzzle1(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle1(input)
	// fmt.Println(r)
	assert.Equal(t, "40", r)
}

func TestPuzzle2(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle2(input)
	// fmt.Println(r)
	assert.Equal(t, "315", r)
}

// func TestExpandBoard(t *testing.T) {
// 	input := utils.ReadTextFile("./data_.txt")
// 	data := loadData(input)
// 	expanded := expandBoard(data)
// 	// fmt.Println(expanded)
// 	assert.Equal(t, 20, expanded.sizex)
// 	assert.Equal(t, 10, expanded.sizey)
// }

// func TestPuzzle3(t *testing.T) {
// 	input := utils.ReadTextFile("./data_.txt")
// 	r := Puzzle2(input)
// 	//fmt.Println(r)
// 	assert.Equal(t, "9021", r)
// }
