package day19_towels

import (
	"aoc/utils"
	"fmt"
	"strings"
)

type Text []rune

type Data struct {
	towels  []string
	designs []string
}

func loadData(input string) Data {
	lines := utils.ParseLines(input)
	towels := strings.Split(lines[0], ", ")
	designs := make([]string, len(lines)-2)
	for i := 0; i < len(designs); i++ {
		designs[i] = lines[i+2]
	}
	return Data{towels, designs}
}

func toTowelsByLength(towels []string) map[int][]string {
	result := make(map[int][]string)
	for _, t := range towels {
		result[len(t)] = append(result[len(t)], t)
	}
	return result
}

func isDesignPossibe(design string, towelsByLen map[int][]string, cache map[string]bool) bool {
	if val, ok := cache[design]; ok {
		return val
	}

	for towelLen, towels := range towelsByLen {
		if towelLen <= len(design) {
			for _, towel := range towels {
				if strings.HasPrefix(design, towel) {
					if towelLen == len(design) || isDesignPossibe(design[towelLen:], towelsByLen, cache) {
						cache[design] = true
						return true
					}
				}
			}
		}
	}

	cache[design] = false
	return false
}

func Puzzle1(input string) string {
	data := loadData(input)
	towelsByLen := toTowelsByLength(data.towels)
	cache := make(map[string]bool)
	count := 0
	for _, design := range data.designs {
		if isDesignPossibe(design, towelsByLen, cache) {
			count++
		}
	}
	return fmt.Sprint(count)
}

func countPossibleDesigns(design string, towelsByLen map[int][]string, cache map[string]int) int {
	if val, ok := cache[design]; ok {
		return val
	}

	sum := 0
	for towelLen, towels := range towelsByLen {
		if towelLen <= len(design) {
			for _, towel := range towels {
				if strings.HasPrefix(design, towel) {
					if towelLen == len(design) {
						sum += 1
					} else {
						sum += countPossibleDesigns(design[towelLen:], towelsByLen, cache)
					}
				}
			}
		}
	}

	cache[design] = sum
	return sum
}

func Puzzle2(input string) string {
	data := loadData(input)
	towelsByLen := toTowelsByLength(data.towels)
	cache := make(map[string]int)
	sum := 0
	for _, design := range data.designs {
		sum += countPossibleDesigns(design, towelsByLen, cache)
	}
	return fmt.Sprint(sum)
}
