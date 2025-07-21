package day01_mass

import (
	"aoc/utils"
)

func loadData(input string) []int {
	return utils.ParseIntsWithFields(input)
}

// func loadDataExpanded(input string) Data {
// 	grid := utils.ParseLinesOfIntsGrid(input)
// 	size := len(grid)

// 	newSize := 5 * size
// 	newGrid := make([][]int, newSize)

// 	for i := range newSize {
// 		newGrid[i] = make([]int, newSize)
// 		for j := range newSize {
// 			val := (grid[i%size][j%size] + (i / size) + (j / size)) % 9
// 			newGrid[i][j] = utils.If(val == 0, 9, val)
// 		}
// 	}

// 	graph := utils.BuildGraph(newGrid, newSize)
// 	return Data{graph, newSize}
// }

func Puzzle1(input string) string {
	return "input"
}

func Puzzle2(input string) string {
	return "input"
}

// func Puzzle2(input string) string {
// 	return Puzzle(loadDataExpanded(input))
// }

// // func expandData(data Data) Data {

// // 	newData := make([][])
// // 	return data
// // }

// // func Puzzle2(input string) string {
// // 	return Puzzle(input)
// // }
