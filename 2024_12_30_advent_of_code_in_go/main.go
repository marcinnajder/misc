package main

import (
	"aoc/aoc2024/day01_distances"
	"aoc/aoc2024/day02_reports"
	"aoc/aoc2024/day03_instructions"
	"aoc/aoc2024/day04_xmas"
	"aoc/aoc2024/day05_ordering"
	"aoc/aoc2024/day06_routes"
	"aoc/aoc2024/day07_equations"
	"aoc/aoc2024/day08_antennas"
	"aoc/aoc2024/day09_compacting"
	"fmt"
	"os"
	"time"
)

// - "go run ." - odpalenie 'main'
// - "go test ./..." - odpalenie wszystkich unit testow
// - "gore" - odpalenie repl, potem ":import "strings"" aby zaimportowac pakiet
// - "alt+shift+enter" - wykonanie komendy 'test function at cursor' ktora uruchamia test w ktorym jestesmy

func main() {
	executePuzzle("aoc2024/day01_distances", day01_distances.Puzzle1, day01_distances.Puzzle2)
	executePuzzle("aoc2024/day02_reports", day02_reports.Puzzle1, day02_reports.Puzzle2)
	executePuzzle("aoc2024/day03_instructions", day03_instructions.Puzzle1, day03_instructions.Puzzle2)
	executePuzzle("aoc2024/day04_xmas", day04_xmas.Puzzle1, day04_xmas.Puzzle2)
	executePuzzle("aoc2024/day05_ordering", day05_ordering.Puzzle1, day05_ordering.Puzzle2)
	executePuzzle("aoc2024/day06_routes", day06_routes.Puzzle1, day06_routes.Puzzle2)
	executePuzzle("aoc2024/day07_equations", day07_equations.Puzzle1, day07_equations.Puzzle2)
	executePuzzle("aoc2024/day08_antennas", day08_antennas.Puzzle1, day08_antennas.Puzzle2)
	executePuzzle("aoc2024/day09_compacting", day09_compacting.Puzzle1, day09_compacting.Puzzle2_)
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
