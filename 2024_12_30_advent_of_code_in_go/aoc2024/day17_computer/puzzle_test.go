package day17_computer

import (
	"aoc/utils"
	"fmt"
	"slices"
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestLoadData(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	data := loadData(input)
	fmt.Println(data)
	// assert.Equal(t, 15, data.size)
	// assert.Equal(t, Point{1, 13}, data.start)
	// assert.Equal(t, Point{13, 1}, data.end)
}

func TestProcess(t *testing.T) {
	// d := process(Data{0, 2024, 43690, []int{4, 0}})
	// fmt.Println(d)

	assert.Equal(t, Data{0, 1, 9, []int{}}, process(Data{0, 0, 9, []int{2, 6}}))
	assert.Equal(t, Data{10, 0, 0, []int{0, 1, 2}}, process(Data{10, 0, 0, []int{5, 0, 5, 1, 5, 4}}))
	assert.Equal(t, Data{0, 0, 0, []int{4, 2, 5, 6, 7, 7, 7, 7, 3, 1, 0}}, process(Data{2024, 0, 0, []int{0, 1, 5, 4, 3, 0}}))
	assert.Equal(t, Data{0, 26, 0, []int{}}, process(Data{0, 29, 0, []int{1, 7}}))
	assert.Equal(t, Data{0, 44354, 43690, []int{}}, process(Data{0, 2024, 43690, []int{4, 0}}))
}

func TestPuzzle1(t *testing.T) {
	input := utils.ReadTextFile("./data_.txt")
	r := Puzzle1(input)
	fmt.Println(r)
	// assert.Equal(t, "11048", r)
}

// for i := range 117440 + 1 {
// 	data := Data{i, 0, 0, []int{0, 3, 5, 4, 3, 0}}
// 	r := process2(data)
// 	fmt.Println(data, "->", r)
// }

// 1504520 35184387387069
// 256 35184387387325
// 261888 35184387649213
// 256 35184387649469
// 199160 35184387848629
// 66824 35184387915453
// 256 35184387915709
// 63992 35184387979701
// 1966080 35184389945781
// 66824 35184390012605
// 256 35184390012861
// 63992 35184390076853

func TestPuzzle2(t *testing.T) {
	numbers := []int{2, 4, 1, 1, 7, 5, 1, 5, 4, 0, 0, 3, 5, 5, 3, 0}

	//start := int(math.Pow(8, 15))
	// end := int(math.Pow(8, 16))
	// jumps := -1
	j := 0
	jj := 0
	k := 0

	// start := 35184387387069
	// diffs := []int{1504520, 256, 261888, 256, 199160, 66824, 256, 63992, 1966080, 66824, 256, 63992}

	start := 37702043791037
	diffs := []int{426599, 1, 23, 1, 23, 1, 16871, 1, 1511, 1, 23, 1, 23, 1, 121319, 1, 73727, 1, 130943, 1, 12671, 1, 2687, 1, 771071, 1, 12671, 1, 2687, 1, 771071, 1, 12671, 1, 2687, 1, 771071, 1, 12671, 1, 2687, 1}

	//38801555418813
	for i := start; j < 100000000; i += diffs[k] {
		//for i := start; j < 100000000; i += int(math.Pow(8, 0)) {
		//for i := start; j < 1000000; i += int(math.Pow(8, 12)) { // 10000000 timeout
		//for i := range 100000 {
		data := Data{i, 0, 0, numbers}
		r, _ := process2(data)

		// if jumps != jumps2 {
		// 	fmt.Println(i, jumps2, len(data.numbers), len(r.numbers), data, "->", r)
		// 	jumps = jumps2
		// }

		// fmt.Println(len(r.numbers), r.numbers, j, i)

		if slices.Equal(r.numbers, numbers) {
			panic(i)
		}
		// Program: 2,4,1,1,7,5,1,5,4,0,0,3,5,5,3,0

		// 383
		if r.numbers[0] == numbers[0] && r.numbers[1] == numbers[1] && r.numbers[2] == numbers[2] && r.numbers[3] == numbers[3] && r.numbers[4] == numbers[4] && r.numbers[5] == numbers[5] && r.numbers[6] == numbers[6] && r.numbers[7] == numbers[7] && r.numbers[8] == numbers[8] && r.numbers[9] == numbers[9] && r.numbers[10] == numbers[10] && r.numbers[11] == numbers[11] {
			//if r.numbers[0] == numbers[0] && r.numbers[1] == numbers[1] && r.numbers[2] == numbers[2] && r.numbers[3] == numbers[3] && r.numbers[4] == numbers[4] && r.numbers[5] == numbers[5] && r.numbers[6] == numbers[6] && r.numbers[7] == numbers[7] && r.numbers[8] == numbers[8] && r.numbers[9] == numbers[9] && r.numbers[10] == numbers[10] && r.numbers[11] == numbers[11] && r.numbers[12] == numbers[12] {
			//if r.numbers[0] == numbers[0] && r.numbers[1] == numbers[1] && r.numbers[2] == numbers[2] && r.numbers[3] == numbers[3] && r.numbers[4] == numbers[4] {
			//if r.numbers[0] == numbers[0] && r.numbers[1] == numbers[1] && r.numbers[2] == numbers[2] && r.numbers[3] == numbers[3] {

			fmt.Println(j-jj, i)
			jj = j
		}

		// if r.numbers[0] == numbers[0] && r.numbers[1] == numbers[1] && r.numbers[2] == numbers[2] && r.numbers[3] == numbers[3] {
		// 	panic(i)
		// }

		j++
		k = (k + 1) % len(diffs)
	}

	// data := Data{117440, 0, 0, []int{0, 3, 5, 4, 3, 0}}
	// r := process2(data)
	// fmt.Println(r)
	// assert.Equal(t, "11048", r)
}

// Register A: 2024
// Register B: 0
// Register C: 0

// Program: 0,3,5,4,3,0

// func TestPuzzle2(t *testing.T) {
// 	input := utils.ReadTextFile("./data__.txt")
// 	r := Puzzle2(input)
// 	// fmt.Println(r)
// 	assert.Equal(t, "583", r)
// }

// func TestPuzzle2WithInput2(t *testing.T) {
// 	r := Puzzle2(input2)
// 	//fmt.Println(r)
// 	assert.Equal(t, "64", r)
// }
