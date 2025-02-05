package day13_combinations

import (
	"aoc/utils"
	"fmt"
	"math"
	"regexp"
	"strconv"
)

type Position struct {
	x, y int
}
type Game struct {
	a, b, prize Position
}

func loadData(input string) []Game {
	lines := utils.ParseLines(input)
	numbersRegex, _ := regexp.Compile(`\d+`)
	games := make([]Game, (len(lines)+1)/4)

	i := 0
	j := 0
	for i < len(lines) {
		as := numbersRegex.FindAllString(lines[i], math.MaxInt)
		ax, _ := strconv.Atoi(as[0])
		ay, _ := strconv.Atoi(as[1])
		bs := numbersRegex.FindAllString(lines[i+1], math.MaxInt)
		bx, _ := strconv.Atoi(bs[0])
		by, _ := strconv.Atoi(bs[1])
		ps := numbersRegex.FindAllString(lines[i+2], math.MaxInt)
		px, _ := strconv.Atoi(ps[0])
		py, _ := strconv.Atoi(ps[1])
		games[j] = Game{Position{ax, ay}, Position{bx, by}, Position{px, py}}
		i += 4
		j++
	}

	return games
}

//   manually derived formula :)
// Button A: X+94, Y+34
// Button B: X+22, Y+67
// Prize: X=8400, Y=5400
// y = (5400*94 - 34*8400) / (67*94 -34*22)
// x = (8400 - 22y) / 94

func playWithFormula(g Game) int {
	b := (g.prize.y*g.a.x - g.prize.x*g.a.y) / (g.b.y*g.a.x - g.a.y*g.b.x)
	a := (g.prize.x - g.b.x*b) / g.a.x

	// we are using 'int' type instead of 'float' so the result is truncated
	// belowe we check the final equation, it will work corretly only for whole numbers
	if a*g.a.x+b*g.b.x == g.prize.x && a*g.a.y+b*g.b.y == g.prize.y {
		return a*3 + b
	}

	return 0
}

func Puzzle(input string, increaseBy int, play func(g Game) int) string {
	games := loadData(input)
	sum := 0
	for _, g := range games {
		g.prize.x += increaseBy
		g.prize.y += increaseBy
		sum += play(g)
	}
	return fmt.Sprint(sum)
}

func Puzzle1(input string) string {
	return Puzzle(input, 0, playWithFormula)
}

func Puzzle2(input string) string {
	return Puzzle(input, 10000000000000, playWithFormula)
}

// works only with small numbers :)
func playWithIteration(game Game) int {
	for a := 0; a <= 100; a++ { // check no more than 100 times
		xval := game.a.x * a
		if xval > game.prize.x {
			break
		}

		xleft := game.prize.x - xval
		if xleft%game.b.x == 0 {
			b := xleft / game.b.x
			if a*game.a.y+b*game.b.y == game.prize.y {
				return a*3 + b
			}
		}
	}
	return 0
}

func Puzzle1_(input string) string {
	return Puzzle(input, 0, playWithIteration)
}
