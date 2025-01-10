package utils

import (
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestParseInts(t *testing.T) {
	assert.Equal(t, []int{1, 22, 333}, ParseInts("  1  22   333  "))
	assert.Equal(t, []int{}, ParseInts("   "))
}

func TestParseIntsLines(t *testing.T) {
	assert.Equal(t, [][]int{{1, 22}, {333}}, ParseIntsLines("  1  22  \n 333  "))
}

func TestCopyOnRemove(t *testing.T) {
	array := [...]int{0, 5, 10, 15, 100} // [5]int
	slice1 := array[:]

	assert.Equal(t, 5, len(array))
	assert.Equal(t, 5, len(slice1))

	slice2 := CopyOnRemove(slice1, 1)
	assert.Equal(t, []int{0, 10, 15, 100}, slice2)
	assert.Equal(t, [...]int{0, 5, 10, 15, 100}, array) // array was not modified

	assert.Equal(t, []int{ /*0*/ 5, 10, 15, 100}, CopyOnRemove(slice1, 0))
	assert.Equal(t, []int{0, 5, 10, 15 /*100*/}, CopyOnRemove(slice1, 4))
}

type tfs int

const tfsNone tfs = 0

const (
	tfsOne tfs = 1 << iota
	tfsTwo
	tfsFour
)

func TestFlags(t *testing.T) {

	var value tfs = tfsNone

	assert.Equal(t, tfsTwo, AddFlags(value, tfsTwo))
	assert.Equal(t, tfsFour, AddFlags(value, tfsFour))

	value = AddFlags(value, tfsOne)
	assert.Equal(t, tfsOne, value)
	assert.True(t, HasFlags(value, tfsOne))

	value = AddFlags(value, tfsTwo)
	assert.True(t, HasFlags(value, tfsOne))
	assert.True(t, HasFlags(value, tfsTwo))

	value = RemoveFlags(value, tfsOne)
	assert.False(t, HasFlags(value, tfsOne))
	assert.True(t, HasFlags(value, tfsTwo))

	allflags := AllFlags(tfsFour)
	assert.True(t, HasFlags(allflags, tfsOne))
	assert.True(t, HasFlags(allflags, tfsTwo))
	assert.True(t, HasFlags(allflags, tfsFour))

	assert.True(t, HasFlags(allflags, tfsOne, tfsTwo, tfsFour))

}
