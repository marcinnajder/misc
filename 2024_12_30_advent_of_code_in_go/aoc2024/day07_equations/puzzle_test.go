package day07_equations

import (
	"aoc/utils"
	"fmt"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestLoadData(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	entries := loadData(input)
	// fmt.Println(entries)
	assert.Equal(t, 9, len(entries))
	assert.Equal(t, 190, entries[0].result)
	assert.Equal(t, []int{10, 19}, entries[0].numbers)
}

func TestCountCombinations(t *testing.T) {

	input := utils.ReadTextFile("./data_.txt")
	entries := loadData(input)

	tests := []struct {
		ops                     []Op
		indexesOfCorrectEntries map[int]int
	}{
		{ops2, map[int]int{0: 1, 1: 1, 8: 1}},
		{ops3, map[int]int{0: 1, 1: 1, 8: 1 /**/, 3: 1, 4: 1, 6: 1}},
	}

	for _, tt := range tests {
		for i, entry := range entries {
			testname := fmt.Sprintf("ops:%d entry:%v ", len(tt.ops), entry)

			t.Run(testname, func(t *testing.T) {
				assert.Equal(t, tt.indexesOfCorrectEntries[i], countCombinations(1, entry.numbers[0], entry, tt.ops, true)) // anyOrAll=FALSE
			})
		}
	}

	// 2 combinations found
	assert.Equal(t, 2, countCombinations(1, entries[1].numbers[0], entries[1], ops2, false)) // anyOrAll=TRUE
}

func TestPuzzle1(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle1(input)
	// fmt.Println(r)
	assert.Equal(t, "3749", r)
}

func TestPuzzle2(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle2(input)
	// fmt.Println(r)
	assert.Equal(t, "11387", r)

}
