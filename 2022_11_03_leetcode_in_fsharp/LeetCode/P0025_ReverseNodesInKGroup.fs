// url: https://leetcode.com/problems/reverse-nodes-in-k-group/
// tags: list, pattern-matching, rec
// examples:  [1,2,3,4,5] 2 -> [2,1,4,3,5]

module LeetCode.P0025_ReverseNodesInKGroup

open Utils

let rec reverseKGroup' lst k n gr rest =
    match lst with
    | [] -> rest
    | head :: tail ->
        if n = k then
            head :: gr @ (reverseKGroup' tail k 1 [] tail)
        else
            reverseKGroup' tail k (n + 1) (head :: gr) rest

let reverseKGroup lst k = reverseKGroup' lst k 1 [] lst

reverseKGroup [ 1; 2; 3; 4; 5 ] 2 === [ 2; 1; 4; 3; 5 ]
reverseKGroup [ 1; 2; 3; 4; 5 ] 3 === [ 3; 2; 1; 4; 5 ]


// todo: Follow-up: Can you solve the problem in O(1) extra memory space?
// - it's possible that it cannot be done using immutable list, instead mutable list must be used
