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

func lastOccupiedIndex(blocks []Block, index int) int {
	for i := index; i >= 0; i-- {
		if isOccupied(blocks[i]) {
			return i
		}
	}
	return -1
}

func compact1(blocks []Block) []Block {
	res := make([]Block, 0)
	i := 0
	j := lastOccupiedIndex(blocks, len(blocks)-1)

	// index is like a pointer to the item inside collection
	// modify content of item only via index like "coll[i].foo=1"
	// local variable like "el:=call[i]; el.foo=1" copies the structure

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

func compact2(blocks []Block) []Block {
	// 'blocks' parameter is mutated
	moved := make(map[int]struct{})
	j := len(blocks)

	for {
		// find next occupied from the right
		j = lastOccupiedIndex(blocks, j-1)
		if j == -1 { // stop condition
			return blocks
		}

		mb := blocks[j]

		if _, ok := moved[mb.id]; ok {
			continue // already moved, find next
		}

		// find first free space going from the left
		i := -1
		for k, b := range blocks {
			if k >= j {
				break
			}
			if !isOccupied(b) && b.size >= mb.size {
				i = k
				break
			}
		}

		if i != -1 {
			diffsize := blocks[i].size - mb.size
			if diffsize > 0 { // split free space
				blocks = slices.Insert(blocks, i+1, Block{-1, diffsize}) // insert new free space ...
				j += 1                                                   // ... increment index
			}

			blocks[i].id = mb.id
			blocks[i].size = mb.size
			moved[mb.id] = struct{}{}
			blocks[j].id = -1 // mutate!
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
