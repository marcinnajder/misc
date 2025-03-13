package day17_computer

import (
	"aoc/utils"
	"fmt"
	"iter"
	"math"
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

// func TestPuzzle3___(t *testing.T) {
// 	numbers := []int{2, 4, 1, 1, 7, 5, 1, 5, 0, 3, 4, 3, 5, 5, 3, 0}
// 	end := int(math.Pow(8, 16))
// 	for i := 39945837047485; i < end; i++ {
// 		data := Data{i, 0, 0, numbers}
// 		r, _ := process2(data)

// 		if compareFirstN(numbers, r.numbers, 16) {
// 			fmt.Println(i, "tutaj", r)
// 			panic("koniec")
// 		}

// 	}
// }

func TestPuzzle2Final(t *testing.T) {
	numbers := []int{2, 4, 1, 1, 7, 5, 1, 5, 4, 0, 0, 3, 5, 5, 3, 0} // github -> 164541160582845 (0.00s)
	// numbers := []int{2, 4, 1, 1, 7, 5, 1, 5, 0, 3, 4, 3, 5, 5, 3, 0} // google -> 164542125272765 (8s)
	// numbers := []int{2, 4, 1, 3, 7, 5, 1, 5, 0, 3, 4, 3, 5, 5, 3, 0} // MH -> timeout

	start := int(math.Pow(8, 15))
	end := int(math.Pow(8, 16))

	foundIndexes := make([]int, 0, 16)

	searchStart := start

	offsetTotal := 0
	for n := 0; n < 15; n++ {

		for i := searchStart; i < end; i++ {

			data := Data{i, 0, 0, numbers}
			r, _ := process2(data)

			// fmt.Println(r)
			if slices.Equal(r.numbers, numbers) {
				fmt.Println("solution found", i)
				return
			}

			if r.numbers[0] == numbers[16-n-1] { // always match the number at first index to the next number in final sequnces
				offset := i - searchStart
				foundIndexes = append(foundIndexes, offset)
				foundIndexesLen := len(foundIndexes)

				offsetTotal = 0
				for j := 1; j <= foundIndexesLen; j++ {
					offsetTotal += int(math.Pow(8, float64(j))) * foundIndexes[foundIndexesLen-j]
				}

				searchStart = start + offsetTotal
				// fmt.Println(r, i, searchStart, offsetTotal)
				break
			}
		}
	}

	// using 'offsetTotal' intead of 'searchStart' :////
	for i := offsetTotal; i < end; i++ {
		data := Data{i, 0, 0, numbers}
		r, _ := process2(data)
		if slices.Equal(r.numbers, numbers) {
			fmt.Println("koniec", i)
			return
		}
	}

	fmt.Println(offsetTotal)

	fmt.Println("solution not found")
}

// ------------------------------------------------------------------------------------------------------------
// - analysis of generated numbers

// -- the number of generated numbers changes with the power of 8, if we want to have 16 numbers we need to start from A=16^15
func TestWhenLengthOfGeneratedNumbersChanges(t *testing.T) {
	numbers := []int{2, 4, 1, 1, 7, 5, 1, 5, 4, 0, 0, 3, 5, 5, 3, 0}

	prevLen := -1
	for i := 0; i < 5000; i++ {
		data := Data{i, 0, 0, numbers}
		r, _ := process2(data)

		if len(r.numbers) != prevLen {
			fmt.Println(i, r.numbers, len(r.numbers))
			prevLen = len(r.numbers)
		}
	}

	// 0 [4] 1
	// 8 [0 4] 2
	// 64 [4 0 4] 3
	// 512 [4 4 0 4] 4
	// 4096 [4 4 4 0 4] 5
}

// -- with each interation of power of 8 next value in generated numbers changes
func TestWhenValueOfNextNumberChanges(t *testing.T) {

	numbers := []int{2, 4, 1, 1, 7, 5, 1, 5, 4, 0, 0, 3, 5, 5, 3, 0}
	start := int(math.Pow(8, 15))

	rinit, _ := process2(Data{start, 0, 0, numbers})
	changes := make(map[int]struct{})

	n := 0
	for i := start; ; i += int(math.Pow(8, 0)) {
		data := Data{i, 0, 0, numbers}
		r, _ := process2(data)
		// fmt.Println(r.numbers, i)

		for z := 0; z < 16; z++ {
			if r.numbers[z] != rinit.numbers[z] {
				if _, ok := changes[z]; !ok {
					changes[z] = struct{}{}
					fmt.Println("change", n, z, i, r.numbers)
				}
			}
		}

		n++
		if n > 1200 {
			return
		}
	}

	// change 2 0 35184372088834 [6 4 4 4 4 4 4 4 4 4 4 4 4 4 0 4]
	// change 16 1 35184372088848 [4 6 4 4 4 4 4 4 4 4 4 4 4 4 0 4]			2         *2*2*2
	// change 128 2 35184372088960 [4 4 6 4 4 4 4 4 4 4 4 4 4 4 0 4]		2 *2*2*2  * 2*2*2
	// change 1024 3 35184372089856 [4 4 4 6 4 4 4 4 4 4 4 4 4 4 0 4]		2*2*2*2  * 2*2*2  * 2*2*2
}

// - so we know exactly the value of A when particular number is changed
func TestWhenValueOfNextNumberChanges2(t *testing.T) {
	numbers := []int{2, 4, 1, 1, 7, 5, 1, 5, 4, 0, 0, 3, 5, 5, 3, 0}
	start := int(math.Pow(8, 15))

	j := 2
	for i := 1; i < 16; i++ {
		j = j * 8 // 16, 128, 1024, ...
		data := Data{start + j, 0, 0, numbers}

		r, _ := process2(data)
		fmt.Println(j, r.numbers, i, start+j)
	}

	// 16 [4 6 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 1 35184372088848
	// 128 [4 4 6 4 4 4 4 4 4 4 4 4 4 4 0 4] 2 35184372088960
	// 1024 [4 4 4 6 4 4 4 4 4 4 4 4 4 4 0 4] 3 35184372089856
	// 8192 [4 4 4 4 6 4 4 4 4 4 4 4 4 4 0 4] 4 35184372097024
	// 65536 [4 4 4 4 4 6 4 4 4 4 4 4 4 4 0 4] 5 35184372154368
	// 524288 [4 4 4 4 4 4 6 4 4 4 4 4 4 4 0 4] 6 35184372613120
	// 4194304 [4 4 4 4 4 4 4 6 4 4 4 4 4 4 0 4] 7 35184376283136
	// 33554432 [4 4 4 4 4 4 4 4 6 4 4 4 4 4 0 4] 8 35184405643264
	// 268435456 [4 4 4 4 4 4 4 4 4 6 4 4 4 4 0 4] 9 35184640524288
	// 2147483648 [4 4 4 4 4 4 4 4 4 4 6 4 4 4 0 4] 10 35186519572480
	// 17179869184 [4 4 4 4 4 4 4 4 4 4 4 6 4 4 0 4] 11 35201551958016
	// 137438953472 [4 4 4 4 4 4 4 4 4 4 4 4 6 4 0 4] 12 35321811042304
	// 1099511627776 [4 4 4 4 4 4 4 4 4 4 4 4 4 6 0 4] 13 36283883716608
	// 8796093022208 [4 4 4 4 4 4 4 4 4 4 4 4 4 4 7 4] 14 43980465111040
	// 70368744177664 [4 4 4 4 4 4 4 4 4 4 4 4 4 4 0 7] 15 105553116266496
}

// - we can see the pattern how values changes after change of last number (first numbers change with different pattern)
func TestChangesAfterLastNumberChange(t *testing.T) {
	numbers := []int{2, 4, 1, 1, 7, 5, 1, 5, 4, 0, 0, 3, 5, 5, 3, 0}

	start := 105553116266496 // the moment when the last number is changed (last number is 7 instead of 4 for the first time )
	n := 0

	for i := start; ; i += int(math.Pow(8, 1)) {
		// for i := start - 500; ; i++ {
		data := Data{i, 0, 0, numbers}
		r, _ := process2(data)
		fmt.Println(r.numbers, i)

		n++
		if n > 100 {
			return
		}
	}
}

// ------------------------------------------------------------------------------------------------------------
// solution 1
// - a) TestFindingDiffPatternsAfterFewCycles
// - finding pattern of diffs when repeating sequence of number is found ("2,4" then "2,4,1" then "2,4,1,1" then ...)
// - it works only by change for some data, the main reseason it does not work is that pattern is broken at some point
// (around genering 13th number, more details later)
// - b) TestDeterminateNextItemByLookingAtLast4Items
// - try to determinate next number by looking at last 4 numbers
// - but here the problem is that next number is not deterministic meaning that after the same sequence of the same 4 number,
// the 5th can be different :/
// - summary
// -- so in general we can not assume all numbers are generated using the same pattern
// -- but we are able to find the solution for some specific input data ...
// -- ... or we can user patterns to speed up the "search space" of possible values of A

func TestFindingDiffPatternsAfterFewCycles(t *testing.T) {
	numbers := []int{2, 4, 1, 1, 7, 5, 1, 5, 4, 0, 0, 3, 5, 5, 3, 0} // github account (solution is found)
	// numbers := []int{2, 4, 1, 1, 7, 5, 1, 5, 0, 3, 4, 3, 5, 5, 3, 0} // google account (solution is not found, stops at 13th number)
	// numbers := []int{2, 4, 1, 3, 7, 5, 1, 5, 0, 3, 4, 3, 5, 5, 3, 0} // MH account (solution is not found, timeout)
	reps := 2

	start := int(math.Pow(8, 15))
	end := int(math.Pow(8, 16))

	nFirst := 2

	diffsseq := iterateByOne()

loop:
	for {
		i := start
		iprev := i

		diffs := make([]int, 0)
		ivalues := make([]int, 0)

		for diff := range diffsseq {
			i += diff

			if i > end {
				panic("solution not found")
			}

			data := Data{i, 0, 0, numbers}
			r, _ := process2(data)

			if compareFirstN(numbers, r.numbers, nFirst) {
				diff := i - iprev
				iprev = i

				diffs = append(diffs, diff)
				ivalues = append(ivalues, i)

				if compareFirstN(numbers, r.numbers, 16) {
					fmt.Println("solution found:", i, r)
					return
				}

				if patternLen := findPatten(diffs, reps, 2); patternLen != -1 {
					pattern := diffs[len(diffs)-patternLen:]

					//fmt.Println(nFirst, patternLen, pattern, r)
					fmt.Println()
					fmt.Println("nFirst", nFirst)
					fmt.Println("DIFFS", len(diffs), diffs)
					fmt.Println("PATTERN", len(pattern), pattern)

					// start = ivalues[len(diffs)-reps*patternLen] - diffs[len(diffs)-reps*patternLen]
					start = ivalues[0] - diffs[0]
					diffsseq = iterateByDiffs(diffs, pattern)
					nFirst++

					continue loop
				}
			}
		}
	}
}

func iterateByOne() iter.Seq[int] {
	return func(yield func(int) bool) {
		for i := 0; i < math.MaxInt64; i++ {
			if !yield(1) {
				return
			}
		}
	}
}
func iterateByDiffs(diffs, pattern []int) iter.Seq[int] {
	return func(yield func(int) bool) {
		for _, diff := range diffs { // return all values from diff
			if !yield(diff) {
				return
			}
		}
		for {
			for _, diff := range pattern { // then forever return values from found pattern
				if !yield(diff) {
					return
				}
			}
		}
	}
}

// pattern with at least 2 items long repeated at least 2 times
func findPatten[T comparable](items []T, reps, minLen int) int {
	itemsLen := len(items)
	if reps < 2 || minLen < 2 || itemsLen < reps*minLen {
		// panic("invalid arguments: reps<2, minLen<2, len(items)<reps*minLen)")
		return -1
	}

	repsm1 := reps - 1
	minI := (repsm1 * minLen) - 1
	lastI := itemsLen - 1
	last := items[lastI]

main:
	for i := lastI - 1; i >= minI; i-- {
		patternLen := lastI - i

		if patternLen >= minLen && items[i] == last && (i-(repsm1)*patternLen >= 0) { // enough space for all reps
			petternI := i + 1
			pattern := items[petternI:itemsLen]

			for j, p := range pattern {
				for r := range repsm1 {
					if p != items[petternI+j-((r+1)*patternLen)] {
						continue main // :)
					}
				}
			}

			return patternLen
		}
	}

	return -1
}

func TestFindPattern(t *testing.T) {
	// arguments validation
	assert.Panics(t, func() { findPatten([]int{1, 2, 3}, 1, 2) })
	assert.Panics(t, func() { findPatten([]int{1, 2, 3}, 2, 1) })
	assert.Panics(t, func() { findPatten([]int{1, 2, 3}, 2, 2) })
	assert.NotPanics(t, func() { findPatten([]int{1, 2, 3, 4}, 2, 2) })

	// 00000 123 123
	assert.Equal(t, 3, findPatten([]int{0, 0, 0, 0, 0, 0, 1, 2, 3, 1, 2, 3}, 2 /*reps*/, 2 /*minLen*/))
	assert.Equal(t, 3, findPatten([]int{0, 0, 0, 0, 0, 0, 1, 2, 3, 1, 2, 3}, 2 /*reps*/, 3 /*minLen*/))
	assert.Equal(t, -1, findPatten([]int{0, 0, 0, 0, 0, 0, 1, 2, 3, 1, 2, 3}, 2 /*reps*/, 4 /*minLen*/))
	assert.Equal(t, -1, findPatten([]int{0, 0, 0, 0, 0, 0, 1, 2, 3, 1, 2, 3}, 3 /*reps*/, 2 /*minLen*/))
	assert.Equal(t, 3, findPatten([]int{0, 0, 0, 1, 2, 3, 1, 2, 3, 1, 2, 3}, 3 /*reps*/, 2 /*minLen*/))
	assert.Equal(t, -1, findPatten([]int{0, 0, 0, 1, 2, 3, 1, 2, 3, 1, 2, 3}, 4 /*reps*/, 2 /*minLen*/))

	// 0 1234 123 1234 123 - last element '3' in also in the middle of pattern 1234123
	assert.Equal(t, 7, findPatten([]int{0, 1, 2, 3, 4, 1, 2, 3, 1, 2, 3, 4, 1, 2, 3}, 2 /*reps*/, 2 /*minLen*/))

	// r := findPatten([]int{0, 1, 2, 3, 4, 1, 2, 3, 1, 2, 3, 4, 1, 2, 3}, 2 /*reps*/, 2 /*minLen*/)
	// _ = r
}

func TestDeterminateNextItemByLookingAtLast4Items(t *testing.T) {
	numbers := []int{2, 4, 1, 1, 7, 5, 1, 5, 4, 0, 0, 3, 5, 5, 3, 0}
	// numbers := []int{2, 4, 1, 1, 7, 5, 1, 5, 0, 3, 4, 3, 5, 5, 3, 0}

	start := int(math.Pow(8, 15))
	// end := int(math.Pow(8, 16))
	nFirst := 2
	diffsseq := iterateByOneAlways()

	for {
		i := start
		iprev := i

		diffs := make([]int, 0)
		ivalues := make([]int, 0)

		diffsseq(func(diff []int) (int, bool) {

			var diffNext int

			if len(diff) == 1 { // if diff is exactly one possible diff, use it
				diffNext = diff[0]
			} else { // if there are many possible diffs, try to check for which value pattern is met
				diffNextList := make([]int, 0)
				for _, d := range diff {
					r, _ := process2(Data{i + d, 0, 0, numbers})
					if compareFirstN(numbers, r.numbers, nFirst) {
						diffNextList = append(diffNextList, d)
					}
				}

				if oj := len(diffNextList); oj != 1 { // zero or more than one possible diffs, check the same but with longer pattern

					diffNextList2 := make([]int, 0)
					for _, d := range diff {
						r, _ := process2(Data{i + d, 0, 0, numbers})
						if compareFirstN(numbers, r.numbers, nFirst-1) {
							diffNextList2 = append(diffNextList2, d)
						}
					}

					if len(diffNextList2) != 1 {
						panic("???")
					}

					diffNext = diffNextList2[0]
				} else { // only one diff holds the pattern, use it
					diffNext = diffNextList[0]
				}

			}

			if diffNext != 1 {
				fmt.Println(diffNext)
			}

			i += diffNext
			data := Data{i, 0, 0, numbers}
			r, _ := process2(data)

			if compareFirstN(numbers, r.numbers, nFirst) {
				if compareFirstN(numbers, r.numbers, 16) {
					fmt.Println("solution is found", i, r)
					panic("solution is found")
				}

				diff := i - iprev
				iprev = i

				diffs = append(diffs, diff)
				ivalues = append(ivalues, i)

				if len(diffs) == 1000 { // change the way we iterate the diff after 1000 steps
					start = ivalues[0] - diffs[0]
					diffsseq = iterateWithNextPossibleValues(diffs)
					nFirst++

					return diffNext, false
				}
			}

			return diffNext, true

		})
	}
}

type Last4 struct {
	val1, val2, val3, val4 int
}

// '[]int' - sequence generates values '[]int' (not one but many possible next numbers)
// '(int, bool)' - can stop pulling values and choose one from the possible next values
type MySeq func(yield func([]int) (int, bool))

func iterateByOneAlways() MySeq {
	one := []int{1}
	return func(yield func([]int) (int, bool)) {
		for i := 0; i < math.MaxInt64; i++ {
			if _, ok := yield(one); !ok {
				return
			}
		}
	}
}

func iterateWithNextPossibleValues(diffs []int) MySeq {

	// mapping from last 4 values into set of next possible values
	lastmap := make(map[Last4][]int)

	return func(yield func([]int) (int, bool)) {
		last := Last4{0, 0, 0, 0}

		for _, diff := range diffs { // just iterate all passed diffs and create map

			if d, ok := lastmap[last]; ok {
				if d[0] != diff {
					if slices.Index(d, diff) == -1 {
						lastmap[last] = append(d, diff)
					}
				}
			} else {
				lastmap[last] = []int{diff}
			}

			last = Last4{last.val2, last.val3, last.val4, diff}

			if _, ok := yield([]int{diff}); !ok {
				return
			}
		}

		for { // iterate forever using created map
			if diff, ok := lastmap[last]; !ok {
				panic("we dont know what the next values is :/ this sequences of 4 last values don't appear in the past ")
			} else {
				if choosen, ok := yield(diff); !ok {
					return
				} else {
					last = Last4{last.val2, last.val3, last.val4, choosen}
				}
			}
		}
	}
}

// ------------------------------------------------------------------------------------------------------------
// - solution 2
// - if we generate output numbers for consecutive values of A some pattern emarges
// [4 4 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088832
// [4 4 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088833
// [6 4 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088834
// [7 4 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088835
// [0 4 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088836
// [1 4 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088837
// [2 4 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088838
// [3 4 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088839 // 8
// [0 4 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088840
// [4 4 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088841
// [7 4 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088842
// [5 4 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088843
// [0 4 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088844
// [1 4 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088845
// [2 4 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088846
// [3 4 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088847
// [4 6 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088848 change 16 1 35184372088848
// [4 6 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088849
// [4 6 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088850
// [3 6 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088851
// [0 6 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088852
// [0 6 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088853
// [2 6 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088854
// [3 6 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088855
// [0 7 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088856
// [4 7 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088857
// [5 7 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088858
// [1 7 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088859
// [0 7 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088860
// [0 7 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088861
// [2 7 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088862
// [3 7 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088863 // 32
// [4 0 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088864
// [4 0 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088865
// [2 0 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088866
// [7 0 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088867
// [1 0 4 4 4 4 4 4 4 4 4 4 4 4 0 4] 35184372088868
// - just look at first 8 lines, only first number is changed in particular order 4,4,6,7,0,1,2,3
// - then in chunks of 8 lines the second number is changed, the number is replicated 8 times and is equal to 4, the 4, the 6, then, 7, ...
// - so with time the next number is changed (we can calculate when exactly) and new numers are generated at first index,
// those numbers are repeated duplicated moving changes to the right
// - we are looking for numbers like "2, 4, 1, 1, 7, 5, 1, 5, 4, 0, 0, 3, 5, 5, 3, 0" so we can write code that jumps genering numbers
// like "0" then "3,0" then "5,3,0" then "5,5,3,0" ... up to the point when all numbers are on its place
// - the problem is that this pattern breaks at 13th number :/ so even if execute all 16 steps moving numbers to the rigth,
// the solution is not found
// - the inner loop belowe finds only first matching number, but there can be more correct numbers, so  the next implementation
// is using recurrency, but still solution is not found

func TestGeneringFinalNumbersUsingLoop(t *testing.T) {
	numbers := []int{2, 4, 1, 1, 7, 5, 1, 5, 4, 0, 0, 3, 5, 5, 3, 0} // 164541160582845
	start := int(math.Pow(8, 15))
	end := int(math.Pow(8, 16))

	foundIndexes := make([]int, 0, 16)

	searchStart := start

	for n := len(numbers) - 1; n >= 0; n-- {

		for i := searchStart; i < end; i++ {

			data := Data{i, 0, 0, numbers}
			r, _ := process2(data)

			// fmt.Println(r)
			if slices.Equal(r.numbers, numbers) {
				fmt.Println("solution found", i)
				return
			}

			if r.numbers[0] == numbers[n] { // always match the number at first index to the next number in final sequnces
				offset := i - searchStart
				foundIndexes = append(foundIndexes, i-searchStart)
				foundIndexesLen := len(foundIndexes)

				offsetTotal := 0
				for j := 1; j <= foundIndexesLen; j++ {
					offsetTotal += int(math.Pow(8, float64(j))) * foundIndexes[foundIndexesLen-j]
				}
				fmt.Println(r, i, offset)

				searchStart = start + offsetTotal
				break
			}
		}
	}

	// {0 4 1 [0 4 4 4 4 4 4 4 4 4 4 4 4 4 0 4]} 35184372088836 4
	// {0 4 1 [3 0 4 4 4 4 4 4 4 4 4 4 4 4 0 4]} 35184372088869 5
	// {0 4 1 [5 3 0 4 4 4 4 4 4 4 4 4 4 4 0 4]} 35184372089131 3
	// {0 4 1 [5 5 3 0 4 4 4 4 4 4 4 4 4 4 0 4]} 35184372091226 2
	// {0 4 1 [3 5 5 3 0 4 4 4 4 4 4 4 4 4 0 4]} 35184372107987 3
	// {0 4 1 [0 3 5 5 3 0 4 4 4 4 4 4 4 4 0 4]} 35184372242072 0
	// {0 4 1 [0 0 3 5 5 3 0 4 4 4 4 4 4 4 0 4]} 35184373314759 7
	// {0 4 1 [4 0 0 3 5 5 3 0 4 4 4 4 4 4 0 4]} 35184381896249 1
	// {0 4 1 [5 4 0 0 3 5 5 3 0 4 4 4 4 4 0 4]} 35184450548171 3
	// {0 4 1 [1 5 4 0 0 3 5 5 3 0 4 4 4 4 0 4]} 35184999763547 3
	// {0 4 1 [5 1 5 4 0 0 3 5 5 3 0 4 4 4 0 4]} 35189393486554 2
	// {0 4 1 [7 5 1 5 4 0 0 3 5 5 3 0 4 4 0 4]} 35224543270614 6
	// {0 4 1 [1 7 5 1 5 4 0 0 3 5 5 3 0 4 0 4]} 35505741543095 7
	// {0 4 1 [1 1 7 5 1 5 4 0 0 3 5 5 3 2 0 4]} 37755327722938 2 // <- here 2 appears instead of 3
	// {0 4 1 [4 1 1 7 5 1 5 4 0 0 3 5 5 7 0 4]} 55752017161680 0
	// {0 1 0 [2 6 1 1 7 5 1 5 4 0 0 3 5 5 3 1]} 199725532671650 34

	fmt.Println("solution not found")
}

func TestGeneringFinalNumbersUsingRec(t *testing.T) {

	numbers := []int{2, 4, 1, 1, 7, 5, 1, 5, 4, 0, 0, 3, 5, 5, 3, 0} // github
	// numbers := []int{2, 4, 1, 1, 7, 5, 1, 5, 0, 3, 4, 3, 5, 5, 3, 0} // google

	start := int(math.Pow(8, 15)) // 35184372088832
	end := int(math.Pow(8, 16))

	val, found := search(start, end, numbers, len(numbers)-1, start, make([]int, 0, 14))

	if found {
		fmt.Println("no jest :)", val)
	}

	fmt.Println("ni ma :(")
}

func search(start int, end int, numbers []int, n int, searchStart int, foundIndexes []int) (val int, found bool) {
	if n >= 0 {
		//searchEnd := searchStart + utils.If(n == 15, 8, int(math.Pow(8, float64(16-n-1))))
		searchEnd := searchStart + 8
		for i := searchStart; i < end && i < searchEnd; i++ {

			data := Data{i, 0, 0, numbers}
			r, _ := process2(data)

			//fmt.Println(r)

			if slices.Equal(r.numbers, numbers) {
				fmt.Println("koniec", i)
				return i, true
			}

			if r.numbers[0] == numbers[n] {
				offset := i - searchStart
				foundIndexesNew := slices.Concat(foundIndexes, []int{offset}) // copy on add
				foundIndexesNewLen := len(foundIndexesNew)

				offsetTotal := 0
				for j := 1; j <= foundIndexesNewLen; j++ {
					offsetTotal += int(math.Pow(8, float64(j))) * foundIndexesNew[foundIndexesNewLen-j]
				}

				searchStartNew := start + offsetTotal

				fmt.Println(r.numbers, n, offset, offsetTotal, i)

				valRec, foundRec := search(start, end, numbers, n-1, searchStartNew, foundIndexesNew)

				if foundRec {
					return valRec, true
				}
			}
		}
	}

	return 0, false
}

// func TestPuzzle3(t *testing.T) {
// 	numbers := []int{2, 4, 1, 1, 7, 5, 1, 5, 4, 0, 0, 3, 5, 5, 3, 0}
// 	// numbers := []int{2, 4, 1, 1, 7, 5, 1, 5, 0, 3, 4, 3, 5, 5, 3, 0}
// 	reps := 2

// 	// rrr, _ := process2(Data{56256477, 0, 0, numbers})
// 	// _ = rrr

// 	//
// 	start := int(math.Pow(8, 15))
// 	end := int(math.Pow(8, 16))
// 	j := 0
// 	jj := 0
// 	k := 0

// 	_ = j
// 	// ------------------------------------------------------------------------------------------

// 	nFirst := 2

// 	// steps
// 	// j++
// 	// k = (k + 1) % len(diffs)
// 	// j = 0

// 	// i := start
// 	diffsseq := iterateI()

// loop:
// 	for {
// 		i := start
// 		iprev := i

// 		diffs := make([]int, 0)
// 		ivalues := make([]int, 0)
// 		for _, diff := range diffsseq {
// 			i += diff

// 			data := Data{i, 0, 0, numbers}
// 			r, _ := process2(data)

// 			if compareFirstN(numbers, r.numbers, nFirst) {
// 				diff := i - iprev
// 				iprev = i

// 				// fmt.Println(diff, i, r.numbers)

// 				diffs = append(diffs, diff)
// 				ivalues = append(ivalues, i)

// 				if compareFirstN(numbers, r.numbers, 16) {
// 					fmt.Println(i, "tutaj", r)
// 					panic("koniec")
// 				}

// 				if patternLen := findPatten(diffs, reps, 2); patternLen != -1 {
// 					pattern := diffs[len(diffs)-patternLen:]

// 					fmt.Println(nFirst, patternLen, pattern, r)

// 					start = ivalues[len(diffs)-reps*patternLen] - diffs[len(diffs)-reps*patternLen]
// 					// start = i - diffs[reps*patternLen]

// 					diffsseq = iterateDiffs(pattern)
// 					nFirst++ // odkomentowac !!

// 					// fmt.Println("len(diffs)", len(diffs), "start", start, "pattern", len(pattern))

// 					continue loop
// 				}
// 			}
// 		}
// 	}

// 	_ = nFirst
// 	_ = end
// 	_ = start
// 	_ = k
// 	_ = jj
// }

func TestPuzzle2(t *testing.T) {
	numbers := []int{2, 4, 1, 1, 7, 5, 1, 5, 4, 0, 0, 3, 5, 5, 3, 0}
	start := int(math.Pow(8, 15))
	end := int(math.Pow(8, 16))
	j := 0
	jj := 0
	k := 0

	// ------------------------------------------------------------------------------------------
	numbers = []int{0, 3, 5, 4, 3, 0}
	//numbers = []int{5, 4, 0, 3, 3, 0}

	start = int(math.Pow(8, float64(len(numbers)-1)))

	b := 1
	for i := start; j < 117440+10; i++ {
		data := Data{i, 0, 0, numbers}
		r, _ := process2(data)

		fmt.Println(r.numbers)

		// if compareFirstN(numbers, r.numbers, b) {
		// 	fmt.Println(j-jj, i, r.numbers)
		// 	jj = j
		// 	b++
		// }

		// if compareFirstN(numbers, r.numbers, len(numbers)) {
		// 	fmt.Println(j-jj, i, r.numbers)
		// 	jj = j
		// }
		j++
	}

	_ = b
	_ = end
	_ = start
	_ = k
	_ = jj
	return

	// ------------------------------------------------------------------------------------------
	// 11
	// start = 37702043791037
	// diffs := []int{426599, 1, 23, 1, 23, 1, 16871, 1, 1511, 1, 23, 1, 23, 1, 121319, 1, 73727, 1, 130943, 1, 12671, 1, 2687, 1, 771071, 1, 12671, 1, 2687, 1, 771071, 1, 12671, 1, 2687, 1, 771071, 1, 12671, 1, 2687, 1}

	//38801555418813
	// for i := start; j < 566000000; i += diffs[k] {
	// 	//for i := start; j < 100000000; i += int(math.Pow(8, 0)) {
	// 	//for i := start; j < 1000000; i += int(math.Pow(8, 12)) { // 10000000 timeout
	// 	//for i := range 100000 {
	// 	data := Data{i, 0, 0, numbers}
	// 	r, _ := process2(data)

	// 	// if jumps != jumps2 {
	// 	// 	fmt.Println(i, jumps2, len(data.numbers), len(r.numbers), data, "->", r)
	// 	// 	jumps = jumps2
	// 	// }

	// 	// fmt.Println(len(r.numbers), r.numbers, j, i)

	// 	if slices.Equal(r.numbers, numbers) {
	// 		panic(i)
	// 	}

	// 	// 383
	// 	if compareFirstN(numbers, r.numbers, 12) {
	// 		fmt.Println(j-jj, i)
	// 		jj = j
	// 	}

	// 	// if r.numbers[0] == numbers[0] && r.numbers[1] == numbers[1] && r.numbers[2] == numbers[2] && r.numbers[3] == numbers[3] {
	// 	// 	panic(i)
	// 	// }

	// 	j++
	// 	k = (k + 1) % len(diffs)
	// }

	// ------------------------------------------------------------------------------------------
	// for i := end - 1; j < 100; i-- {
	// 	data := Data{i, 0, 0, numbers}
	// 	r, _ := process2(data)

	// 	fmt.Println(r)

	// 	if compareLastN(numbers, r.numbers, 1) {
	// 		fmt.Println(j-jj, i)
	// 		jj = j
	// 	}
	// 	j++
	// }

	// ------------------------------------------------------------------------------------------
	// for i := start; j < 100000; i++ {
	// 	data := Data{i, 0, 0, numbers}
	// 	r, _ := process2(data)

	// 	if compareFirstN(numbers, r.numbers, 5) {
	// 		fmt.Println(j-jj, i)
	// 		jj = j
	// 	}
	// 	j++
	// }

	// ------------------------------------------------------------------------------------------
	// -- przechodzenie od konca
	// for i := end; i >= start; i-- {
	// 	data := Data{i, 0, 0, numbers}
	// 	r, _ := process2(data)

	// 	if slices.Equal(r.numbers, numbers) {
	// 		panic(i)
	// 	}
	// }

	// ------------------------------------------------------------------------------------------
	// -- przechodzenie przez wiekszy step aby isc szybciej
	// step := int(math.Pow(8, 10))
	// step := 1
	// // start += int(math.Pow(8, 10))
	// for i := start; i < end; i += step {
	// 	if i > start+(step*1000000) {
	// 		break
	// 	}
	// 	data := Data{i, 0, 0, numbers}
	// 	r, _ := process2(data)

	// 	// fmt.Println(len(r.numbers), r.numbers, i)

	// 	if r.numbers[5] == numbers[5] {
	// 		fmt.Println(j-jj, i)
	// 		jj = j
	// 	}
	// 	j++
	// }

	// start := 35184387387069
	// diffs := []int{1504520, 256, 261888, 256, 199160, 66824, 256, 63992, 1966080, 66824, 256, 63992}

	// data := Data{117440, 0, 0, []int{0, 3, 5, 4, 3, 0}}
	// r := process2(data)
	// fmt.Println(r)
	// assert.Equal(t, "11048", r)
}

func compareFirstN[T comparable](items1, items2 []T, n int) bool {
	if len(items1) != len(items2) {
		return false
	}

	return slices.Equal(items1[0:n], items2[0:n])
}

func compareLastN[T comparable](items1, items2 []T, n int) bool {
	if len(items1) != len(items2) {
		return false
	}

	j := len(items1) - n
	for i := len(items1) - 1; i >= j; i-- {
		if items1[i] != items2[i] {
			return false
		}
	}
	return true
}

func TestCompareFirstN(t *testing.T) {
	assert.Equal(t, true, compareFirstN([]int{5, 10, 15}, []int{5, 10, 15}, 3))
	assert.Equal(t, false, compareFirstN([]int{5, 10, 15}, []int{5, 10, 150}, 3))
	assert.Equal(t, true, compareFirstN([]int{5, 10, 15}, []int{5, 10, 150}, 2))
	assert.Equal(t, false, compareFirstN([]int{5, 10, 15}, []int{5, 100, 150}, 2))
	assert.Equal(t, true, compareFirstN([]int{5, 10, 15}, []int{5, 10, 150}, 1))
	assert.Equal(t, false, compareFirstN([]int{5, 10, 15}, []int{50, 100, 150}, 1))
	assert.Equal(t, false, compareFirstN([]int{5, 10}, []int{5, 10, 15}, 3))
}

func TestCompareLastN(t *testing.T) {
	assert.Equal(t, true, compareLastN([]int{5, 10, 15}, []int{5, 10, 15}, 3))
	assert.Equal(t, false, compareLastN([]int{5, 10, 15}, []int{50, 10, 15}, 3))
	assert.Equal(t, true, compareLastN([]int{5, 10, 15}, []int{50, 10, 15}, 2))
	assert.Equal(t, false, compareLastN([]int{5, 10, 15}, []int{50, 100, 15}, 2))
	assert.Equal(t, true, compareLastN([]int{5, 10, 15}, []int{50, 100, 15}, 1))
	assert.Equal(t, false, compareLastN([]int{5, 10, 15}, []int{50, 100, 150}, 1))
	assert.Equal(t, false, compareLastN([]int{5, 10}, []int{5, 10, 15}, 3))
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

// if r.numbers[0] == numbers[0] && r.numbers[1] == numbers[1] && r.numbers[2] == numbers[2] && r.numbers[3] == numbers[3] && r.numbers[4] == numbers[4] && r.numbers[5] == numbers[5] && r.numbers[6] == numbers[6] && r.numbers[7] == numbers[7] && r.numbers[8] == numbers[8] && r.numbers[9] == numbers[9] && r.numbers[10] == numbers[10] && r.numbers[11] == numbers[11] {

// 	fmt.Println(j-jj, i)
// 	jj = j
// }
