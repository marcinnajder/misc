package day18_path

import (
	"aoc/utils"
	"fmt"
)

func loadData(input string) []utils.Point {
	lines := utils.ParseLines(input)
	corrupted := make([]utils.Point, 0)
	for _, line := range lines {
		coors := utils.ParseIntsWithSplit(line, ",")
		corrupted = append(corrupted, utils.Point{X: coors[0], Y: coors[1]})
	}
	return corrupted
}

func buildGraph(corrupted []utils.Point, boardSize int) []utils.Edge[utils.Point] {
	corruptedMap := make(map[utils.Point]struct{}) // set
	for _, p := range corrupted {
		corruptedMap[p] = struct{}{}
	}

	row := make([]int, boardSize) // 'singleton' row
	for i := range boardSize {
		row[i] = 1 // weight of the edge
	}

	grid := make([][]int, boardSize)
	for i := range boardSize {
		grid[i] = row // sharing the same row in memory
	}

	graph := make([]utils.Edge[utils.Point], 0)
	for edge := range utils.BuildGraphSeq(grid, boardSize) {
		if _, ok1 := corruptedMap[edge.From]; !ok1 {
			if _, ok2 := corruptedMap[edge.To]; !ok2 {
				graph = append(graph, edge)
			}
		}
	}

	return graph
}

func puzzle1Impl(input string, boardSize int, nFirst int) string {
	start := utils.Point{X: 0, Y: 0}
	end := utils.Point{X: boardSize - 1, Y: boardSize - 1}
	corrupted := loadData(input)

	graph := buildGraph(corrupted[0:nFirst], boardSize)
	pathLen := utils.DijkstraShortestPath(graph, start, end)

	return fmt.Sprint(pathLen)
}

func Puzzle1(input string) string {
	return puzzle1Impl(input, 71, 1024)
}

func canTraverseGraph[T comparable](graph []utils.Edge[T], start, end T) bool {
	for visited := range utils.DijkstraTraverse(graph, start) {
		if visited.Node == end {
			return true
		}
	}
	return false
}

func puzzle2Impl(input string, boardSize int, nFirst int) string {
	corrupted := loadData(input)

	start := utils.Point{X: 0, Y: 0}
	end := utils.Point{X: boardSize - 1, Y: boardSize - 1}

	from := nFirst
	to := len(corrupted) - 1
	var middle int

	for {
		middle = (to + from) / 2 // "binary search"

		if middle == to { // from == to
			break
		}

		graph := buildGraph(corrupted[0:middle+1], boardSize)
		canTraverse := canTraverseGraph(graph, start, end)

		if middle == from { // from + 1 == to
			if canTraverse {
				middle = to
			}
			break
		}

		// next iteration
		if canTraverse {
			from = middle
		} else {
			to = middle
		}
	}

	point := corrupted[middle]
	return fmt.Sprintf("%d,%d", point.X, point.Y)
}

func Puzzle2(input string) string {
	return puzzle2Impl(input, 71, 1024)
}
