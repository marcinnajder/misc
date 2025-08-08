package main

import (
	"aoc/aoc2019/day01_mass"
	"aoc/aoc2019/day02_computer"
	"aoc/aoc2019/day03_snakes"
	"aoc/aoc2019/day04_passwords"
	"aoc/aoc2021/day01_increases"
	"aoc/aoc2021/day15_graph"
	"aoc/aoc2024/day01_distances"
	"aoc/aoc2024/day02_reports"
	"aoc/aoc2024/day03_instructions"
	"aoc/aoc2024/day04_xmas"
	"aoc/aoc2024/day05_ordering"
	"aoc/aoc2024/day06_routes"
	"aoc/aoc2024/day07_equations"
	"aoc/aoc2024/day08_antennas"
	"aoc/aoc2024/day09_compacting"
	"aoc/aoc2024/day10_trails"
	"aoc/aoc2024/day11_blinks"
	"aoc/aoc2024/day12_regions"
	"aoc/aoc2024/day13_combinations"
	"aoc/aoc2024/day14_robots"
	"aoc/aoc2024/day15_boxes"
	"aoc/aoc2024/day16_maze"
	"aoc/aoc2024/day18_path"
	"aoc/aoc2024/day19_towels"

	"fmt"
	"os"
	"time"
)

// - run/test app 'with' vs 'without' POWERSEQ
// -- 2 kinds of solutions in files with first line commented (puzzle_powerseq.go, puzzle_powerseq.go)
// -- by default app is running 'without' powerseq
// -- to run/test 'with' powerseq
// --- configure vs code changing 'settings.json' file in 2 places ("go.testTags",  "gopls")
// --- to run/test from command line including '-tags powerseq'

// - "go run ." - odpalenie 'main'
// go run .
// go run -tags powerseq .

//"go test ./..." - odpalenie wszystkich unit testow
// go test -v ./...
// go test -tags powerseq -v ./...
// go test -tags powerseq ./...
// go test -tags powerseq -v ./aoc2024/day01_distances
// go test -v ./aoc2024/day01_distances

// - "gore" - odpalenie repl, potem ":import "strings"" aby zaimportowac pakiet

// - "alt+shift+enter" - wykonanie komendy 'test function at cursor' ktora uruchamia test w ktorym jestesmy

func main() {
	executePuzzle("aoc2019/day01_mass", day01_mass.Puzzle1, day01_mass.Puzzle2)
	executePuzzle("aoc2019/day02_computer", day02_computer.Puzzle1, day02_computer.Puzzle2)
	executePuzzle("aoc2019/day03_snakes", day03_snakes.Puzzle1, day03_snakes.Puzzle2)
	executePuzzle("aoc2019/day04_passwords", day04_passwords.Puzzle1_, day04_passwords.Puzzle2_)
	executePuzzle("aoc2021/day01_increases", day01_increases.Puzzle1_, day01_increases.Puzzle2_)
	executePuzzle("aoc2021/day15_graph", day15_graph.Puzzle1, day15_graph.Puzzle2)
	executePuzzle("aoc2024/day01_distances", day01_distances.Puzzle1, day01_distances.Puzzle2)
	executePuzzle("aoc2024/day02_reports", day02_reports.Puzzle1, day02_reports.Puzzle2)
	executePuzzle("aoc2024/day03_instructions", day03_instructions.Puzzle1, day03_instructions.Puzzle2)
	executePuzzle("aoc2024/day04_xmas", day04_xmas.Puzzle1, day04_xmas.Puzzle2)
	executePuzzle("aoc2024/day05_ordering", day05_ordering.Puzzle1, day05_ordering.Puzzle2)
	executePuzzle("aoc2024/day06_routes", day06_routes.Puzzle1, day06_routes.Puzzle2)
	executePuzzle("aoc2024/day07_equations", day07_equations.Puzzle1, day07_equations.Puzzle2)
	executePuzzle("aoc2024/day08_antennas", day08_antennas.Puzzle1, day08_antennas.Puzzle2)
	executePuzzle("aoc2024/day09_compacting", day09_compacting.Puzzle1, day09_compacting.Puzzle2_)
	executePuzzle("aoc2024/day10_trails", day10_trails.Puzzle1, day10_trails.Puzzle2)
	executePuzzle("aoc2024/day11_blinks", day11_blinks.Puzzle1, day11_blinks.Puzzle2)
	executePuzzle("aoc2024/day12_regions", day12_regions.Puzzle1, day12_regions.Puzzle2)
	executePuzzle("aoc2024/day13_combinations", day13_combinations.Puzzle1, day13_combinations.Puzzle2)
	executePuzzle("aoc2024/day14_robots", day14_robots.Puzzle1, day14_robots.Puzzle2)
	executePuzzle("aoc2024/day15_boxes", day15_boxes.Puzzle1, day15_boxes.Puzzle2)
	executePuzzle("aoc2024/day16_maze", day16_maze.Puzzle1, day16_maze.Puzzle2)
	executePuzzle("aoc2024/day18_path", day18_path.Puzzle1, day18_path.Puzzle2)
	executePuzzle("aoc2024/day19_towels", day19_towels.Puzzle1, day19_towels.Puzzle2)
}

func executePuzzle(puzzlePath string, puzzle1 func(string) string, puzzle2 func(string) string) {
	inputBytes, err := os.ReadFile(fmt.Sprintf("./%s/data.txt", puzzlePath))
	if err != nil {
		panic(err)
	}
	inputString := string(inputBytes)

	start := time.Now()
	result1 := puzzle1(inputString)
	fmt.Printf("%s puzzle1: %s (%v) \n", puzzlePath, result1, time.Since(start))

	start = time.Now()
	result2 := puzzle2(inputString)
	fmt.Printf("%s puzzle2: %s (%v) \n", puzzlePath, result2, time.Since(start))
}
