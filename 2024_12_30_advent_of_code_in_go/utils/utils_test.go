package utils

import (
	"testing"

	"github.com/stretchr/testify/assert"
)

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
