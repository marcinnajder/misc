package day12_regions

import (
	"aoc/utils"
	"fmt"
	"iter"
	"slices"
)

type Dirs int // flags

const (
	DirNone Dirs = 0
	DirR    Dirs = 1
	DirL    Dirs = DirR * 2 // opposites
	DirB    Dirs = 4
	DirT    Dirs = DirB * 2 // opposites
)

var allDirs = []Dirs{DirR, DirL, DirB, DirT}

func foreachDirs(dirs Dirs) iter.Seq[Dirs] {
	return func(yield func(d Dirs) bool) {
		if dirs != DirNone {
			for _, d := range allDirs {
				if utils.HasFlags(dirs, d) && !yield(d) {
					return
				}
			}
		}
	}
}

type BlockId struct {
	id   int
	dirs Dirs
}

type Pair struct {
	id1 int
	id2 int
}

type Block struct {
	r    int
	c    int
	dirs Dirs
}

func loadData(input string) []string {
	return utils.ParseLines(input)
}

func findRegions(ids [][]BlockId, joins map[Pair]struct{}) map[int][]Block {
	rid := -1
	joinsmap := make(map[int]int)

	// transform from list of id pairs into map from id to id
	for pair := range joins {
		id1, ok1 := joinsmap[pair.id1]
		id2, ok2 := joinsmap[pair.id2]

		if !ok1 && !ok2 {
			rid--
			joinsmap[pair.id1] = rid
			joinsmap[pair.id2] = rid
		} else if ok1 && ok2 {
			if id1 != id2 {
				for k, v := range joinsmap {
					if v == id2 {
						joinsmap[k] = id1
					}
				}
			}
		} else if ok1 {
			joinsmap[pair.id2] = id1
		} else {
			joinsmap[pair.id1] = id2
		}
	}

	regions := make(map[int][]Block)

	for r, row := range ids {
		for c, block := range row {
			rrid := block.id
			if id, ok := joinsmap[block.id]; ok {
				rrid = id
			}

			regions[rrid] = append(regions[rrid], Block{r, c, block.dirs})
		}
	}

	return regions
}

func Puzzle(input string, sidesCounter func(regions map[int][]Block) int) string {
	lines := loadData(input)
	size := len(lines)
	sizem1 := size - 1
	joins := make(map[Pair]struct{}) // set
	ids := make([][]BlockId, size)
	id := 1

	for i := range size {
		ids[i] = make([]BlockId, size)
	}
	ids[0][0].id = id

	checkNeighbor := func(r, c, rn, cn int, dir Dirs) {
		if lines[r][c] == lines[rn][cn] { // the same values of adjacent blocks
			if ids[rn][cn].id == 0 { // not visited yet
				ids[rn][cn].id = ids[r][c].id
			} else if ids[rn][cn].id != ids[r][c].id {
				id1 := ids[r][c].id
				id2 := ids[rn][cn].id
				joins[Pair{min(id1, id2), max(id1, id2)}] = struct{}{}
			}
		} else {
			if ids[rn][cn].id == 0 { // not visited yet
				id++
				ids[rn][cn].id = id
			}

			ids[r][c].dirs = utils.AddFlags(ids[r][c].dirs, dir)
			ids[rn][cn].dirs = utils.AddFlags(ids[rn][cn].dirs, dir*2) // opposite direction
		}
	}

	// all rows and columns without the last ones
	for r := range sizem1 {
		for c := range sizem1 {
			checkNeighbor(r, c, r, c+1, DirR) // right
			checkNeighbor(r, c, r+1, c, DirB) // bottom
		}
	}

	// last column and row
	for i := range sizem1 {
		checkNeighbor(sizem1, i, sizem1, i+1, DirR) // last row
		checkNeighbor(i, sizem1, i+1, sizem1, DirB) // last column
	}

	// add external sides
	for i := range size {
		ids[0][i].dirs = utils.AddFlags(ids[0][i].dirs, DirT)
		ids[sizem1][i].dirs = utils.AddFlags(ids[sizem1][i].dirs, DirB)
		ids[i][0].dirs = utils.AddFlags(ids[i][0].dirs, DirL)
		ids[i][sizem1].dirs = utils.AddFlags(ids[i][sizem1].dirs, DirR)
	}

	regions := findRegions(ids, joins)
	sum := sidesCounter(regions)
	return fmt.Sprint(sum)
}

func countAllSides(regions map[int][]Block) int {
	sum := 0
	for _, blocks := range regions {
		sidesCount := 0
		for _, b := range blocks {
			if b.dirs != DirNone {
				for range foreachDirs(b.dirs) {
					sidesCount++
				}
			}
		}
		sum += len(blocks) * sidesCount
	}
	return sum
}

func Puzzle1(input string) string {
	return Puzzle(input, countAllSides)
}

func countJoinedIndexes(indexes []int) int {
	if len(indexes) < 2 {
		return len(indexes)
	}

	slices.Sort(indexes)

	sum := 1
	for i := range len(indexes) - 1 {
		if indexes[i]+1 != indexes[i+1] {
			sum++
		}
	}
	return sum
}

func countJoinedSides(regions map[int][]Block) int {
	sum := 0
	for _, blocks := range regions {

		indexesByDir := make(map[Dirs]map[int][]int)
		for _, dir := range allDirs {
			indexesByDir[dir] = make(map[int][]int)
		}

		for _, b := range blocks {
			if b.dirs != DirNone {
				for s := range foreachDirs(b.dirs) {
					indexes := indexesByDir[s]
					if s == DirB || s == DirT {
						indexes[b.r] = append(indexes[b.r], b.c) // horizontal
					} else {
						indexes[b.c] = append(indexes[b.c], b.r) // vertical
					}
				}
			}
		}

		sidesCount := 0
		for _, indexesGroup := range indexesByDir {
			for _, indexes := range indexesGroup {
				sidesCount += countJoinedIndexes(indexes)
			}
		}

		sum += len(blocks) * sidesCount
	}
	return sum
}

func Puzzle2(input string) string {
	return Puzzle(input, countJoinedSides)
}
