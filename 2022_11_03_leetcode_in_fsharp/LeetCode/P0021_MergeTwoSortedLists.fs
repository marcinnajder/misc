// url: https://leetcode.com/problems/merge-two-sorted-lists/
// tags: list, pattern-matching, rec
// examples: [ 1; 2; 4 ] [ 1; 3; 4 ] -> [1; 1; 2; 3; 4; 4]

module LeetCode.P0021_MergeTwoSortedLists

open Utils

let rec mergeTwoLists list1 list2 =
    match list1, list2 with
    | lst, [] -> lst
    | [], lst -> lst
    | head1 :: tail1, head2 :: tail2 ->
        if head1 = head2 then head1 :: head2 :: mergeTwoLists tail1 tail2
        else if head1 < head2 then head1 :: mergeTwoLists tail1 list2
        else head2 :: mergeTwoLists list1 tail2

mergeTwoLists [ 1; 2; 4 ] [ 1; 3; 4 ] === [ 1; 1; 2; 3; 4; 4 ]
mergeTwoLists [] ([]: list<int>) === []
