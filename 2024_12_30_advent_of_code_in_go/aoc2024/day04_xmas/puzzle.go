package day04_xmas

import (
	"aoc/utils"
	"fmt"
)

type Direction int

const (
	DirsT Direction = iota
	DirsTR
	DirsR
	DirsBR
	DirsB
	DirsBL
	DirsL
	DirsTL
)

func loadData(input string) [][]rune {
	return utils.ParseLinesOfRunes(input)
}

// singletons
var dirsAll = []Direction{DirsT, DirsTR, DirsR, DirsBR, DirsB, DirsBL, DirsL, DirsTL}
var dirsWitoutTop = []Direction{DirsR, DirsBR, DirsB, DirsBL, DirsL}
var dirsWitoutTopAndLeft = []Direction{DirsR, DirsBR, DirsB}
var dirsWitoutTopAndRight = []Direction{DirsB, DirsBL, DirsL}
var dirsWitoutBottom = []Direction{DirsT, DirsTR, DirsR, DirsL, DirsTL}
var dirsWitoutBottomAndLeft = []Direction{DirsT, DirsTR, DirsR}
var dirsWitoutBottomAndRight = []Direction{DirsT, DirsL, DirsTL}
var dirsWitoutLeft = []Direction{DirsT, DirsTR, DirsR, DirsBR, DirsB}
var dirsWitoutRight = []Direction{DirsT, DirsB, DirsBL, DirsL, DirsTL}

func availableDirections(x, y, wordlen, boardsize int) []Direction {
	steps := wordlen - 1
	var topOk, bottomOk, leftOk, rightOk = y-steps >= 0, y+steps < boardsize, x-steps >= 0, x+steps < boardsize

	if !topOk {
		if !leftOk {
			return dirsWitoutTopAndLeft
		}
		if !rightOk {
			return dirsWitoutTopAndRight
		}
		return dirsWitoutTop
	}
	if !bottomOk {
		if !leftOk {
			return dirsWitoutBottomAndLeft
		}
		if !rightOk {
			return dirsWitoutBottomAndRight
		}
		return dirsWitoutBottom
	}

	if !leftOk {
		return dirsWitoutLeft
	}
	if !rightOk {
		return dirsWitoutRight
	}

	return dirsAll
}

func move(x, y, steps int, dir Direction) (xx int, yy int) {
	switch dir {
	case DirsT:
		return x, y - steps
	case DirsTL:
		return x - steps, y - steps
	case DirsTR:
		return x + steps, y - steps
	case DirsB:
		return x, y + steps
	case DirsBL:
		return x - steps, y + steps
	case DirsBR:
		return x + steps, y + steps
	case DirsL:
		return x - steps, y
	case DirsR:
		return x + steps, y
	}
	panic(fmt.Sprintf("unknown direction: %v", dir))
}

type Occurrence struct {
	X, Y       int
	Directions []Direction
}

func Puzzle(input, word string, counter func([]Occurrence) int) string {
	wordrunes := []rune(word)
	wordlen := len(wordrunes)
	lines := loadData(input)
	boardSize := len(lines)
	occurrences := make([]Occurrence, 0)

	for y, line := range lines {
		for x, char := range line {
			if char == wordrunes[0] {
				dirs := availableDirections(x, y, wordlen, boardSize)
				step := 1
				for ; step < wordlen; step++ {
					dirsNext := make([]Direction, 0)
					for _, dir := range dirs {
						xx, yy := move(x, y, step, dir)
						if lines[yy][xx] == wordrunes[step] {
							dirsNext = append(dirsNext, dir)
						}
					}
					dirs = dirsNext
					if len(dirs) == 0 { // no directions available
						break
					}
				}

				if len(dirs) > 0 && step == wordlen {
					occurrences = append(occurrences, Occurrence{x, y, dirs})
				}
			}
		}
	}

	count := counter(occurrences)
	return fmt.Sprint(count)
}

func Puzzle1(input string) string {
	return Puzzle(input, "XMAS", func(occurrences []Occurrence) int {
		count := 0
		for _, o := range occurrences {
			count += len(o.Directions)
		}
		return count
	})
}

func Puzzle2(input string) string {
	word := "MAS"
	halfsteps := (len(word) - 1) / 2

	return Puzzle(input, word, func(occurrences []Occurrence) int {
		count := 0
		middles := make(map[string]struct{}) // set
		for _, o := range occurrences {
			for _, dir := range o.Directions {
				switch dir {
				case DirsTL, DirsTR, DirsBL, DirsBR: // diagonal only
					x, y := move(o.X, o.Y, halfsteps, dir)
					middle := fmt.Sprintf("%d-%d", x, y)
					if _, ok := middles[middle]; ok {
						count++
					} else {
						middles[middle] = struct{}{}
					}
				}
			}
		}
		return count
	})
}
