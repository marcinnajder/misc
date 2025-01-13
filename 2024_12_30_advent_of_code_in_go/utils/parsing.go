package utils

import (
	"strconv"
	"strings"
)

func parseInts(fields []string) []int {
	numbers := make([]int, len(fields))
	for i, field := range fields {
		if number, err := strconv.Atoi(field); err != nil {
			panic(err)
		} else {
			numbers[i] = number
		}
	}
	return numbers
}

func ParseIntsWithFields(text string) []int {
	return parseInts(strings.Fields(text))
}

func ParseIntsWithSplit(text string, sep string) []int {
	return parseInts(strings.Split(text, sep))
}

func ParseLines(text string) []string {
	return strings.Split(text, "\n")
}

func ParseLinesOfRunes(input string) [][]rune {
	lines := ParseLines(input)
	linesRunes := make([][]rune, len(lines))
	for i, line := range lines {
		linesRunes[i] = []rune(line)
	}
	return linesRunes
}

func ParseLinesOfInts(input string) [][]int {
	lines := ParseLines(input)
	linesNumbers := make([][]int, len(lines))
	for i, line := range lines {
		numbers := ParseIntsWithFields(line)
		linesNumbers[i] = numbers
	}
	return linesNumbers
}
