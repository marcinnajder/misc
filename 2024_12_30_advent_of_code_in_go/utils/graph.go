package utils

import (
	"iter"
	"slices"

	pq "gopkg.in/dnaeon/go-priorityqueue.v1"
)

// https://www.redblobgames.com/pathfinding/a-star/introduction.html
// https://pkg.go.dev/container/heap
// https://github.com/dnaeon/go-priorityqueue

type Edge[T any] struct {
	From   T
	To     T
	Weight int
}

type Visited[T comparable] struct {
	Node     T
	Length   int
	CameFrom map[T][]T
}

func DijkstraTraverseSeq[T comparable](graph iter.Seq[Edge[T]], start T) iter.Seq[Visited[T]] {
	return func(yield func(Visited[T]) bool) {
		costSoFar := map[T]int{start: 0}
		queue := pq.New[T, int64](pq.MinHeap)
		queue.Put(start, int64(0))
		cameFrom := make(map[T][]T)

		neighbors := make(map[T][]Edge[T])
		for e := range graph {
			neighbors[e.From] = append(neighbors[e.From], e)
		}

		for !queue.IsEmpty() {
			item := queue.Get()

			if !yield(Visited[T]{item.Value, int(item.Priority), cameFrom}) {
				return
			}

			for _, next := range neighbors[item.Value] {
				newCost := costSoFar[item.Value] + next.Weight
				if nextCost, ok := costSoFar[next.To]; !ok || newCost < nextCost {
					costSoFar[next.To] = newCost
					queue.Put(next.To, int64(newCost))
					cameFrom[next.To] = []T{item.Value}
				} else if ok && newCost == nextCost { // many paths of the same length
					cameFrom[next.To] = append(cameFrom[next.To], item.Value)
				}
			}
		}
	}
}

func DijkstraTraverse[T comparable](graph []Edge[T], start T) iter.Seq[Visited[T]] {
	return DijkstraTraverseSeq(slices.Values(graph), start)
}

func DijkstraShortestPath[T comparable](graph []Edge[T], start T, finish T) int {
	return DijkstraShortestPathFunc(graph, start, func(visited Visited[T]) bool {
		return visited.Node == finish
	})
}

func DijkstraShortestPathFunc[T comparable](graph []Edge[T], start T, pred func(Visited[T]) bool) int {
	for visited := range DijkstraTraverse(graph, start) {
		if pred(visited) {
			return visited.Length
		}
	}
	panic("couldn't find any item meeting passed predicate")
}

type Point struct {
	X, Y int
}

func BuildGraph(g [][]int, size int) []Edge[Point] {
	edges := make([]Edge[Point], 0)
	for e := range BuildGraphSeq(g, size) {
		edges = append(edges, e)
	}
	return edges
}

func BuildGraphSeq(g [][]int, size int) iter.Seq[Edge[Point]] {
	return func(yield func(Edge[Point]) bool) {
		sizem1 := size - 1

		addEdges := func(x1, y1, x2, y2 int) bool {
			if !yield(Edge[Point]{From: Point{x1, y1}, To: Point{x2, y2}, Weight: g[y2][x2]}) {
				return false
			}
			if !yield(Edge[Point]{From: Point{x2, y2}, To: Point{x1, y1}, Weight: g[y1][x1]}) {
				return false
			}
			return true
		}

		// all rows and columns without the last ones
		for y := 0; y < size-1; y++ {
			for x := 0; x < size-1; x++ {
				if !addEdges(x, y, x, y+1) {
					return
				}
				if !addEdges(x, y, x+1, y) {
					return
				}
			}
		}

		// last column and row
		for i := range sizem1 {
			if !addEdges(i, sizem1, i+1, sizem1) { // last row
				return
			}
			if !addEdges(sizem1, i, sizem1, i+1) { // last column
				return
			}
		}
	}
}

func GetAllVisited[T comparable](endVisited []Visited[T]) map[T]struct{} {
	visitedNodes := make(map[T]struct{})

	if len(endVisited) == 0 {
		return visitedNodes
	}

	cameFrom := endVisited[0].CameFrom
	pathNodes := make([]T, len(endVisited))
	for i := range len(endVisited) {
		pathNodes[i] = endVisited[i].Node
	}

	for i := 0; i < len(pathNodes); i++ { // 'pathNodes' is mutated during iteration
		currentNode := pathNodes[i]
		visitedNodes[currentNode] = struct{}{}

		for _, t := range cameFrom[currentNode] {
			if _, ok := visitedNodes[t]; !ok {
				pathNodes = append(pathNodes, t)
			}
		}
	}

	return visitedNodes
}
