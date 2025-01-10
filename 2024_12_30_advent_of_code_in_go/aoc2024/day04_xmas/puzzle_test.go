package day04_xmas

import (
	"aoc/utils"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestAvailableDirections2(t *testing.T) {
	worldlen := 4
	size := 10

	assert.Equal(t, dirsWitoutTop, availableDirections(5, 0, worldlen, size))
	assert.Equal(t, dirsWitoutTop, availableDirections(5, 1, worldlen, size))
	assert.Equal(t, dirsWitoutTop, availableDirections(5, 2, worldlen, size))
	assert.Equal(t, dirsAll, availableDirections(5, 3, worldlen, size))

	assert.Equal(t, dirsWitoutBottom, availableDirections(5, 9, worldlen, size))
	assert.Equal(t, dirsWitoutBottom, availableDirections(5, 8, worldlen, size))
	assert.Equal(t, dirsWitoutBottom, availableDirections(5, 7, worldlen, size))
	assert.Equal(t, dirsAll, availableDirections(5, 6, worldlen, size))

	assert.Equal(t, dirsWitoutLeft, availableDirections(0, 5, worldlen, size))
	assert.Equal(t, dirsWitoutLeft, availableDirections(1, 5, worldlen, size))
	assert.Equal(t, dirsWitoutLeft, availableDirections(2, 5, worldlen, size))
	assert.Equal(t, dirsAll, availableDirections(3, 5, worldlen, size))

	assert.Equal(t, dirsWitoutRight, availableDirections(9, 5, worldlen, size))
	assert.Equal(t, dirsWitoutRight, availableDirections(8, 5, worldlen, size))
	assert.Equal(t, dirsWitoutRight, availableDirections(7, 5, worldlen, size))
	assert.Equal(t, dirsAll, availableDirections(6, 5, worldlen, size))

	assert.Equal(t, dirsWitoutTopAndLeft, availableDirections(0, 0, worldlen, size))
	assert.Equal(t, dirsWitoutTopAndRight, availableDirections(size-1, 0, worldlen, size))
	assert.Equal(t, dirsWitoutBottomAndLeft, availableDirections(0, size-1, worldlen, size))
	assert.Equal(t, dirsWitoutBottomAndRight, availableDirections(size-1, size-1, worldlen, size))
}

func TestMove(t *testing.T) {
	var x, y int
	x, y = move(5, 10, 2, DirsT)
	assert.Equal(t, [...]int{5, 10 - 2}, [...]int{x, y})
	x, y = move(5, 10, 2, DirsTR)
	assert.Equal(t, [...]int{5 + 2, 10 - 2}, [...]int{x, y})
	x, y = move(5, 10, 2, DirsR)
	assert.Equal(t, [...]int{5 + 2, 10}, [...]int{x, y})
	x, y = move(5, 10, 2, DirsBR)
	assert.Equal(t, [...]int{5 + 2, 10 + 2}, [...]int{x, y})
	x, y = move(5, 10, 2, DirsB)
	assert.Equal(t, [...]int{5, 10 + 2}, [...]int{x, y})
	x, y = move(5, 10, 2, DirsBL)
	assert.Equal(t, [...]int{5 - 2, 10 + 2}, [...]int{x, y})
	x, y = move(5, 10, 2, DirsL)
	assert.Equal(t, [...]int{5 - 2, 10}, [...]int{x, y})
	x, y = move(5, 10, 2, DirsTL)
	assert.Equal(t, [...]int{5 - 2, 10 - 2}, [...]int{x, y})
}

func TestPuzzle1(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle1(input)
	// fmt.Println(r)
	assert.Equal(t, "18", r)
}

func TestPuzzle2(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle2(input)
	//fmt.Println(r)
	assert.Equal(t, "9", r)
}
