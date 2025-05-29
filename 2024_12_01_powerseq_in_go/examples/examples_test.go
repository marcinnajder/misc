// go test ./...

package examples

import (
	"slices"
	"testing"

	"github.com/marcinnajder/gopowerseq/seq"
	"github.com/stretchr/testify/assert"
)

func TestTraverse(t *testing.T) {
	allElements := traverse([]Element{root})
	getName := func(e Element) string {
		return e.name
	}
	flatNames := slices.Collect(seq.Map(getName)(allElements))

	// fmt.Println(flatNames)
	assert.Equal(t, []string{"C", "A", "file1", "file2", "file666"}, flatNames)
}

func TestFindElementOfKind(t *testing.T) {
	assert.Equal(t, "C", findElementOfKind(root, "disc").name)
	assert.Equal(t, "A", findElementOfKind(root, "folder").name)
	assert.Equal(t, "file1", findElementOfKind(root, "file").name)
	assert.Nil(t, findElementOfKind(root, "unknown-kind"))
}

func TestGetUniqueElementTypes(t *testing.T) {
	assert.Equal(t, []string{"disc", "folder", "file"}, getUniqueElementTypes(root))
}
