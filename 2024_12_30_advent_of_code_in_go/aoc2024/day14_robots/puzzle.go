package day14_robots

import (
	"aoc/utils"
	"fmt"
	"iter"
	"maps"
	"math"
	"regexp"
	"slices"
	"strconv"
	"strings"
)

type Coords struct {
	x, y int
}
type Robot struct {
	start, velocity Coords
}

func loadData(input string) []Robot {
	lines := utils.ParseLines(input)
	numbersRegex, _ := regexp.Compile(`-?\d+`) // also negative numbers

	robots := make([]Robot, len(lines))

	for i := 0; i < len(lines); i++ {
		ns := numbersRegex.FindAllString(lines[i], math.MaxInt)
		xpos, _ := strconv.Atoi(ns[0])
		ypos, _ := strconv.Atoi(ns[1])
		xvel, _ := strconv.Atoi(ns[2])
		yvel, _ := strconv.Atoi(ns[3])
		robots[i] = Robot{Coords{xpos, ypos}, Coords{xvel, yvel}}
	}

	return robots
}

func move(width, higth int, point, velocity Coords) Coords {
	x := point.x + velocity.x
	if x < 0 {
		x = width + x
	} else if x >= width {
		x = x - width
	}

	y := point.y + velocity.y
	if y < 0 {
		y = higth + y
	} else if y >= higth {
		y = y - higth
	}

	return Coords{x, y}
}

func puzzle1(width, higth int, input string) string {
	data := loadData(input)

	robots := make([]Coords, len(data))
	for i := range len(data) {
		robots[i] = data[i].start
	}

	steps := 100
	for range steps {
		for i := 0; i < len(robots); i++ {
			robots[i] = move(width, higth, robots[i], data[i].velocity)
		}
	}

	var q1, q2, q3, q4 int
	midx := width / 2
	midy := higth / 2

	for _, r := range robots {
		if r.x < midx {
			if r.y < midy {
				q1++
			} else if r.y > midy {
				q2++
			}
		} else if r.x > midx {
			if r.y < midy {
				q3++
			} else if r.y > midy {
				q4++
			}
		}
	}

	return fmt.Sprint(q1 * q2 * q3 * q4)
}

func Puzzle1(input string) string {
	return puzzle1(101, 103, input)
}

type RobotsMap map[int]map[int][]Coords

func setRobot(robots RobotsMap, pos, vel Coords) {
	row, ok := robots[pos.y]
	if !ok {
		row = make(map[int][]Coords)
		robots[pos.y] = row
	}
	row[pos.x] = append(row[pos.x], vel)
}

func foreachRobot(robots RobotsMap) iter.Seq2[Coords, Coords] {
	return func(yield func(Coords, Coords) bool) {
		for y, row := range robots {
			for x, vels := range row {
				for _, vel := range vels {
					if !yield(Coords{x, y}, vel) {
						return
					}
				}
			}
		}
	}
}

func findFirstSolidLine(row map[int][]Coords, lineLen int) int {
	columns := slices.Collect(maps.Keys(row))
	slices.Sort(columns)

	for _, c := range columns {
		i := 0
		for ; i < lineLen; i++ {
			if _, ok := row[c+i]; !ok {
				break
			}
		}
		if i == lineLen {
			return c
		}
	}

	return -1
}

func isSolidTriangle(robots RobotsMap, leftCorner Coords, widthOfTree int) bool {
	for r, c, w := leftCorner.y, leftCorner.x, widthOfTree; w > 0; r, c, w = r-1, c+1, w-2 {
		for i := range w {
			if _, ok := robots[r][c+i]; !ok {
				return false
			}
		}
	}
	return true
}

func printRobots(robots RobotsMap, width, higth int) {
	for r := 0; r < higth; r++ {
		line := make([]string, width)
		for i := range width {
			line[i] = "."
		}
		for c := range robots[r] {
			line[c] = "#"
		}
		fmt.Println(strings.Join(line, ""))
	}
}

func Puzzle2(input string) string {
	data := loadData(input)
	width := 101
	higth := 103
	widthOfTree := 7 // some even number
	higthOfTree := widthOfTree / 2

	robots := make(RobotsMap)
	for _, r := range data {
		setRobot(robots, r.start, r.velocity)
	}

	step := 1
	for {
		robotsnext := make(RobotsMap)
		for pos, vel := range foreachRobot(robots) {
			setRobot(robotsnext, move(width, higth, pos, vel), vel)
		}
		robots = robotsnext

		for r := higth - 1; r >= higthOfTree; r-- { // for each row from the bottom
			if row, ok := robots[r]; ok && len(row) >= widthOfTree { // if row contains enough robots ...
				if c := findFirstSolidLine(row, widthOfTree); c != -1 { // ... creating long enough solid line
					if isSolidTriangle(robots, Coords{c + 1, r - 1}, widthOfTree-2) { // triangle above current row
						// printRobots(robots, width, higth)
						return fmt.Sprint(step)
					}
				}
			}
		}

		step++
	}
}

// .....................................................................................................
// ............................................###############################..........................
// .......................#....................#.............................#.............#............
// ............................................#.............................#..........................
// ...........................#................#.............................#..........................
// .........#..................................#.............................#............#.............
// ............................................#..............#..............#........#.................
// ..#..#.....................#................#.............###.............#..........................
// ............................................#............#####............#..............#...........
// ............................................#...........#######...........#..........................
// .....................#......................#..........#########..........#..........................
// ..........#.................................#............#####............#..........................
// ............................................#...........#######...........#..........................
// ............................................#..........#########..........#..........................
// ...................#........................#.........###########.........#.......#..................
// ..........#.................................#........#############........#......................#...
// ...............#............................#..........#########..........#..........................
// ...........#...#............................#.........###########.........#..........................
// .........................#..................#........#############........#.......................#..
// ............................................#.......###############.......#..........................
// ............................................#......#################......#..........................
// ............................................#........#############........#..........................
// ..........#.................................#.......###############.......#..........................
// ............................................#......#################......#..........................
// ........................#........#..........#.....###################.....#..........................
// ............................................#....#####################....#..........................
// ....#.......................................#.............###.............#..........................
// ..#.....................#...................#.............###.............#...................#......
// ...............#............................#.............###.............#..........................
// .............#..............................#.............................#..........................
// ..............#...........................#.#.............................#..........................
// ............................................#.............................#................#.........
// ............................................#.............................#..........................
// ....................#.......................###############################..........................
