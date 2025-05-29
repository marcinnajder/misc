// go test ./...

package examples

import (
	"iter"

	"github.com/marcinnajder/gopowerseq/seq"
	"github.com/marcinnajder/gopowerseq/seqs"
)

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

func traverse(elements []Element) iter.Seq[Element] {
	return seqs.FlatMap(elements, func(e Element) iter.Seq[Element] {
		if e.items == nil {
			return seq.Of(e)
		}
		return seq.Concat(seq.Of(e), traverse(e.items))
	})
}

func findElementOfKind(rootElement Element, kind string) *Element {
	allElements := traverse([]Element{rootElement})
	el, index := seq.Find(func(e Element) bool { return e.kind == kind })(allElements)
	if index >= 0 {
		return &el
	} else {
		return nil
	}
}

func getUniqueElementTypes(rootElement Element) []string {
	return seq.Pipe2(
		traverse([]Element{rootElement}),
		seq.DistinctFunc(func(e Element) string { return e.kind }),
		seq.ToSlice[string](),
	)
}

// function getUniqueElementTypes(element: Element) {
//   return pipe(
//     traverse([element]),
//     map((ee) => ee.type),
//     distinct(),
//     toarray()
//   );
// }
