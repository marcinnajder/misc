package utils

var UsingPS bool = true

func UsePS[T any](withoutPS, withPS T) T {
	if UsingPS {
		return withPS
	} else {
		return withoutPS
	}
}
