package day12_regions

import (
	"aoc/utils"
	"slices"
)

type Dir int

const (
	DirR Dir = 1
	DirL Dir = DirR * 2
	DirB Dir = 4
	DirT Dir = DirB * 2
)

type Data struct {
	lines []string
	size  int
}

func loadData(input string) Data {
	lines := utils.ParseLines(input)
	size := len(lines)
	return Data{lines, size}
}

type Block struct {
	id    int
	sides []Dir
}

type IdPair struct {
	id1 int
	id2 int
}

type BBlock struct {
	block Block
	r     int
	c     int
}

func puzzle(input string) string {
	data := loadData(input)
	size := data.size
	lines := data.lines

	ids := make([][]Block, size)
	for i := range size {
		ids[i] = make([]Block, size)
	}

	id := 1
	ids[0][0].id = id

	pairs := make(map[IdPair]struct{}) // set

	check := func(r, c, rn, cn int, dir Dir) {
		if lines[r][c] == lines[rn][cn] { // the same
			if ids[rn][cn].id == 0 { // not visited
				ids[rn][cn].id = ids[r][c].id
			} else if ids[rn][cn].id != ids[r][c].id {
				id1 := ids[r][c].id
				id2 := ids[rn][cn].id
				pairs[IdPair{min(id1, id2), max(id1, id2)}] = struct{}{}
			}
		} else {
			if ids[rn][cn].id == 0 { // not visited
				id++
				ids[rn][cn].id = id
			} else {
				if ids[rn][cn].id == ids[r][c].id {
					panic("nie powinienem tutaj byc :) ")
				}
			}

			ids[r][c].sides = append(ids[r][c].sides, dir)
			ids[rn][cn].sides = append(ids[rn][cn].sides, dir*2)
		}
	}

	for r := range size {
		for c := range size {
			if c+1 < size { // right
				check(r, c, r, c+1, DirR)
			}
			if r+1 < size { // bottom
				check(r, c, r+1, c, DirB)
			}

			if r == 0 {
				ids[r][c].sides = append(ids[r][c].sides, DirT)
			}
			if r == size-1 {
				ids[r][c].sides = append(ids[r][c].sides, DirB)
			}
			if c == 0 {
				ids[r][c].sides = append(ids[r][c].sides, DirL)
			}
			if c == size-1 {
				ids[r][c].sides = append(ids[r][c].sides, DirR)
			}

		}
	}

	mappings := make(map[int]int)
	mid := -1
	for pair := range pairs {
		m1, ok1 := mappings[pair.id1]
		m2, ok2 := mappings[pair.id2]

		if !ok1 && !ok2 {
			mid--
			mappings[pair.id1] = mid
			mappings[pair.id2] = mid
		} else if ok1 && ok2 {
			if m1 != m2 {
				for k, v := range mappings {
					if v == m2 {
						mappings[k] = m1
					}
				}
			}
		} else if ok1 {
			mappings[pair.id2] = m1
		} else {
			mappings[pair.id1] = m2
		}
	}

	final := make([][]int, size)

	countsById := make(map[int]int)
	sidesById := make(map[int]int)
	regions := make(map[int][]BBlock)

	for r, row := range ids {
		final[r] = make([]int, size)
		for c, block := range row {
			gid := block.id
			if m, ok := mappings[block.id]; ok {
				gid = m
			}

			countsById[gid]++
			sidesById[gid] += len(block.sides)

			final[r][c] = gid

			regions[gid] = append(regions[gid], BBlock{block, r, c})
		}
	}

	// fmt.Println(final)

	sum := 0
	for k, v := range countsById {
		sum += v * sidesById[k]
	}

	sum2 := 0
	for _, blocks := range regions {
		ss := 0
		for _, b := range blocks {
			ss += len(b.block.sides)
		}
		sum2 += len(blocks) * ss
	}

	sum3 := 0
	for _, blocks := range regions {
		hej := make(map[Dir]map[int][]int)
		for _, d := range []Dir{DirT, DirB, DirL, DirR} {
			hej[d] = make(map[int][]int)
		}

		//ss := 0
		for _, b := range blocks {
			//ss += len(b.block.sides)
			for _, s := range b.block.sides {
				dmap := hej[s]
				if s == DirB || s == DirT {
					dmap[b.r] = append(dmap[b.r], b.c)
				} else {
					dmap[b.c] = append(dmap[b.c], b.r)
				}
			}
		}

		sss := 0
		for _, mmm := range hej {
			for _, items := range mmm {
				sss += countSides(items)
			}
		}

		sum3 += len(blocks) * sss

	}

	return ""
}

func countSides(items []int) int {
	slices.Sort(items)

	sum := 1
	for i := range len(items) - 1 {
		if items[i]+1 != items[i+1] {
			sum++
		}
	}

	return sum

}

// if c+1 < size { // right
// 	if lines[r][c] == lines[r][c+1] { // the same
// 		if ids[r][c+1].id == 0 { // not visited
// 			ids[r][c+1].id = ids[r][c].id
// 		} else if ids[r][c+1].id != ids[r][c].id {
// 			pairs = append(pairs, []int{ids[r][c+1].id, ids[r][c].id})
// 		}
// 	} else {
// 		if ids[r][c+1].id == 0 { // not visited
// 			id++
// 			ids[r][c+1].id = id
// 		} else {
// 			panic("nie powinienem tutaj byc :) ")
// 		}

// 		ids[r][c].size++
// 		ids[r][c+1].size++
// 	}
// }

// func loadData(input string) []int {
// 	return utils.ParseIntsWithFields(input)
// }

// func transform(val int) []int {
// 	if val == 0 {
// 		return []int{1}
// 	}

// 	text := strconv.Itoa(val)
// 	if len(text)%2 == 0 {
// 		middle := len(text) / 2
// 		v1, _ := strconv.Atoi(text[0:middle])
// 		v2, _ := strconv.Atoi(text[middle:])
// 		return []int{v1, v2}
// 	}

// 	return []int{val * 2024}
// }

// func Puzzle(input string, blinks int) string {
// 	numbers := loadData(input)

// 	his := make(map[int]int)
// 	for _, n := range numbers {
// 		his[n]++
// 	}

// 	for range blinks {
// 		hisnext := make(map[int]int)
// 		for n, o := range his {
// 			for _, nn := range transform(n) {
// 				hisnext[nn] += o
// 			}
// 		}
// 		his = hisnext
// 	}

// 	sum := 0
// 	for _, o := range his {
// 		sum += o
// 	}
// 	return fmt.Sprint(sum)
// }

// func Puzzle1(input string) string {
// 	return Puzzle(input, 25)
// }

// func Puzzle2(input string) string {
// 	return Puzzle(input, 75)
// }

/// ------------------

// func puzzle(input string) string {
// 	data := loadData(input)
// 	size := data.size
// 	lines := data.lines

// 	ids := make([][]Block, size)
// 	for i := range size {
// 		ids[i] = make([]Block, size)
// 	}

// 	id := 1
// 	ids[0][0].id = id

// 	pairs := make(map[IdPair]struct{}) // set

// 	check := func(r, c, rn, cn int) {
// 		if lines[r][c] == lines[rn][cn] { // the same
// 			if ids[rn][cn].id == 0 { // not visited
// 				ids[rn][cn].id = ids[r][c].id
// 			} else if ids[rn][cn].id != ids[r][c].id {
// 				id1 := ids[r][c].id
// 				id2 := ids[rn][cn].id
// 				pairs[IdPair{min(id1, id2), max(id1, id2)}] = struct{}{}
// 			}
// 		} else {
// 			if ids[rn][cn].id == 0 { // not visited
// 				id++
// 				ids[rn][cn].id = id
// 			} else {
// 				if ids[rn][cn].id == ids[r][c].id {
// 					panic("nie powinienem tutaj byc :) ")
// 				}
// 			}

// 			ids[r][c].sides++
// 			ids[rn][cn].sides++
// 		}
// 	}

// 	for r := range size {
// 		for c := range size {
// 			if c+1 < size { // right
// 				check(r, c, r, c+1)
// 			}
// 			if r+1 < size { // bottom
// 				check(r, c, r+1, c)
// 			}

// 			if r == 0 || r == size-1 {
// 				ids[r][c].sides++
// 			}
// 			if c == 0 || c == size-1 {
// 				ids[r][c].sides++
// 			}
// 		}
// 	}

// 	fmt.Println(pairs)

// 	for _, row := range ids {
// 		fmt.Println(row)
// 	}

// 	mappings := make(map[int]int)
// 	mid := -1
// 	for pair := range pairs {
// 		m1, ok1 := mappings[pair.id1]
// 		m2, ok2 := mappings[pair.id2]

// 		if !ok1 && !ok2 {
// 			mid--
// 			mappings[pair.id1] = mid
// 			mappings[pair.id2] = mid
// 		} else if ok1 && ok2 {
// 			if m1 != m2 {
// 				for k, v := range mappings {
// 					if v == m2 {
// 						mappings[k] = m1
// 					}
// 				}
// 			}
// 		} else if ok1 {
// 			mappings[pair.id2] = m1
// 		} else {
// 			mappings[pair.id1] = m2
// 		}

// 		// if m, ok := mappings[pair.id1]; ok {
// 		// 	mappings[pair.id2] = m
// 		// } else if m, ok := mappings[pair.id2]; ok {
// 		// 	mappings[pair.id1] = m
// 		// } else {
// 		// 	mappings[pair.id1] = mid
// 		// 	mappings[pair.id2] = mid
// 		// 	mid--
// 		// }
// 	}

// 	final := make([][]int, size)
// 	countsById := make(map[int]int)
// 	sidesById := make(map[int]int)
// 	for r, row := range ids {
// 		final[r] = make([]int, size)
// 		for c, block := range row {
// 			gid := block.id
// 			if m, ok := mappings[block.id]; ok {
// 				gid = m
// 			}
// 			countsById[gid]++
// 			sidesById[gid] += block.sides
// 			final[r][c] = gid
// 		}
// 	}

// 	fmt.Println(final)

// 	sum := 0
// 	for k, v := range countsById {
// 		sum += v * sidesById[k]
// 	}

// 	return ""
// }
