package day20_cheating

import (
	"aoc/utils"
	"iter"
	"strings"
)

type Point struct {
	x int
	y int
}

type Data struct {
	board [][]rune
	size  int
	start Point
	end   Point
}

// copied from 'day16_maze'
func loadData(input string) Data {
	lines := utils.ParseLinesOfRunes(input)
	linelen := len(lines[0]) + 1 // \n
	findChar := func(char rune) Point {
		index := strings.IndexRune(input, char)
		return Point{x: index % linelen, y: index / linelen}
	}
	return Data{lines, len(lines[0]), findChar('S'), findChar('E')}
}

var allMoves = []Point{{0, 1}, {0, -1}, {1, 0}, {-1, 0}}

func movePoint(point Point, move Point) Point {
	return Point{point.x + move.x, point.y + move.y}
}

func buildRouteMap(data Data) map[Point]int {
	nextMoveMap := make(map[Point][]Point)
	var currentMove Point

	// build 'nextMoveMap' and set start move
	for _, move := range allMoves {
		nextMoveMap[move] = []Point{move, {move.y, move.x}, {move.y * -1, move.x * -1}} // the same move or turns
		if p := movePoint(data.start, move); data.board[p.y][p.x] != '#' {
			currentMove = move
		}
	}

	// walk the path
	currentPoint := data.start
	steps := 1
	result := map[Point]int{currentPoint: steps}

	for {
		for _, move := range nextMoveMap[currentMove] {
			if p := movePoint(currentPoint, move); data.board[p.y][p.x] != '#' {
				currentMove = move
				currentPoint = p
				break
			}
		}

		steps++
		result[currentPoint] = steps

		if currentPoint == data.end {
			break
		}
	}

	return result
}

func Puzzle1(input string) string {
	data := loadData(input)

	sizem1 := data.size - 1
	routeMap := buildRouteMap(data)

	cheatsByPoints := make(map[Point]int)
	cheatsHistogram := make(map[int]int)

	for y := 1; y < sizem1; y++ {
		for x := 1; x < sizem1; x++ {
			if data.board[y][x] == '#' {

				if steps1, ok1 := routeMap[Point{x - 1, y}]; ok1 {
					if steps2, ok2 := routeMap[Point{x + 1, y}]; ok2 {
						saved := utils.Abs(steps1, steps2) - 2
						cheatsByPoints[Point{x, y}] = saved
						// fmt.Println(Point{x, y}, saved)
						cheatsHistogram[saved] = cheatsHistogram[saved] + 1
						continue
					}
				}

				if steps1, ok1 := routeMap[Point{x, y + 1}]; ok1 {
					if steps2, ok2 := routeMap[Point{x, y - 1}]; ok2 {
						saved := utils.Abs(steps1, steps2) - 2
						cheatsByPoints[Point{x, y}] = saved
						// fmt.Println(Point{x, y}, saved)
						cheatsHistogram[saved] = cheatsHistogram[saved] + 1
					}
				}
			}
		}
	}

	// fmt.Println(cheatsHistogram)
	// fmt.Println(cheatsByPoints)

	sum := 0
	for saved, cheats := range cheatsHistogram {
		if saved >= 100 {
			sum += cheats
		}
	}

	// fmt.Println(sum)

	// a := cheatsByPoints[Point{8, 1}]  // 12
	// b := cheatsByPoints[Point{10, 7}] // 20
	// c := cheatsByPoints[Point{8, 8}]  // 38
	// d := cheatsByPoints[Point{6, 7}]  // 64

	// fmt.Println(a, b, c, d)

	return ""
}

type Visited map[Point]map[Point]int

type Stack[T any] []T

func (stack Stack[T]) Push(item T) Stack[T] {
	newStack := append(stack, item)
	return newStack
}

func getNeighbours(data Data, point Point) iter.Seq[Point] {
	return func(yield func(Point) bool) {
		for _, m := range allMoves {
			if (point.x == 0 && m.x == -1) || (point.y == 0 && m.y == -1) ||
				(point.x == data.size-1 && m.x == 1) || (point.y == data.size-1 && m.y == 1) {
				continue // move of bounds, skip it
			}

			if !yield(movePoint(point, m)) {
				return
			}
		}
	}
}

func getNeighboursRocks(data Data, point Point) iter.Seq[Point] {
	return func(yield func(Point) bool) {
		for n := range getNeighbours(data, point) {
			if data.board[n.y][n.x] == '#' {
				if !yield(movePoint(point, n)) {
					return
				}
			}
		}
	}
}

func (stack Stack[T]) Pop() (Stack[T], T, bool) {
	length := len(stack)
	if length > 0 {
		val := stack[length-1]
		newStack := stack[0 : length-1]
		return newStack, val, true
	}
	var val T
	return stack, val, false
}

func wasMoveExecuted(visited Visited, from, to Point) bool {
	if m1, ok1 := visited[from]; ok1 {
		_, ok2 := m1[to]
		return ok2
	}
	return false
}

func registerPoint(visited Visited, point Point, maxCheats int) {

	// neighbours :=
	// if m1, ok1 := visited[from]; ok1 {
	// 	_, ok2 := m1[to]
	// 	return ok2
	// }
	// return false
}

func Puzzle2(input string) {
	data := loadData(input)

	visited := make(Visited)
	todo := Stack[Point]{Point{0, 0}}

	for len(todo) > 0 {

		todo, point, _ := todo.Pop()

		for n := range getNeighboursRocks(data, point) {

			if !wasMoveExecuted(visited, point, n) && !wasMoveExecuted(visited, n, point) {
				todo = todo.Push(n)
			}

		}

	}

}
