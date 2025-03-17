package day18_path

import (
	"aoc/utils"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestLoadData(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	data := loadData(input)
	// fmt.Println(data)
	assert.Equal(t, 25, len(data))
}

func TestPuzzle1(t *testing.T) {
	input := utils.ReadTextFile("./data__.txt")
	r := puzzle1Impl(input, 7, 12)
	//fmt.Println(r)
	assert.Equal(t, "12", r)
}

func TestPuzzle2_(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := puzzle2Impl(input, 7, 12)
	// fmt.Println(r)
	assert.Equal(t, "6,1", r)
}
