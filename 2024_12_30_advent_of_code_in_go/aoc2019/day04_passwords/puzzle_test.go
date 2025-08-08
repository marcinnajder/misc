package day04_passwords

import (
	"aoc/utils"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestLoadData(t *testing.T) {
	minNums, maxNums := loadData("254032-789860")
	assert.Equal(t, []int{2, 5, 4, 0, 3, 2}, minNums)
	assert.Equal(t, []int{7, 8, 9, 8, 6, 0}, maxNums)
}

func TestIsValid1(t *testing.T) {

	assert.True(t, isValid1(numberToPassword(222222)))
	assert.True(t, isValid1(numberToPassword(225555)))
	assert.True(t, isValid1(numberToPassword(225566)))
	assert.True(t, isValid1(numberToPassword(123455)))

	assert.False(t, isValid1(numberToPassword(225544)))
	assert.False(t, isValid1(numberToPassword(123456)))
	assert.False(t, isValid1(numberToPassword(224454)))
}

func TestIsValid2(t *testing.T) {
	// additional explanation was necessary, test cases copied from link belowe
	// https://www.reddit.com/r/adventofcode/comments/e65jgt/2019_day_4_part_2_am_i_misunderstanding_the_given/
	assert.False(t, isValid2(numberToPassword(123444)))
	assert.False(t, isValid2(numberToPassword(124444)))

	assert.True(t, isValid2(numberToPassword(113334)))

	assert.True(t, isValid2(numberToPassword(113345)))
	assert.True(t, isValid2(numberToPassword(111122)))
	assert.True(t, isValid2(numberToPassword(112233)))
	assert.True(t, isValid2(numberToPassword(123445)))

}

func TestPuzzle1(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	result := Puzzle1(input)
	// fmt.Println(result)
	assert.Equal(t, "1033", result)
}

func TestPuzzle2(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	result := Puzzle2(input)
	//fmt.Println(result)
	assert.Equal(t, "670", result)
}
