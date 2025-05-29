# Programming with sequences in Go - solving tasks using gopowerseq

## Advent Of Code "count increases"

[Advent of code](https://adventofcode.com/) website provides many sample programming tasks. Implementing them is a great way of learning a new programming language. Let's take [the first one from 2021](https://adventofcode.com/2021/day/1). For the input list of numbers, count how many numbers are greater than the preview ones. For the sample data provided with the task description, there are 7 such numbers.

```
199 (N/A - no previous measurement)
200 (increased)
208 (increased)
210 (increased)
200 (decreased)
207 (increased)
240 (increased)
269 (increased)
260 (decreased)
263 (increased)
```

```go
import (
  "iter"
  "strconv"
  "strings"

  "github.com/marcinnajder/gopowerseq/seq"
  "github.com/marcinnajder/gopowerseq/seqs"
)

func loadData(input string) []int {
  lines := strings.Split(input, "\n")
  numbers := make([]int, len(lines))
  for i, line := range lines {
    if number, err := strconv.Atoi(line); err != nil {
      panic(err)
    } else {
      numbers[i] = number
    }
  }
  return numbers
}

func countIncreases(numbers iter.Seq[int]) int {
  return seq.Pipe3(
    numbers,
    seq.Pairwise[int](),
    seq.ToTuples[int, int](),
    seq.CountFunc(func(p seq.Tuple[int, int]) bool { return p.Item2 > p.Item1 }),
  )
}

func Puzzle1(input string) int {
  data := loadData(input)
  numbers := seq.Of(data...)
  return countIncreases(numbers)
}
```

The implementation using gopowerseq operators is quite simple. `Pairwise` function creates a sequence of pairs, then `CountFunc` function counts items matching specified condition. In the second part of the task, numbers are grouped in "sliding windows" with the size of 3, then all items inside the window are summed up. In the end, those sums are counted the same way as in the first part, whether `B+B+B` is greater than `A+A+A`, ... This time the correct answer is 5.

```
199  A
200  A B
208  A B C
210    B C D
200  E   C D
207  E F   D
240  E F G
269    F G H
260      G H
263        H
```

Once again, the gopowerseq library contains all the necessary operators.

```go
func Puzzle2(input string) int {
  data := loadData(input)
  numbers := seq.Pipe2(
    seq.Of(data...),
    seq.Windowed[int](3),
    seq.Map(seqs.Sum[int]))
  return countIncreases(numbers)
}
```

## Traversing the tree structure

Let's say we have a tree structure stored in the database. It's not excessively large, so we can load the entire structure into memory at once. Files and folders serve as a suitable representation for such a structure.

```go
type Element struct {
  name  string
  kind  string
  items []Element
}

var root = Element{
  name: "C",
  kind: "disc",
  items: []Element{
    {name: "A", kind: "folder", items: []Element{{"file1", "file", nil}, {"file2", "file", nil}}},
    {name: "file666", kind: "file"},
  },
}
```

Now let's say we have to implement two different REST endpoints:

- The first one finds the first element from the tree of the specified `kind` like `folder` or `file`
- The second one returns all unique `kind` values

It would be useful to have a general method for traversing the tree structure. We could use it during the implementation of both endpoints.

```go
func traverse(elements []Element) iter.Seq[Element] {
  return seqs.FlatMap(elements, func(e Element) iter.Seq[Element] {
    if e.items == nil {
      return seq.Of(e)
    }
    return seq.Concat(seq.Of(e), traverse(e.items))
  })
}
```

The `traverse` function converts the tree structure into a flat sequence of elements by using the `FlatMap` and `Concat` operators, along with recursive calls. This function is lazy, it serves as a blueprint for the process of traversing the tree. In general, there is no need to traverse the entire structure. In the first endpoint, our goal is to find the first element of a specified `kind`.

```go
func findElementOfKind(rootElement Element, kind string) *Element {
  allElements := traverse([]Element{rootElement})
  el, index := seq.Find(func(e Element) bool { return e.kind == kind })(allElements)
  if index >= 0 {
    return &el
  } else {
    return nil
  }
}
```

In the second endpoint, we must process the entire structure because our goal is to identify all the unique kinds of elements.

```go
func getUniqueElementTypes(rootElement Element) []string {
  return seq.Pipe2(
    traverse([]Element{rootElement}),
    seq.DistinctFunc(func(e Element) string { return e.kind }),
    seq.ToSlice[string](),
  )
}
```

The sequence allows to split the solution into two separated phases. The first one encapsulates the logic of iteration over tree data structure. Because the sequence is lazy, the real work can be postponed and moved to separate functions. In the second phase, we process all items at once (`getUniqueElementTypes`) or a minimal number of items until the specified element is found (`findElementOfKind`).
