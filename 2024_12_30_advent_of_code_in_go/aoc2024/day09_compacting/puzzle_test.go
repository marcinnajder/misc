package day09_compacting

// func TestLoadData(t *testing.T) {
// 	input := utils.ReadTextFile("./data_.txt")
// 	data := loadData(input)
// 	//fmt.Println(data)
// 	assert.Equal(t, 2, len(data.antennas))
// 	assert.Equal(t, 4, len(data.antennas['0']))
// 	assert.Equal(t, 3, len(data.antennas['A']))
// 	assert.True(t, slices.Contains(data.antennas['0'], Point{8, 1}))
// 	assert.False(t, slices.Contains(data.antennas['0'], Point{8, 2}))
// }

// func TestCalcAntinodes(t *testing.T) {
// 	// map[48:[{8 1} {5 2} {7 3} {4 4}] 65:[{6 5} {8 8} {9 9}]]}
// 	p1, p2 := calcAntinodes(Point{8, 8}, Point{9, 9})
// 	assert.Equal(t, Point{7, 7}, p1)
// 	assert.Equal(t, Point{10, 10}, p2)
// 	p3, p4 := calcAntinodes(Point{9, 9}, Point{8, 8})
// 	assert.Equal(t, Point{10, 10}, p3)
// 	assert.Equal(t, Point{7, 7}, p4)

// }

// func TestFindAntinodes(t *testing.T) {
// 	input := utils.ReadTextFile("./data_.txt")
// 	data := loadData(input)
// 	var rr = findAntinodes(Point{0, 0}, Point{1, 2}, data.size, 100000)

// 	fmt.Println(slices.Collect(rr))

// 	//r := Puzzle(input, 1000000)
// 	//fmt.Println(r)
// 	//assert.Equal(t, "3749", r)
// }

// func TestPuzzle1(t *testing.T) {
// 	input := utils.ReadTextFile("./data_.txt")
// 	// data := loadData(input)
// 	r := Puzzle1(input)

// 	fmt.Println(r)

// 	// for y, line := range data.lines {
// 	// 	for x, char := range line {

// 	// 	}

// 	// }

// 	//assert.Equal(t, "3749", r)
// }

// func TestPuzzle2(t *testing.T) {
// 	input := utils.ReadTextFile("./data_.txt")
// 	r := Puzzle2(input)
// 	// fmt.Println(r)
// 	assert.Equal(t, "11387", r)

// }
