package day10_trails

import (
	"aoc/utils"
	"fmt"
	"iter"
)

type Point struct {
	x int
	y int
}

func (p Point) String() string {
	return fmt.Sprintf("%d,%d", p.x, p.y)
}

type Data struct {
	board  [][]int
	size   int
	starts []Point
}

func loadData(input string) Data {
	lines := utils.ParseLinesOfRunes(input)
	size := len(lines)
	starts := make([]Point, 0)
	board := make([][]int, size)
	for y, line := range lines {
		row := make([]int, size)
		board[y] = row
		for x, r := range line {
			d := utils.RuneToDigit(r)
			row[x] = d
			if d == 0 {
				starts = append(starts, Point{x, y})
			}
		}
	}
	return Data{board, size, starts}
}

func neighbours(data Data, p Point, step int) iter.Seq[Point] {
	return func(yield func(Point) bool) {
		if p.x > 0 && data.board[p.y][p.x-1] == step && !yield(Point{p.x - 1, p.y}) {
			return
		}
		if p.y > 0 && data.board[p.y-1][p.x] == step && !yield(Point{p.x, p.y - 1}) {
			return
		}
		if p.x < data.size-1 && data.board[p.y][p.x+1] == step && !yield(Point{p.x + 1, p.y}) {
			return
		}
		if p.y < data.size-1 && data.board[p.y+1][p.x] == step && !yield(Point{p.x, p.y + 1}) {
			return
		}
	}
}

type Set[T comparable] map[T]struct{}

type Path string

// walk through all trails at once
func Puzzle(input string, joinpath func(path Path, p Point) Path) string {
	data := loadData(input)
	pointsByStep := make([]map[Point]Set[Path], 10) //  Map<Point, Set<Path>> []

	pointsByStep[0] = make(map[Point]Set[Path])
	for _, p := range data.starts {
		pointsByStep[0][p] = Set[Path]{Path(p.String()): struct{}{}}
	}

	for step := 1; step <= 9; step++ {
		points := make(map[Point]Set[Path])
		for p, ppaths := range pointsByStep[step-1] {
			for n := range neighbours(data, p, step) {
				npaths, ok := points[n]
				if !ok {
					npaths = make(Set[Path])
					points[n] = npaths
				}
				for path := range ppaths {
					npaths[joinpath(path, n)] = struct{}{}
				}
			}
		}
		pointsByStep[step] = points
	}

	sum := 0
	for _, ppaths := range pointsByStep[9] {
		sum += len(ppaths)
	}
	return fmt.Sprint(sum)
}

func Puzzle1(input string) string {
	return Puzzle(input, func(path Path, p Point) Path {
		return path
	})
}

func Puzzle2(input string) string {
	return Puzzle(input, func(path Path, p Point) Path {
		return Path(fmt.Sprintf("%s-%v", path, p))
	})
}
