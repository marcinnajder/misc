package day09_compacting

import (
	"aoc/utils"
	"fmt"
	"iter"
)

type Point struct {
	x int
	y int
}

type Data struct {
	lines    [][]rune
	size     int
	antennas map[rune][]Point
}

func loadData(input string) Data {
	lines := utils.ParseLinesOfRunes(input)
	size := len(lines)
	antennas := make(map[rune][]Point)
	for y, line := range lines {
		for x, a := range line {
			if a != '.' {
				antennas[a] = append(antennas[a], Point{x, y})
			}
		}
	}
	return Data{lines, size, antennas}
}

func lineAntinodes(p1, p2 Point, size, nfirst int) iter.Seq[Point] {
	return func(yield func(Point) bool) {
		dx := p1.x - p2.x
		dy := p1.y - p2.y
		for p, dir := range map[Point]int{p1: 1, p2: -1} { // for each of two directions
			for i := 1; i <= nfirst; i++ {
				x := p.x + dx*dir*i
				y := p.y + dy*dir*i
				if x >= 0 && y >= 0 && x < size && y < size && !yield(Point{x, y}) {
					return
				}
			}
		}
	}
}

func allAntinodes(data Data, nfirst int) map[Point]struct{} {
	points := make(map[Point]struct{})
	for _, as := range data.antennas {
		for i := 0; i < len(as)-1; i++ {
			for j := i + 1; j < len(as); j++ {
				for p := range lineAntinodes(as[i], as[j], data.size, nfirst) {
					points[p] = struct{}{}
				}
			}
		}
	}
	return points
}

func Puzzle1(input string) string {
	data := loadData(input)
	points := allAntinodes(data, 1)
	return fmt.Sprint(len(points))
}

func Puzzle2(input string) string {
	data := loadData(input)
	points := allAntinodes(data, data.size)

	// include antennas
	for _, as := range data.antennas {
		for _, a := range as {
			points[a] = struct{}{}
		}
	}
	return fmt.Sprint(len(points))
}
