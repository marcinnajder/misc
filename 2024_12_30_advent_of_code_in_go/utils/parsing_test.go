package utils

import (
	"testing"

	"github.com/stretchr/testify/assert"
)

func TestParseInts(t *testing.T) {
	assert.Equal(t, []int{1, 22, 333}, ParseIntsWithFields("  1  22   333  "))
	assert.Equal(t, []int{}, ParseIntsWithFields("   "))
}

func TestParseIntsLines(t *testing.T) {
	assert.Equal(t, [][]int{{1, 22}, {333}}, ParseLinesOfInts("  1  22  \n 333  "))
}
