package day03_snakes

import (
	"aoc/utils"
	"cmp"
	"iter"
	"math"
	"slices"
	"strconv"
	"strings"
)

type Move struct {
	direction string // R,L,U,D
	distance  int
}

func loadData(input string) (moves1 []Move, moves2 []Move) {
	lines := utils.ParseLines(input)
	result := make([][]Move, len(lines))

	for lineIndex, line := range lines {
		moves := strings.Split(line, ",")
		result[lineIndex] = make([]Move, len(moves))
		for moveIndex, move := range moves {
			direction := move[0:1]                // first char
			distance, _ := strconv.Atoi(move[1:]) // rest chars
			result[lineIndex][moveIndex] = Move{direction, distance}
		}
	}

	return result[0], result[1]
}

type Range struct {
	from, to int
}

type Point struct {
	x, y int
}

type Line struct {
	isHorizontal bool
	pos          int
	r            Range // order of endings corresponds to movement
	steps        int
}

func (r Range) contains(pos int) bool {
	if r.from <= r.to {
		return pos >= r.from && pos <= r.to
	}
	return pos >= r.to && pos <= r.from
}

func (line1 Line) isCrossing(line2 Line) bool {
	return line1.r.contains(line2.pos) && line2.r.contains(line1.pos)
}

func walk(moves []Move, origin Point) iter.Seq[Line] {
	return func(yield func(Line) bool) {
		var r Range
		current := origin
		steps := 0
		for _, move := range moves {
			if move.direction == "L" || move.direction == "R" {
				pos := utils.If(move.direction == "L", current.x-move.distance, current.x+move.distance)
				r = Range{current.x, pos}
				if (!yield(Line{true, current.y, r, steps})) {
					return
				}
				current = Point{pos, current.y}
			} else {
				pos := utils.If(move.direction == "D", current.y-move.distance, current.y+move.distance)
				r = Range{current.y, pos}
				if (!yield(Line{false, current.x, r, steps})) {
					return
				}
				current = Point{current.x, pos}
			}
			steps += utils.AbsDiff(r.to, r.from)
		}
	}
}

func findCrossingLines(lines []Line, line2 Line) iter.Seq[Line] {
	return func(yield func(Line) bool) {
		for _, line1 := range lines { // inefficient scanning of all lines
			if line1.isCrossing(line2) {
				if !yield(line1) {
					return
				}
			}
		}
	}
}

func findCrossingLinesOrdered(lines1 []Line, line2 Line) iter.Seq[Line] {
	return func(yield func(Line) bool) {
		// find any line crossing searched line2
		foundIndex, found := slices.BinarySearchFunc(lines1, line2, func(l1, l2 Line) int {
			if l2.r.contains(l1.pos) {
				return 0
			}
			return cmp.Compare(l1.pos, min(l2.r.from, l2.r.to))
		})

		if !found {
			return
		}

		// scan lines to the left
		for i := foundIndex; i >= 0; i-- {
			line1 := lines1[i]
			if !line2.r.contains(line1.pos) {
				break
			}
			if line1.r.contains(line2.pos) && !yield(line1) {
				return
			}
		}

		// scan lines to the right (body of the loop is exactly the same :D )
		for i := foundIndex + 1; i < len(lines1); i++ {
			line1 := lines1[i]
			if !line2.r.contains(line1.pos) {
				break
			}
			if line1.r.contains(line2.pos) && !yield(line1) {
				return
			}
		}
	}
}

func compareLineByPos(line1, line2 Line) int {
	return cmp.Compare(line1.pos, line2.pos)
}

var sortLines = true // binary search sorted list instead of full scanning for better performance

type FindCrossingFunc func(lines []Line, line2 Line) iter.Seq[Line]

func Puzzle(input string, valueFunc func(Point, Line, Line) int) string {
	origin := Point{0, 0} // works for any origin
	moves1, moves2 := loadData(input)

	horizonalLines := make([]Line, 0)
	verticalLines := make([]Line, 0)
	for line := range walk(moves1, origin) {
		if line.isHorizontal {
			horizonalLines = append(horizonalLines, line)
		} else {
			verticalLines = append(verticalLines, line)
		}
	}

	var findCrossing FindCrossingFunc
	if sortLines { // two alternative implementations
		// sort in place
		slices.SortFunc(horizonalLines, compareLineByPos)
		slices.SortFunc(verticalLines, compareLineByPos)
		findCrossing = findCrossingLinesOrdered
	} else {
		findCrossing = findCrossingLines
	}

	minValue := math.MaxInt
	for line2 := range walk(moves2, origin) {
		for line1 := range findCrossing(utils.If(line2.isHorizontal, verticalLines, horizonalLines), line2) {
			value := valueFunc(origin, line1, line2)
			if value > 0 && value < minValue {
				minValue = value
			}
		}
	}

	return strconv.Itoa(minValue)
}

func Puzzle1(input string) string {
	return Puzzle(input, func(origin Point, line1, line2 Line) int {
		return utils.AbsDiff(utils.If(line1.isHorizontal, origin.y, origin.x), line1.pos) +
			utils.AbsDiff(utils.If(line2.isHorizontal, origin.y, origin.x), line2.pos)
	})
}

func Puzzle2(input string) string {
	return Puzzle(input, func(origin Point, line1, line2 Line) int {
		return line1.steps + utils.AbsDiff(line1.r.from, line2.pos) +
			line2.steps + utils.AbsDiff(line2.r.from, line1.pos)
	})
}

// ** alternative implementation
// - lines of the first snake are still sorted, but we are not searching them from scratch for each next line
// - we hold "current lines" (each per orientation) and moving them according to the movement of the second snake
// - performance should be little bit better ;)
type State struct {
	lines []Line
	index int // index of current line
}

func Puzzle_(input string, valueFunc func(Point, Line, Line) int) string {
	origin := Point{0, 0} // works for any origin
	moves1, moves2 := loadData(input)

	lines := map[bool]*State{ // pointer to struct :) to allow modifications without copying
		false: {nil, -1},
		true:  {nil, -1},
	}

	for line := range walk(moves1, origin) {
		state := lines[line.isHorizontal]
		state.lines = append(state.lines, line)
	}

	// sort in place
	slices.SortFunc(lines[true].lines, compareLineByPos)
	slices.SortFunc(lines[false].lines, compareLineByPos)

	minValue := math.MaxInt
	for line2 := range walk(moves2, origin) {
		state := lines[!line2.isHorizontal]
		dir := cmp.Compare(line2.r.to, line2.r.from) // 1 - forward, -1 - backward
		end := utils.If(dir == 1, len(state.lines)-1, 0)
		start := utils.If(dir == 1, 0, len(state.lines)-1)

		// set the "current line" (line in range), only if not already set
		if state.index == -1 {
			for i := start; cmp.Compare(i, end) != dir; /*If(dir == 1, i <= end, i >= end)*/ i += dir {
				line1 := state.lines[i]
				if line2.r.contains(line1.pos) {
					state.index = i
					break
				}
			}
		}

		if state.index != -1 {
			for i := state.index; cmp.Compare(i, end) != dir; i += dir {
				line1 := state.lines[i]

				// scan lines until range is missed
				if cmp.Compare(line1.pos, line2.r.to) == dir /*If(dir == 1, line1.pos > line2.r.to, line1.pos < line2.r.to)*/ {
					break
				}

				state.index = i // set new current index

				if line1.isCrossing(line2) {
					value := valueFunc(origin, line1, line2)
					if value > 0 && value < minValue {
						minValue = value
					}
				}
			}
		}
	}

	return strconv.Itoa(minValue)
}
