package day02_reports

import (
	"aoc/utils"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestLoadData(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	data := loadData(string(input))
	assert.Equal(t, []int{7, 6, 4, 2, 1}, data[0])
	assert.Equal(t, []int{1, 2, 7, 8, 9}, data[1])
}

func TestIsSafeStrict_(t *testing.T) {
	assert.Equal(t, -1, findBrokenIndex([]int{}))
	assert.Equal(t, -1, findBrokenIndex([]int{1}))
	assert.Equal(t, -1, findBrokenIndex([]int{1, 2}))
	assert.Equal(t, -1, findBrokenIndex([]int{1, 2, 3}))
	assert.Equal(t, -1, findBrokenIndex([]int{1, 2, 3, 5}))       // +2
	assert.Equal(t, -1, findBrokenIndex([]int{1, 2, 3, 5, 8}))    // + 3
	assert.Equal(t, 5, findBrokenIndex([]int{1, 2, 3, 5, 8, 12})) // +4

	assert.Equal(t, -1, findBrokenIndex([]int{-1, -2, -3}))
	assert.Equal(t, -1, findBrokenIndex([]int{-1, -2, -3, -5}))     // -2
	assert.Equal(t, 4, findBrokenIndex([]int{-1, -2, -3, -5, -10})) // -5

	assert.Equal(t, 2, findBrokenIndex([]int{1, 2, 2})) // +0
	assert.Equal(t, 2, findBrokenIndex([]int{1, 2, 1})) // +1, -1
}

func TestRemovingPrePreItem(t *testing.T) {
	numbers := []int{62, 61, 62, 63, 65, 67, 68, 71}

	assert.Equal(t, 2, findBrokenIndex(numbers))

	assert.Equal(t, 2, findBrokenIndex(utils.CopyOnRemove(numbers, 2)))
	assert.Equal(t, 1, findBrokenIndex(utils.CopyOnRemove(numbers, 2-1)))
	assert.Equal(t, -1, findBrokenIndex(utils.CopyOnRemove(numbers, 2-2)))

}

func TestPuzzle1(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	result := Puzzle1(input)
	// fmt.Println(result)
	assert.Equal(t, "2", result)
}

func TestPuzzle2(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	result := Puzzle2(input)
	// fmt.Println(result)
	assert.Equal(t, "4", result)
}

func TestPuzzle2_(t *testing.T) {
	input := utils.ReadTextFile("./data__.txt")
	result1, result2 := Puzzle2_(input)
	// fmt.Println(result1)
	assert.Equal(t, "386", result1)
	_ = result2
}

// func TestIsSafeStrict(t *testing.T) {
// 	assert.True(t, isSafeStrict([]int{}))
// 	assert.True(t, isSafeStrict([]int{1}))
// 	assert.True(t, isSafeStrict([]int{1, 2}))
// 	assert.True(t, isSafeStrict([]int{1, 2, 3}))
// 	assert.True(t, isSafeStrict([]int{1, 2, 3, 5}))         // +2
// 	assert.True(t, isSafeStrict([]int{1, 2, 3, 5, 8}))      // + 3
// 	assert.False(t, isSafeStrict([]int{1, 2, 3, 5, 8, 12})) // +4

// 	assert.True(t, isSafeStrict([]int{-1, -2, -3}))
// 	assert.True(t, isSafeStrict([]int{-1, -2, -3, -5}))       // -2
// 	assert.False(t, isSafeStrict([]int{-1, -2, -3, -5, -10})) // -5

// 	assert.False(t, isSafeStrict([]int{1, 2, 2})) // +0
// 	assert.False(t, isSafeStrict([]int{1, 2, 1})) // +1, -1
// }
