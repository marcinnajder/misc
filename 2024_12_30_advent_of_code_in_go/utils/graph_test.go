package utils

import (
	"testing"

	"github.com/stretchr/testify/assert"
)

// https://adventofcode.com/2021/day/15
var input = `1163751742
1381373672
2136511328
3694931569
7463417111
1319128137
1359912421
3125421639
1293138521
2311944581`

func TestDijkstraShortestPath(t *testing.T) {
	lines := ParseLinesOfIntsGrid(input)
	graph := BuildGraph(lines, len(lines))
	r := DijkstraShortestPath(graph, Point{0, 0}, Point{9, 9})
	//fmt.Println(r)
	assert.Equal(t, 40, r)
}
