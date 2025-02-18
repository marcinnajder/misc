package day16_maze

import (
	"aoc/utils"
	"testing"

	"github.com/stretchr/testify/assert"
)

// maze with walls on both sides of starting point
var input2 = `#################
#...#...#...#..E#
#.#.#.#.#.#.#.#.#
#.#.#.#...#...#.#
#.#.#.#.###.#.#.#
#...#.#.#.....#.#
#.#.#.#.#.#####.#
#.#...#.#.#.....#
#.#.#####.#.###.#
#.#.#.......#...#
#.#.###.#####.###
#.#.#...#.....#.#
#.#.#.#####.###.#
#.#.#.........#.#
#.#.#.#########.#
#S#.............#
#################`

func TestLoadData(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	data := loadData(input)
	assert.Equal(t, 15, data.size)
	assert.Equal(t, Point{1, 13}, data.start)
	assert.Equal(t, Point{13, 1}, data.end)
}

func TestLoadDataWithInput2(t *testing.T) {
	data := loadData(input2)
	assert.Equal(t, 17, data.size)
	assert.Equal(t, Point{1, 15}, data.start)
	assert.Equal(t, Point{15, 1}, data.end)
}

func TestPuzzle1(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle1(input)
	// fmt.Println(r)
	assert.Equal(t, "7036", r)
}

func TestPuzzle1WithInput2(t *testing.T) {
	r := Puzzle1(input2)
	// fmt.Println(r)
	assert.Equal(t, "11048", r)
}

func TestPuzzle2(t *testing.T) {
	input := utils.ReadTextFile("./data__.txt")
	r := Puzzle2(input)
	// fmt.Println(r)
	assert.Equal(t, "583", r)
}

func TestPuzzle2WithInput2(t *testing.T) {
	r := Puzzle2(input2)
	//fmt.Println(r)
	assert.Equal(t, "64", r)
}
