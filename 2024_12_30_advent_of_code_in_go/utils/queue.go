package utils

// https://www.geeksforgeeks.org/queue-in-go-language/
// https://stackoverflow.com/a/55214816

// https://stackoverflow.com/questions/2818852/is-there-a-queue-implementation
// - https://stackoverflow.com/a/55214816
// - https://pkg.go.dev/std -> https://pkg.go.dev/container -> https://pkg.go.dev/container/list@go1.24.0

// https://www.geeksforgeeks.org/queue-in-go-language/
// - There are many ways to implement queues in Golang using other Data structures as: 1. Using Slices 2. Using Structures 3. Using LinkList

// // Queue is a queue
// type Queue interface {
// 	Front() *list.Element
// 	Len() int
// 	Add(interface{})
// 	Remove()
// }

// type queueImpl struct {
// 	*list.List
// }

// func (q *queueImpl) Add(v interface{}) {
// 	q.PushBack(v)
// }

// func (q *queueImpl) Remove() {
// 	e := q.Front()
// 	q.List.Remove(e)
// }

// // New is a new instance of a Queue
// func New() Queue {
// 	return &queueImpl{list.New()}
// }
