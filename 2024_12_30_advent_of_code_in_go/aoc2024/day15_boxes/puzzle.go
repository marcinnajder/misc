package day15_boxes

import (
	"aoc/utils"
	"fmt"
	"strings"
)

type Point struct {
	x int
	y int
}
type Data struct {
	board [][]rune
	sizex int
	sizey int
	start Point
	moves []rune
}

func loadData(input string) Data {
	lines := utils.ParseLinesOfRunes(input)
	board := make([][]rune, 0)
	moves := make([]rune, 0)

	i := 0
	for line := lines[i]; len(line) != 0; i, line = i+1, lines[i+1] {
		board = append(board, line)
	}

	sizey := i
	sizex := i
	i++

	for ; i < len(lines); i++ {
		moves = append(moves, lines[i]...)
	}

	lineLen := sizey + 1
	index := strings.IndexRune(input, '@')
	return Data{board, sizey, sizex, Point{index % lineLen, index / lineLen}, moves}
}

var dirDiffMap = map[rune]Point{'^': {0, -1}, 'v': {0, 1}, '<': {-1, 0}, '>': {1, 0}}

func nextPoint(dirDiff Point, p Point) Point {
	return Point{p.x + dirDiff.x, p.y + dirDiff.y}
}

func nextFreePoint(data Data, dirDiff Point, point Point) (pp Point, ok bool) {
	p := point
	for {
		next := nextPoint(dirDiff, p)
		nextVal := data.board[next.y][next.x]
		if nextVal == '#' {
			return Point{}, false
		}
		if nextVal == '.' {
			return next, true
		}
		p = next
	}
}

func swapPoints(data Data, p1, p2 Point) {
	temp := data.board[p1.y][p1.x]
	data.board[p1.y][p1.x] = data.board[p2.y][p2.x]
	data.board[p2.y][p2.x] = temp
}

func calcResult(data Data) int {
	sum := 0
	for y := 0; y < data.sizey; y++ {
		for x := 0; x < data.sizex; x++ {
			if data.board[y][x] == 'O' || data.board[y][x] == '[' {
				sum += 100*y + x
			}
		}
	}
	return sum
}

func Puzzle1(input string) string {
	data := loadData(input)
	current := data.start

	for _, dir := range data.moves {
		dirDiff := dirDiffMap[dir]
		free, ok := nextFreePoint(data, dirDiff, current)

		if ok {
			next := nextPoint(dirDiff, current)

			if next != free {
				swapPoints(data, free, next)
			}

			swapPoints(data, next, current)
			current = next
		}
	}

	sum := calcResult(data)
	return fmt.Sprint(sum)
}

func expandBoard(data Data) Data {
	newBoard := make([][]rune, data.sizey)
	for y := 0; y < data.sizey; y++ {
		newBoard[y] = make([]rune, data.sizey*2)
		for x := 0; x < data.sizey; x++ {
			char := data.board[y][x]
			switch char {
			case 'O':
				newBoard[y][x*2] = '['
				newBoard[y][x*2+1] = ']'
			case '@':
				newBoard[y][x*2] = '@'
				newBoard[y][x*2+1] = '.'
			default: // '#', '.'
				newBoard[y][x*2] = char
				newBoard[y][x*2+1] = char
			}
		}
	}
	newStart := Point{data.start.x * 2, data.start.y}
	return Data{newBoard, data.sizex * 2, data.sizey, newStart, data.moves}
}

func findPointsMovedVertically(data Data, dir Point, point Point) []Point {
	points := []Point{point}
	visited := make(map[Point]struct{})

	for i := 0; i < len(points); i++ {
		next := nextPoint(dir, points[i])

		if _, ok := visited[next]; ok {
			continue
		}

		nextVal := data.board[next.y][next.x]
		if nextVal == '#' {
			return nil
		}

		if nextVal != '.' { // '[' or ']'
			visited[next] = struct{}{}
			points = append(points, next)

			next2 := nextPoint(dirDiffMap[utils.If(nextVal == ']', '<', '>')], next)
			visited[next2] = struct{}{}
			points = append(points, next2)
		}
	}

	return points
}

func movePoint(data Data, dir Point, p Point) {
	next := nextPoint(dir, p)
	data.board[next.y][next.x] = data.board[p.y][p.x]
	data.board[p.y][p.x] = '.'
}

func Puzzle2(input string) string {
	data := loadData(input)
	data = expandBoard(data) // override 'data'
	current := data.start

	for _, dir := range data.moves {
		if dir == '^' || dir == 'v' {
			points := findPointsMovedVertically(data, dirDiffMap[dir], current)
			if points != nil {
				for i := len(points) - 1; i >= 0; i-- {
					movePoint(data, dirDiffMap[dir], points[i])
				}
				current = nextPoint(dirDiffMap[dir], current)
			}
		} else {
			free, ok := nextFreePoint(data, dirDiffMap[dir], current)
			if ok {
				if dir == '<' {
					for i := free.x + 1; i <= current.x; i++ {
						movePoint(data, dirDiffMap[dir], Point{i, current.y})
					}
				} else { // '>'
					for i := free.x - 1; i >= current.x; i-- {
						movePoint(data, dirDiffMap[dir], Point{i, current.y})
					}
				}
				current = nextPoint(dirDiffMap[dir], current)
			}
		}
	}

	sum := calcResult(data)
	return fmt.Sprint(sum)
}
