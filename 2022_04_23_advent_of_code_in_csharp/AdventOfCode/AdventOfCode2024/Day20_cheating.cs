namespace AdventOfCode.AdventOfCode2024.Day20;

record Point(int X, int Y);

record Move(int X, int Y);

record Data(char[][] Board, int Size, Point Start, Point End);

static class Day20
{
    public static Data LoadData(string input)
    {
        var board = Grid.LoadGridOfCharsFromData(input);
        var size = board[0].Length; // square 
        var lineLength = size + 1; // include end of line (\n)
        return new Data(board, size, FindCharInInput('S'), FindCharInInput('E'));

        Point FindCharInInput(char c)
        {
            var index = input.IndexOf(c);
            return new Point(index % lineLength, index / lineLength);
        }
    }

    // public static string Puzzle1(string input)
    // {
    //     var data = LoadData(input);
    //     Console.WriteLine(data);
    //     return data.ToString();
    // }

    private static Move[] allMoves = [new(0, 1), new(0, -1), new(1, 0), new(-1, 0)];

    private static Point MovePoint(Point point, Move move) => new(point.X + move.X, point.Y + move.Y);

    private static Dictionary<Point, int> BuildRouteMap(Data data)
    {
        Dictionary<Move, Move[]> nextMoveMap = allMoves.ToDictionary(
            move => move,
            move => new[] { move, new(move.Y, move.X), new(move.Y * -1, move.X * -1) });

        var current = new
        {
            Move = allMoves.First(move => MovePoint(data.Start, move) is var p && data.Board[p.Y][p.X] != '#'),
            Point = data.Start
        };
        var steps = 1;
        var route = new Dictionary<Point, int> { { current.Point, steps } };

        // walk the path
        while (true)
        {
            current = nextMoveMap[current.Move]
                .Select(move => MovePoint(current.Point, move) is var point && data.Board[point.Y][point.X] != '#'
                    ? new { Move = move, Point = point }
                    : null)
                .First(x => x is not null)!;

            steps++;
            route[current.Point] = steps;

            if (current.Point == data.End)
            {
                return route;
            }
        }
    }

    public static string Puzzle1(string input)
    {
        var data = LoadData(input);
        var routeMap = BuildRouteMap(data);
        var cheatsByPoints = new Dictionary<Point, int>();
        var cheatsHistogram = new Dictionary<int, int>();

        for (var y = 1; y < data.Size - 1; y++)
        {
            for (var x = 1; x < data.Size - 1; x++)
            {
                if (data.Board[y][x] == '#')
                {
                    if (
                        (routeMap.TryGetValue(new Point(x - 1, y), out var steps1) && // left 
                         routeMap.TryGetValue(new Point(x + 1, y), out var steps2)) || // right
                        (routeMap.TryGetValue(new Point(x, y + 1), out steps1) && // down
                         routeMap.TryGetValue(new Point(x, y - 1), out steps2)) // up
                    )
                    {
                        var savedSteps = Math.Abs(steps1 - steps2) - 2;
                        cheatsByPoints[new Point(x, y)] = savedSteps;
                        cheatsHistogram[savedSteps] = cheatsHistogram.GetValueOrDefault(savedSteps, 0) + 1;
                    }
                }
            }
        }

        var result = cheatsHistogram.Where(kv => kv.Key >= 100).Sum(kv => kv.Value);

        return result.ToString();
    }
}


// func AbsDiff(a, b int) int {
//     if diff := a - b; diff < 0 {
//         return diff * -1
//     } else {
//         return diff
//     }
// }

// func Puzzle1(input string) string {
// 	data := loadData(input)
//
// 	sizem1 := data.size - 1
// 	routeMap := buildRouteMap(data)
//
// 	cheatsByPoints := make(map[Point]int)
// 	cheatsHistogram := make(map[int]int)
//
// 	for y := 1; y < sizem1; y++ {
// 		for x := 1; x < sizem1; x++ {
//
// 				if steps1, ok1 := routeMap[Point{x, y + 1}]; ok1 {
// 					if steps2, ok2 := routeMap[Point{x, y - 1}]; ok2 {
// 						saved := utils.AbsDiff(steps1, steps2) - 2
// 						cheatsByPoints[Point{x, y}] = saved
// 						// fmt.Println(Point{x, y}, saved)
// 						//cheatsHistogram[saved] = cheatsHistogram[saved] + 1
// 						cheatsHistogram[saved] += 1
//
// 						// cheatsHistogram[saved] += 1 ??
// 					}
// 				}
// 			}
// 		}
// 	}
//
// 	// fmt.Println(cheatsHistogram)
// 	// fmt.Println(cheatsByPoints)
//
// 	sum := 0
// 	for saved, cheats := range cheatsHistogram {
// 		if saved >= 100 {
// 			sum += cheats
// 		}
// 	}
//
// 	fmt.Println(sum)
//
// 	// a := cheatsByPoints[Point{8, 1}]  // 12
// 	// b := cheatsByPoints[Point{10, 7}] // 20
// 	// c := cheatsByPoints[Point{8, 8}]  // 38
// 	// d := cheatsByPoints[Point{6, 7}]  // 64
//
// 	// fmt.Println(a, b, c, d)
//
// 	return ""
// }
//
// type Visited map[Point]map[Point]int
//
// type Stack[T any] []T
//
// func (stack Stack[T]) Push(item T) Stack[T] {
// 	newStack := append(stack, item)
// 	return newStack
// }
//
// func getNeighbours(data Data, point Point) iter.Seq[Point] {
// 	return func(yield func(Point) bool) {
// 		for _, m := range allMoves {
// 			if (point.x == 0 && m.x == -1) || (point.y == 0 && m.y == -1) ||
// 				(point.x == data.size-1 && m.x == 1) || (point.y == data.size-1 && m.y == 1) {
// 				continue // move out of bounds, skip it
// 			}
//
// 			if !yield(movePoint(point, m)) {
// 				return
// 			}
// 		}
// 	}
// }
//
// func getNeighboursRocks(data Data, point Point) iter.Seq[Point] {
// 	return func(yield func(Point) bool) {
// 		for n := range getNeighbours(data, point) {
// 			if data.board[n.y][n.x] == '#' {
// 				if !yield(movePoint(point, n)) {
// 					return
// 				}
// 			}
// 		}
// 	}
// }
//
// func (stack Stack[T]) Pop() (Stack[T], T, bool) {
// 	length := len(stack)
// 	if length > 0 {
// 		val := stack[length-1]
// 		newStack := stack[0 : length-1]
// 		return newStack, val, true
// 	}
// 	var val T
// 	return stack, val, false
// }
//
// func wasMoveExecuted(visited Visited, from, to Point) bool {
// 	if m1, ok1 := visited[from]; ok1 {
// 		_, ok2 := m1[to]
// 		return ok2
// 	}
// 	return false
// }
//
// func registerPoint(visited Visited, point Point, maxCheats int) {
//
// 	// neighbours :=
// 	// if m1, ok1 := visited[from]; ok1 {
// 	// 	_, ok2 := m1[to]
// 	// 	return ok2
// 	// }
// 	// return false
// }
//
// func Puzzle2(input string) {
// 	data := loadData(input)
//
// 	visited := make(Visited)
// 	todo := Stack[Point]{Point{0, 0}}
//
// 	for len(todo) > 0 {
//
// 		todo, point, _ := todo.Pop()
//
// 		for n := range getNeighboursRocks(data, point) {
//
// 			if !wasMoveExecuted(visited, point, n) && !wasMoveExecuted(visited, n, point) {
// 				todo = todo.Push(n)
// 			}
//
// 		}
//
// 	}
//
// }