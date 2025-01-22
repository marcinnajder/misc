package day09_compacting

import (
	"aoc/utils"
	"fmt"
	"testing"

	"github.com/stretchr/testify/assert"
	"golang.org/x/exp/slices"
)

func TestLoadData(t *testing.T) {
	blocks := loadData("12345")
	assert.Equal(t, []Block{{0, 1}, {-1, 2}, {1, 3}, {-1, 4}, {2, 5}}, blocks)
}

func TestCompact(t *testing.T) {
	tests := []struct {
		input    string
		expected []Block
	}{
		{"12345", []Block{{0, 1}, {2, 2}, {1, 3}, {2, 3}}},
		{"2333133121414131402", []Block{{0, 2}, {9, 2}, {8, 1}, {1, 3}, {8, 3}, {2, 1}, {7, 3}, {3, 3}, {6, 1}, {4, 2}, {6, 1}, {5, 4}, {6, 2}}},
	}

	for _, tt := range tests {
		testname := fmt.Sprintf("input:%s", tt.input)
		t.Run(testname, func(t *testing.T) {
			blocks := loadData(tt.input)
			compacted := compact1(blocks)
			// fmt.Println(compacted)
			assert.Equal(t, tt.expected, compacted)
		})
	}
}

func TestPuzzle1(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle1(input)
	//fmt.Println(r)
	assert.Equal(t, "1928", r)
}

func TestPuzzle2(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle2(input)
	// fmt.Println(r)
	assert.Equal(t, "2858", r)
}

func TestCompact2(t *testing.T) {
	tests := []struct {
		input    string
		expected []Block
	}{
		{"2333133121414131402", []Block{{0, 2}, {9, 2}, {2, 1}, {1, 3}, {7, 3}, {-1, 1}, {4, 2}, {-1, 1}, {3, 3}, {-1, 1}, {-1, 2}, {-1, 1}, {5, 4}, {-1, 1}, {6, 4}, {-1, 1}, {-1, 3}, {-1, 1}, {8, 4}, {-1, 0}, {-1, 2}}},
	}

	for _, tt := range tests {
		testname := fmt.Sprintf("input:%s", tt.input)
		t.Run(testname, func(t *testing.T) {
			blocks := loadData(tt.input)
			compacted := compact2(slices.Clone(blocks))
			compacted_ := compact2_(slices.Clone(blocks))

			// fmt.Println(compacted_, checksum(compacted_))

			assert.Equal(t, tt.expected, compacted)
			assert.Equal(t, tt.expected, compacted_)
			assert.Equal(t, 2858, checksum(compacted))
		})
	}
}
