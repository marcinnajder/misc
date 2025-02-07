package day14_robots

import (
	"aoc/utils"
	"fmt"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestLoadData(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	robots := loadData(input)
	// fmt.Println(robots)
	assert.Equal(t, 12, len(robots))
}

func TestPuzzle1(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := puzzle1(11, 7, input)
	fmt.Println(r)
	assert.Equal(t, "12", r)
}
