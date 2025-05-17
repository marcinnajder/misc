package day20_cheating

import (
	"aoc/utils"
	"slices"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestLoadData(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	data := loadData(input)
	// fmt.Println(data)
	assert.Equal(t, 15, data.size)
	assert.Equal(t, Point{1, 3}, data.start)
	assert.Equal(t, Point{5, 7}, data.end)
}

func TestBuildRouteMap(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	data := loadData(input)
	r := buildRouteMap(data)
	// fmt.Println(r)
	_ = r
	// assert.Equal(t, 15, data.size)
	// assert.Equal(t, Point{1, 3}, data.start)
	// assert.Equal(t, Point{5, 7}, data.end)
}

func TestPuzzle1(t *testing.T) {
	input := utils.ReadTextFile("./data__.txt")
	r := Puzzle1(input)
	// fmt.Println(r)
	_ = r
	//assert.Equal(t, "6", r)
}

// func TestPuzzle2(t *testing.T) {
// 	input := utils.ReadTextFile("./data_.txt")
// 	r := Puzzle2(input)
// 	//fmt.Println(r)
// 	assert.Equal(t, "16", r)
// }

func TestStack(t *testing.T) {
	stack := Stack[int]{}
	stack = stack.Push(5)
	stack = stack.Push(10)
	stack = stack.Push(15)

	assert.Equal(t, Stack[int]{5, 10, 15}, stack)

	stack, val, ok := stack.Pop()
	assert.True(t, ok)
	assert.Equal(t, 15, val)
	assert.Equal(t, Stack[int]{5, 10}, stack)

	stack = stack.Push(150)
	assert.Equal(t, Stack[int]{5, 10, 150}, stack)

	stack, _, _ = stack.Pop()
	stack, _, _ = stack.Pop()
	stack, _, _ = stack.Pop()
	assert.Equal(t, Stack[int]{}, stack)
}

func Test(t *testing.T) {
	data := Data{size: 10}

	assert.Equal(t, []Point{{0, 1}, {1, 0}}, slices.Collect(getNeighbours(data, Point{0, 0})))
	assert.Equal(t, []Point{{1, 1}, {2, 0}, {0, 0}}, slices.Collect(getNeighbours(data, Point{1, 0})))
	assert.Equal(t, []Point{{0, 2}, {0, 0}, {1, 1}}, slices.Collect(getNeighbours(data, Point{0, 1})))
	assert.Equal(t, []Point{{1, 2}, {1, 0}, {2, 1}, {0, 1}}, slices.Collect(getNeighbours(data, Point{1, 1})))

	assert.Equal(t, []Point{{9, 8}, {8, 9}}, slices.Collect(getNeighbours(data, Point{9, 9})))
	assert.Equal(t, []Point{{9, 9}, {9, 7}, {8, 8}}, slices.Collect(getNeighbours(data, Point{9, 8})))
}
