package day09_compacting

import (
	"aoc/utils"
	"fmt"
	"slices"
)

type Block struct {
	id   int
	size int
}

func loadData(input string) []Block {
	blocks := make([]Block, len(input))
	for i, s := range input {
		size := utils.RuneToDigit(s)
		id := utils.If(i%2 == 0, i/2, -1) // -1 for free block
		blocks[i] = Block{id, size}
	}
	return blocks
}

func isOccupied(block Block) bool {
	return block.id != -1
}

func lastOccupied(blocks []Block, begin, end int) int {
	for i := end; i >= begin; i-- {
		if isOccupied(blocks[i]) {
			return i
		}
	}
	return -1
}

func compact1(blocks []Block) []Block {
	// - 'blocks' parameter is mutated
	// - index is like a pointer to the item inside collection
	// modify content of item only via index like "coll[i].foo=1"
	// local variable like "el:=call[i]; el.foo=1" copies the structure !!
	res := make([]Block, 0)
	i := 0
	j := lastOccupied(blocks, 0, len(blocks)-1)

	for {
		if i+1 >= j { // stop condition
			res = append(res, blocks[j]) // copy block
			return res
		}

		if isOccupied(blocks[i]) { // occupied block, return and move to next
			res = append(res, blocks[i]) // copy block
			i++
			continue
		}

		if blocks[j].size >= blocks[i].size { // enough space
			res = append(res, Block{blocks[j].id, blocks[i].size}) // new block
			blocks[j].size = blocks[j].size - blocks[i].size       // mutate!

			// update indexes at the end, just before next iteration (indexes are used above)
			i++
			if blocks[j].size == 0 { // no space left, move back two steps
				j -= 2
			}
		} else {
			res = append(res, Block{blocks[j].id, blocks[j].size}) // new block
			blocks[i].size = blocks[i].size - blocks[j].size       // mutate!

			// update indexes at the end, just before next iteration (indexes are used above)
			j -= 2
		}
	}
}

func checksum(blocks []Block) int {
	sum, i := 0, 0
	for _, b := range blocks {
		if isOccupied(b) {
			for range b.size {
				sum += i * b.id
				i++
			}
		} else {
			i += b.size
		}
	}
	return sum
}

func firstFreeBigEnough(blocks []Block, start, end, size int) int {
	for i := start; i < end; i++ {
		b := blocks[i]
		if !isOccupied(b) && b.size >= size {
			return i
		}
	}
	return -1
}

func compact2(blocks []Block) []Block {
	// - 'blocks' parameter is mutated
	moved := make(map[int]struct{})
	j := len(blocks)

	for {
		j = lastOccupied(blocks, 0, j-1)
		if j == -1 { // stop condition
			return blocks
		}

		mb := blocks[j]

		if _, ok := moved[mb.id]; ok {
			continue // already moved, skip it
		}

		i := firstFreeBigEnough(blocks, 0, j, mb.size)

		if i != -1 {
			diffsize := blocks[i].size - mb.size
			if diffsize > 0 { // split free space
				blocks = slices.Insert(blocks, i+1, Block{-1, diffsize}) // insert free block in the middle ...
				j += 1                                                   // ... and increment index afterwards
			}

			blocks[i].id = mb.id
			blocks[i].size = mb.size
			moved[mb.id] = struct{}{}
			blocks[j].id = -1 // mutate!, zero block
		}
	}
}

func Puzzle(input string, compact func([]Block) []Block) string {
	blocks := loadData(input)
	compacted := compact(blocks)
	sum := checksum(compacted)
	return fmt.Sprint(sum)
}

func Puzzle1(input string) string {
	return Puzzle(input, compact1)
}

func Puzzle2(input string) string {
	return Puzzle(input, compact2)
}

// compact2_ - optimized implementation of compact2 (10ms vs 30 ms)
// - "inserted := make(map[int][]Block)" - avoiding callings of 'slices.Insert' which inserts items in the middle of slice
// - "k := 0" - avoiding searching always from the beginning of blocks -> this optimiation makes all of the work :)

func merge(blocks []Block, inserted map[int][]Block) []Block {
	intertedlen := 0
	for _, ins := range inserted {
		intertedlen += len(ins)
	}

	res := make([]Block, len(blocks)+intertedlen)
	for i, ib := 0, 0; i < len(res); i, ib = i+1, ib+1 {
		if ins, ok := inserted[ib]; ok {
			for k, b := range ins {
				res[i+k] = b
			}
			i += len(ins)
			res[i] = blocks[ib]
		} else {
			res[i] = blocks[ib]
		}
	}
	return res
}

func compact2_(blocks []Block) []Block {
	// - 'blocks' parameter is mutated
	moved := make(map[int]struct{})
	inserted := make(map[int][]Block)
	j := len(blocks)
	k := 0 // index of first free block

	for {
		j = lastOccupied(blocks, k, j-1)
		if j == -1 { // stop condition
			return merge(blocks, inserted)
		}

		mb := blocks[j]

		if _, ok := moved[mb.id]; ok {
			continue // already moved, skip it
		}

		i := firstFreeBigEnough(blocks, k, j, mb.size)
		if i != -1 {
			diffsize := blocks[i].size - mb.size
			if diffsize > 0 { // split free space
				inserted[i] = append(inserted[i], Block{mb.id, mb.size})
				blocks[i].size = diffsize
			} else { // overrides full empty space
				blocks[i].id = mb.id
				blocks[i].size = mb.size
				kk := firstFreeBigEnough(blocks, k, j, 1)
				if kk != -1 {
					k = kk
				}
			}

			moved[mb.id] = struct{}{}
			blocks[j].id = -1 // mutate!, zero block
		}
	}
}

func Puzzle2_(input string) string {
	return Puzzle(input, compact2_)
}
