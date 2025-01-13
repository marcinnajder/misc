package day06_routes

import (
	"aoc/utils"
	"fmt"
	"strings"
)

type Direction int

const (
	DirsT Direction = iota // clockwise
	DirsR
	DirsB
	DirsL
)

type Point utils.Tuple2[int, int]

type Data struct {
	lines [][]rune
	start Point
}

type Turn struct {
	x   int
	y   int
	dir Direction
}

func loadData(input string) Data {
	lines := utils.ParseLinesOfRunes(input)
	linelen := len(lines[0]) + 1 // \n
	index := strings.IndexRune(input, '^')
	y := index / linelen
	x := index % linelen
	return Data{lines, Point{x, y}}
}

func move(x int, y int, dir Direction) (xx int, yy int) {
	switch dir {
	case DirsT:
		return x, y - 1
	case DirsB:
		return x, y + 1
	case DirsR:
		return x + 1, y
	case DirsL:
		return x - 1, y
	}
	panic(fmt.Sprintf("unknown direction: %v", dir))
}

func walk(data Data, obstacle Point) (outOrCycle bool, visited map[Point]struct{}) {
	size := len(data.lines[0])
	visitedPoints := make(map[Point]struct{}) // set
	visitedTurns := make(map[Turn]struct{})   // set
	x, y := data.start.Item1, data.start.Item2
	dir := DirsT

	for {
		visitedPoints[Point{x, y}] = struct{}{}
		xx, yy := move(x, y, dir)

		if xx < 0 || yy < 0 || xx >= size || yy >= size {
			return true, visitedPoints // out of bounds
		}
		if (obstacle.Item1 == xx && obstacle.Item2 == yy) || data.lines[yy][xx] == '#' {
			turn := Turn{x: xx, y: yy, dir: dir}
			if _, ok := visitedTurns[turn]; ok {
				return false, visitedPoints // cycle
			} else {
				visitedTurns[turn] = struct{}{}
			}
			dir = Direction((int(dir) + 1) % 4)
		} else {
			x, y = xx, yy
		}
	}
}

func Puzzle1(input string) string {
	data := loadData(input)
	_, visited := walk(data, Point{-1, -1})
	return fmt.Sprint(len(visited))
}

// the simplest possible implementation (without: caching previous routes, stepping with ranges insted of points, ...)
func Puzzle2(input string) string {
	data := loadData(input)
	_, visited := walk(data, Point{-1, -1})

	delete(visited, data.start) // remove starting postion

	sum := 0
	for obstacle := range visited { // check only visted
		outOrCycle, _ := walk(data, obstacle)
		if !outOrCycle {
			sum++
		}
	}

	return fmt.Sprint(sum)
}
