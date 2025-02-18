package day15_graph

import (
	"aoc/utils"
	"fmt"
)

type Data struct {
	graph []utils.Edge[utils.Point]
	size  int
}

func loadData(input string) Data {
	grid := utils.ParseLinesOfIntsGrid(input)
	size := len(grid)
	graph := utils.BuildGraph(grid, size)
	return Data{graph, size}
}

func loadDataExpanded(input string) Data {
	grid := utils.ParseLinesOfIntsGrid(input)
	size := len(grid)

	newSize := 5 * size
	newGrid := make([][]int, newSize)

	for i := range newSize {
		newGrid[i] = make([]int, newSize)
		for j := range newSize {
			val := (grid[i%size][j%size] + (i / size) + (j / size)) % 9
			newGrid[i][j] = utils.If(val == 0, 9, val)
		}
	}

	graph := utils.BuildGraph(newGrid, newSize)
	return Data{graph, newSize}
}

func Puzzle(data Data) string {
	path := utils.DijkstraShortestPath(data.graph, utils.Point{X: 0, Y: 0}, utils.Point{X: data.size - 1, Y: data.size - 1})
	return fmt.Sprint(path)
}

func Puzzle1(input string) string {
	return Puzzle(loadData(input))
}
func Puzzle2(input string) string {
	return Puzzle(loadDataExpanded(input))
}

// func expandData(data Data) Data {

// 	newData := make([][])
// 	return data
// }

// func Puzzle2(input string) string {
// 	return Puzzle(input)
// }
