package day06_routes

import (
	"aoc/utils"
	"fmt"
	"iter"
	"maps"
	"strings"
)

type Dir int

const (
	DirT Dir = iota // clockwise values
	DirR
	DirB
	DirL
)

type Point struct {
	x int
	y int
}

type Move struct {
	point Point
	dir   Dir
}

type Data struct {
	lines [][]rune
	start Point
}

func loadData(input string) Data {
	lines := utils.ParseLinesOfRunes(input)
	linelen := len(lines[0]) + 1 // \n
	index := strings.IndexRune(input, '^')
	y := index / linelen
	x := index % linelen
	return Data{lines, Point{x, y}}
}

func movePoint(p Point, dir Dir) Point {
	switch dir {
	case DirT:
		return Point{p.x, p.y - 1}
	case DirB:
		return Point{p.x, p.y + 1}
	case DirR:
		return Point{p.x + 1, p.y}
	case DirL:
		return Point{p.x - 1, p.y}
	}
	panic(fmt.Sprintf("unknown direction: %v", dir))
}

func walk(data Data, obstacle Point) (outOrCycle bool, visited map[Point]struct{}) {
	size := len(data.lines[0])
	visitedPoints := make(map[Point]struct{}) // set
	visitedTurns := make(map[Move]struct{})   // set
	point := data.start
	dir := DirT

	for {
		visitedPoints[point] = struct{}{}

		nextPoint := movePoint(point, dir)
		if nextPoint.x < 0 || nextPoint.y < 0 || nextPoint.x >= size || nextPoint.y >= size {
			return true, visitedPoints // out of bounds
		}

		if (obstacle.x == nextPoint.x && obstacle.y == nextPoint.y) || data.lines[nextPoint.y][nextPoint.x] == '#' {
			move := Move{point: nextPoint, dir: dir}
			if _, ok := visitedTurns[move]; ok {
				return false, visitedPoints // cycle
			} else {
				visitedTurns[move] = struct{}{}
			}
			dir = Dir((int(dir) + 1) % 4) // turn right
		} else {
			point = nextPoint
		}
	}
}

func Puzzle1(input string) string {
	data := loadData(input)
	_, visited := walk(data, Point{-1, -1}) // no obstacles
	return fmt.Sprint(len(visited))
}

// the simplest possible implementation (without: caching previous routes, stepping with ranges instead of points, ...)
func Puzzle2(input string) string {
	data := loadData(input)
	_, visited := walk(data, Point{-1, -1})

	delete(visited, data.start) // remove starting postion

	sum := 0
	for obstacle := range visited { // only check for visited
		outOrCycle, _ := walk(data, obstacle)
		if !outOrCycle {
			sum++
		}
	}

	return fmt.Sprint(sum)
}

// ***** optimized implementation
// puzzle2  ~600ms
// puzzle2_ ~30ms or ~80ms (depending on the cache of the route)

// 'walk' function creates always 2 maps, 'walkSeq' function just iterates without additional collections
func walkSeq(data Data, obstacle Point, prev Move) iter.Seq[Move] {
	return func(yield func(move Move) bool) {
		size := len(data.lines[0])
		point := prev.point
		dir := prev.dir

		for {
			nextPoint := movePoint(point, dir)
			if nextPoint.x < 0 || nextPoint.y < 0 || nextPoint.x >= size || nextPoint.y >= size {
				return // out of bounds
			}
			if (obstacle.x == nextPoint.x && obstacle.y == nextPoint.y) || data.lines[nextPoint.y][nextPoint.x] == '#' {
				dir = Dir((int(dir) + 1) % 4) // turn right
			} else {
				point = nextPoint
				if !yield(Move{point, dir}) {
					return
				}
			}
		}
	}
}

func walkUntilOutOrCycle(data Data, obstacle Point, prev Move, visitedTurns map[Move]struct{}) (outOrCycle bool) {
	prevMove := prev

	for move := range walkSeq(data, obstacle, prev) {
		if prevMove.dir != move.dir { // turns are much less than moves, searching in smaller map is much faster :)
			if _, ok := visitedTurns[move]; ok {
				return false // cycle
			}
			visitedTurns[move] = struct{}{}
			prevMove = move
		}
	}

	return true // out of bounds
}

func Puzzle1_(input string) string {
	data := loadData(input)
	visited := make(map[Point]struct{}) // set
	visited[data.start] = struct{}{}    // include starting point
	for move := range walkSeq(data, Point{-1, -1}, Move{data.start, DirT}) {
		visited[move.point] = struct{}{}
	}
	return fmt.Sprint(len(visited))
}

func Puzzle2_(input string) string {
	data := loadData(input)
	visitedPoints := make(map[Point]struct{}) // set
	visitedTurns := make(map[Move]struct{})   // set

	visitedPoints[data.start] = struct{}{}
	prevMove := Move{data.start, DirT}
	sum := 0

	for move := range walkSeq(data, Point{-1, -1}, prevMove) {
		if _, ok := visitedPoints[move.point]; ok { // do not check the obstacle at same position twice
			continue
		}
		visitedPoints[move.point] = struct{}{}

		// ~30 ms for puzzle2_ (walk from the last visited)
		outOrCycle := walkUntilOutOrCycle(data, move.point, prevMove, maps.Clone(visitedTurns) /*clone map !!*/)

		// ~80 ms for puzzle2_ (walk from the start)
		// outOrCycle := walkUntilOutOrCycle(data, move.point, Move{data.start, DirsT}, make(map[Move]struct{}))

		if !outOrCycle {
			sum++
		}

		if prevMove.dir != move.dir {
			visitedTurns[move] = struct{}{}
		}
		prevMove = move
	}

	return fmt.Sprint(sum)
}
