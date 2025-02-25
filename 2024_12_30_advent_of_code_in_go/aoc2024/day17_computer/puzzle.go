package day17_computer

import (
	"aoc/utils"
	"fmt"
	"math"
	"strconv"
	"strings"
)

type Data struct {
	a, b, c int
	numbers []int
}

func loadData(input string) Data {
	lines := utils.ParseLines(input)

	registerPrefixLen := len("Register X: ")
	a, _ := strconv.Atoi(lines[0][registerPrefixLen:])
	b, _ := strconv.Atoi(lines[1][registerPrefixLen:])
	c, _ := strconv.Atoi(lines[2][registerPrefixLen:])

	programPrefixLen := len("Program: ")
	numbers := utils.ParseIntsWithSplit(lines[4][programPrefixLen:], ",")

	return Data{a, b, c, numbers}
}

func process(data Data) Data {
	result := make([]int, 0)

	a, b, c := data.a, data.b, data.c
	i := 0

	combo := func(val int) int {
		switch val {
		case 0, 1, 2, 3:
			return val
		case 4:
			return a
		case 5:
			return b
		case 6:
			return c
		default:
			panic("unknown operand found")
		}
	}
	for i < len(data.numbers) {
		opcode := data.numbers[i]
		operand := data.numbers[i+1]

		switch opcode {
		case 0:
			a = a / int(math.Pow(2, float64(combo(operand))))
		case 1:
			b = b ^ operand
		case 2:
			b = combo(operand) % 8
		case 3:
			if a != 0 {
				fmt.Println("JUMP", result, operand)
				i = operand // 'jump'
			}
		case 4:
			b = b ^ c
		case 5:
			result = append(result, combo(operand)%8)
		case 6:
			b = a / int(math.Pow(2, float64(combo(operand))))
		case 7:
			c = a / int(math.Pow(2, float64(combo(operand))))
		default:
			panic(fmt.Sprintf("unknown 'opcode' %d found", opcode))
		}

		if !(opcode == 3 && a != 0) { // if not 'jump' previously
			i += 2
		}
	}

	return Data{a, b, c, result}
}

func Puzzle1(input string) string {
	data := loadData(input)
	data2 := process(data)
	strs := make([]string, len(data2.numbers))
	for i, n := range data2.numbers {
		strs[i] = strconv.Itoa(n)
	}
	return strings.Join(strs, ",")
}

type State struct {
	a, b, c, ptr int
}

func process2(data Data) (Data, int) {
	result := make([]int, 0)
	states := make(map[State]struct{})

	jumps := 0

	a, b, c := data.a, data.b, data.c
	ptr := 0

	combo := func(val int) int {
		switch val {
		case 0, 1, 2, 3:
			return val
		case 4:
			return a
		case 5:
			return b
		case 6:
			return c
		default:
			panic("unknown operand found")
		}
	}

	for ptr < len(data.numbers) {
		opcode := data.numbers[ptr]
		operand := data.numbers[ptr+1]

		switch opcode {
		case 0:
			a = a / int(math.Pow(2, float64(combo(operand))))
		case 1:
			b = b ^ operand
		case 2:
			b = combo(operand) % 8
		case 3:
			if a != 0 {
				// fmt.Println("JUMP", result, operand)
				ptr = operand // 'jump'
				jumps++
			}
		case 4:
			b = b ^ c
		case 5:
			result = append(result, combo(operand)%8)
		case 6:
			b = a / int(math.Pow(2, float64(combo(operand))))
		case 7:
			c = a / int(math.Pow(2, float64(combo(operand))))
		default:
			panic(fmt.Sprintf("unknown 'opcode' %d found", opcode))
		}

		if !(opcode == 3 && a != 0) { // if not 'jump' previously
			ptr += 2
		}

		s := State{a, b, c, ptr}
		if _, ok := states[s]; ok {
			fmt.Println("CYCLE", s)
			panic("CYCLE")
		} else {
			states[s] = struct{}{}
		}
	}

	// fmt.Println("jumps", jumps)

	return Data{a, b, c, result}, jumps
}
