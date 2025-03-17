package day16_maze

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
	size  int
	start Point
	end   Point
}

type Dir int

const (
	DirH Dir = iota
	DirV
)

type Tile struct {
	point Point
	dir   Dir
}

func loadData(input string) Data {
	lines := utils.ParseLinesOfRunes(input)
	linelen := len(lines[0]) + 1 // \n
	findChar := func(char rune) Point {
		index := strings.IndexRune(input, char)
		return Point{x: index % linelen, y: index / linelen}
	}
	return Data{lines, len(lines[0]), findChar('S'), findChar('E')}
}

func buildGraph(data Data) []utils.Edge[Tile] {
	edges := make([]utils.Edge[Tile], 0)
	visited := make(map[Point]struct{}) // set
	sizem1 := data.size - 1
	sizem2 := data.size - 2

	addEdgesIfPossible := func(p1 Point, dir1 Dir, p2 Point, dir2 Dir, weight int) bool {
		if data.board[p1.y][p1.x] != '#' && data.board[p2.y][p2.x] != '#' {
			edges = append(edges,
				utils.Edge[Tile]{From: Tile{p1, dir1}, To: Tile{p2, dir2}, Weight: weight},
				utils.Edge[Tile]{From: Tile{p2, dir2}, To: Tile{p1, dir1}, Weight: weight})
			return true
		}
		return false
	}

	// walk vertically row by row adding edges in both directions
	for y := 1; y < sizem1; y++ {
		for x := 1; x < sizem2; x++ {
			p1 := Point{x, y}
			p2 := Point{x + 1, y}
			if addEdgesIfPossible(p1, DirH, p2, DirH, 1) {
				if _, ok := visited[p1]; !ok { // the same point is used in two adjacent edges
					visited[p1] = struct{}{}
				}
				visited[p2] = struct{}{}
			}
		}
	}

	// walk horizontally column by column adding edges in both directions
	for x := 1; x < sizem1; x++ {
		for y := 1; y < sizem2; y++ {
			p1 := Point{x, y}
			p2 := Point{x, y + 1}
			if addEdgesIfPossible(p1, DirV, p2, DirV, 1) {
				for _, p := range []Point{p1, p2} {
					if _, ok := visited[p]; ok { // crossing point
						addEdgesIfPossible(p, DirV, p, DirH, 1000)
						delete(visited, p) // the same point is used in two adjacent edges
					}
				}
			}
		}
	}

	// init direction of staring point is always horizontal, add manually such a starting point if it's not
	// included in a graph (when both sides of starting point are walls)
	startingRow := data.board[data.start.y]
	if startingRow[data.start.x-1] == '#' && startingRow[data.start.x+1] == '#' {
		addEdgesIfPossible(data.start, DirH, data.start, DirV, 1000)
	}

	return edges
}

func Puzzle1(input string) string {
	data := loadData(input)
	graph := buildGraph(data)
	pathLength := utils.DijkstraShortestPathFunc(graph, Tile{data.start, DirH}, func(v utils.Visited[Tile]) bool {
		return v.Node.point == data.end
	})
	return fmt.Sprint(pathLength)
}

func Puzzle2(input string) string {
	data := loadData(input)
	graph := buildGraph(data)

	// find all possible shortest paths
	endTiles := make([]utils.Visited[Tile], 0)
	utils.DijkstraShortestPathFunc(graph, Tile{data.start, DirH}, func(v utils.Visited[Tile]) bool {
		if v.Node.point == data.end {
			endTiles = append(endTiles, v)
			return false
		}
		return len(endTiles) > 0 // stop at first node after end nodes are visited
	})

	visitedTiles := utils.GetAllVisited(endTiles)

	// the same point can be used by many tiles (tile=point+dir), find distinct points
	visitedPoints := make(map[Point]struct{})
	for t := range visitedTiles {
		visitedPoints[t.point] = struct{}{}
	}

	return fmt.Sprint(len(visitedPoints))
}
