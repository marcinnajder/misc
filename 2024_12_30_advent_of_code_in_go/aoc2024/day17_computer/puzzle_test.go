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

// for i := range 117440 + 1 {
// 	data := Data{i, 0, 0, []int{0, 3, 5, 4, 3, 0}}
// 	r := process2(data)
// 	fmt.Println(data, "->", r)
// }

// 11111
// 5-4=1

// 121212
// 5-3=2

// 00111111
// 00121212

// len=11
// lastI=10
// i=7

// 00 123 123 123

// 0012312341231234

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

			// benc := make(map[T]struct{})

			// for _, p := range pattern {
			// 	benc[p] = struct{}{}
			// }

			// if len(benc) == 1 {
			// 	continue main // :)
			// }

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

func iterateI() iter.Seq2[int, int] {
	return func(yield func(int, int) bool) {
		for i := 0; i < math.MaxInt64; i++ {
			if !yield(-1, 1) {
				return
			}
		}
	}
}
func iterateI2() iter.Seq[[]int] {
	one := []int{1}
	return func(yield func([]int) bool) {
		for i := 0; i < math.MaxInt64; i++ {
			if !yield(one) {
				return
			}
		}
	}
}

func iterateI3() Seq {
	one := []int{1}
	return func(yield func([]int) (int, bool)) {
		for i := 0; i < math.MaxInt64; i++ {
			if _, ok := yield(one); !ok {
				return
			}
		}
	}
}

func iterateDiffs(diffs []int) iter.Seq2[int, int] {
	return func(yield func(int, int) bool) {
		for {
			for _, diff := range diffs {
				if !yield(-1, diff) {
					return
				}
			}
		}
	}
}
func iterateDiffs2(diffs, pattern []int) iter.Seq2[int, int] {
	return func(yield func(int, int) bool) {

		for _, diff := range diffs {
			if !yield(-1, diff) {
				return
			}
		}

		for {
			for _, diff := range pattern {
				if !yield(-1, diff) {
					return
				}
			}
		}
	}
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

type Last4 struct {
	val1, val2, val3, val4 int
}

type Seq func(yield func([]int) (int, bool))

func iterateDiffs3(diffs []int) Seq {

	lastmap := make(map[Last4][]int)

	return func(yield func([]int) (int, bool)) {
		last := Last4{0, 0, 0, 0}
		for _, diff := range diffs {

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

		for {
			if diff, ok := lastmap[last]; !ok {
				panic("niby nie wiadomo jaka powinna byc nastepna  ")
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

func TestPuzzle4(t *testing.T) {
	numbers := []int{2, 4, 1, 1, 7, 5, 1, 5, 4, 0, 0, 3, 5, 5, 3, 0}
	// numbers := []int{2, 4, 1, 1, 7, 5, 1, 5, 0, 3, 4, 3, 5, 5, 3, 0}

	start := int(math.Pow(8, 15))
	// end := int(math.Pow(8, 16))
	nFirst := 2
	diffsseq := iterateI3()

	for {
		i := start
		iprev := i

		diffs := make([]int, 0)
		ivalues := make([]int, 0)

		diffsseq(func(diff []int) (int, bool) {

			var diffNext int

			if len(diff) == 1 {
				diffNext = diff[0]
			} else {

				diffNextList := make([]int, 0)
				for _, d := range diff {
					r, _ := process2(Data{i + d, 0, 0, numbers})
					if compareFirstN(numbers, r.numbers, nFirst) {
						diffNextList = append(diffNextList, d)
					}
				}

				if oj := len(diffNextList); oj != 1 {

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

					_ = diffNextList2

					diffNext = diffNextList2[0]
				} else {
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
					fmt.Println(i, "tutaj", r)
					panic("koniec")
				}

				diff := i - iprev
				iprev = i

				diffs = append(diffs, diff)
				ivalues = append(ivalues, i)

				if len(diffs) == 1000 {
					start = ivalues[0] - diffs[0]
					diffsseq = iterateDiffs3(diffs)
					nFirst++ // odkomentowac !!

					return diffNext, false
				}
			}

			return diffNext, true

		})

		// for diff := range diffsseq {
		// 	i += diff
		// 	data := Data{i, 0, 0, numbers}
		// 	r, _ := process2(data)

		// 	if compareFirstN(numbers, r.numbers, nFirst) {
		// 		if compareFirstN(numbers, r.numbers, 16) {
		// 			fmt.Println(i, "tutaj", r)
		// 			panic("koniec")
		// 		}

		// 		diff := i - iprev
		// 		iprev = i

		// 		diffs = append(diffs, diff)
		// 		ivalues = append(ivalues, i)

		// 		if len(diffs) == 1000 {
		// 			start = ivalues[0] - diffs[0]
		// 			diffsseq = iterateDiffs3(diffs)
		// 			nFirst++ // odkomentowac !!

		// 			continue loop
		// 		}
		// 	}

		// }
	}
}

func TestPuzzle3(t *testing.T) {
	// numbers := []int{2, 4, 1, 1, 7, 5, 1, 5, 4, 0, 0, 3, 5, 5, 3, 0}
	// numbers := []int{2, 4, 1, 1, 7, 5, 1, 5, 0, 3, 4, 3, 5, 5, 3, 0}
	numbers := []int{2, 4, 1, 3, 7, 5, 1, 5, 0, 3, 4, 3, 5, 5, 3, 0}
	reps := 2

	// rrr, _ := process2(Data{56256477, 0, 0, numbers})
	// _ = rrr

	//
	start := int(math.Pow(8, 15))
	end := int(math.Pow(8, 16))
	j := 0
	jj := 0
	k := 0

	_ = j
	// ------------------------------------------------------------------------------------------

	nFirst := 2

	// steps
	// j++
	// k = (k + 1) % len(diffs)
	// j = 0

	// i := start
	diffsseq := iterateI()

loop:
	for {
		i := start
		iprev := i

		diffs := make([]int, 0)
		ivalues := make([]int, 0)

		// fmt.Println("START", start)

		for _, diff := range diffsseq {
			i += diff

			if i > end {
				panic("koniec :(((( ")
			}

			data := Data{i, 0, 0, numbers}
			r, _ := process2(data)

			if compareFirstN(numbers, r.numbers, nFirst) {
				diff := i - iprev
				iprev = i

				// fmt.Println(diff, i, r.numbers)

				diffs = append(diffs, diff)
				ivalues = append(ivalues, i)

				if compareFirstN(numbers, r.numbers, 16) {
					fmt.Println(i, "tutaj", r)
					panic("koniec")
				}

				if patternLen := findPatten(diffs, reps, 2); patternLen != -1 {
					pattern := diffs[len(diffs)-patternLen:]

					//fmt.Println(nFirst, patternLen, pattern, r)

					fmt.Println()
					fmt.Println("nFirst", nFirst)
					fmt.Println("DIFFS", len(diffs), diffs)
					fmt.Println("PATTERN", len(pattern), pattern)

					// if nFirst == 10 {
					// 	fmt.Println("DIFFS")
					// 	fmt.Println(diffs)
					// 	fmt.Println("PATTERN")
					// 	fmt.Println(pattern)
					// }

					// fmt.Println()
					// fmt.Println(nFirst)
					// fmt.Println(patternLen, " - ", pattern)
					// fmt.Println(len(diffs), " - ", diffs)

					// start = ivalues[len(diffs)-reps*patternLen] - diffs[len(diffs)-reps*patternLen]
					start = ivalues[0] - diffs[0]

					diffsseq = iterateDiffs2(diffs, pattern)
					nFirst++ // odkomentowac !!

					// fmt.Println("DIFF", diffs[0], ivalues[0])
					// fmt.Println("len(diffs)", len(diffs), "start", start, "len(pattern)", len(pattern))

					continue loop
				}
			}
		}
	}

	_ = nFirst
	_ = end
	_ = start
	_ = k
	_ = jj
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
