package main

import (
	"aoc/aoc2024/day01_distances"
	"aoc/aoc2024/day02_reports"
	"aoc/aoc2024/day03_instructions"
	"aoc/aoc2024/day04_xmas"
	"fmt"
	"os"
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
}

func executePuzzle(puzzlePath string, puzzle1 func(string) string, puzzle2 func(string) string) {
	inputBytes, err := os.ReadFile(fmt.Sprintf("./%s/data.txt", puzzlePath))
	if err != nil {
		panic(err)
	}

	inputString := string(inputBytes)
	result1 := puzzle1(inputString)
	fmt.Printf("%s puzzle1: %s \n", puzzlePath, result1)
	result2 := puzzle2(inputString)
	fmt.Printf("%s puzzle2: %s \n", puzzlePath, result2)
}
