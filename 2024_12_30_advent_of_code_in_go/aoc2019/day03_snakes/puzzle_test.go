package day03_snakes

import (
	"aoc/utils"
	"slices"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestLoadData(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	moves1, moves2 := loadData(input)
	// fmt.Println(moves1)
	// fmt.Println(moves2)
	assert.Equal(t, 9, len(moves1))
	assert.Equal(t, 8, len(moves2))
}

func TestWalk(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	moves1, moves2 := loadData(input)
	_ = moves2
	moves2Slice := slices.Collect(walk(moves1, Point{0, 0}))
	// fmt.Println(moves2Slice)
	assert.Equal(t, Line{true, 0, Range{0, 75}, 0}, moves2Slice[0])
	assert.Equal(t, Line{false, 75, Range{0, -30}, 75}, moves2Slice[1])
}

func TestRange(t *testing.T) {
	assert.Equal(t, Range{5, 10}.contains(5), true)
	assert.Equal(t, Range{5, 10}.contains(10), true)
	assert.Equal(t, Range{5, 10}.contains(7), true)
	assert.Equal(t, Range{5, 10}.contains(4), false)
	assert.Equal(t, Range{5, 10}.contains(11), false)

	assert.Equal(t, Range{5, -5}.contains(0), true)
	assert.Equal(t, Range{5, -5}.contains(-6), false)
	assert.Equal(t, Range{5, -5}.contains(6), false)
}

func TestPuzzle1(t *testing.T) {
	input1 := utils.ReadTextFile("./data_.txt")
	input2 := "R98,U47,R26,D63,R33,U87,L62,D20,R33,U53,R51\nU98,R91,D20,R16,D67,R40,U7,R15,U6,R7"
	inOut := map[string]string{input1: "159", input2: "135"}

	for in, out := range inOut {
		outt := Puzzle1(in)
		//fmt.Println(outt)
		assert.Equal(t, outt, out)
	}
}

func TestPuzzle2(t *testing.T) {
	input1 := utils.ReadTextFile("./data_.txt")
	input2 := "R98,U47,R26,D63,R33,U87,L62,D20,R33,U53,R51\nU98,R91,D20,R16,D67,R40,U7,R15,U6,R7"
	inOut := map[string]string{input1: "610", input2: "410"}

	for in, out := range inOut {
		outt := Puzzle2(in)
		//fmt.Println(outt)
		assert.Equal(t, outt, out)
	}
}

func TestFindOverlappingLines(t *testing.T) {

	// vertical lines in 4 columns (0, 5, 10 ,15) with a different heights
	lines := []Line{
		{false, 0, Range{2, 4}, 0},
		{false, 5, Range{6, 8}, 0},
		{false, 10, Range{1, 10}, 0},
		{false, 15, Range{11, 15}, 0},
	}

	assertLines := func(line Line, expected []Line) {
		for i := range 2 {
			var actual []Line
			if i == 0 {
				actual = slices.Collect(findOverlappingLines(lines, line))
			} else {
				linesSorted := slices.SortedFunc(slices.Values(lines), compareLineByPos)
				actual = slices.Collect(findOverlappingLinesOrdered(linesSorted, line))
			}
			slices.SortFunc(actual, compareLineByPos)
			assert.Equal(t, expected, actual)
		}
	}

	assertLines(Line{true, 7, Range{0, 5}, 0}, []Line{{false, 5, Range{6, 8}, 0}})
	assertLines(Line{true, 7, Range{0, 10}, 0}, []Line{{false, 5, Range{6, 8}, 0}, {false, 10, Range{1, 10}, 0}})
	assertLines(Line{true, 8, Range{6, 10}, 0}, []Line{{false, 10, Range{1, 10}, 0}})
}
