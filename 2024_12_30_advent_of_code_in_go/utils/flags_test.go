package utils

import (
	"testing"

	"github.com/stretchr/testify/assert"
)

// TestFlags
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
