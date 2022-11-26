// url: https://leetcode.com/problems/add-two-numbers/
// tags: list, pattern-matching, rec
// examples: [ 2; 4; 3 ] [ 5; 6; 4 ] // -> [7; 0; 8]

module LeetCode.P0002_AddTwoNumbers

let addTwoNumbers l1 l2 =
    let rec addDigits lst1 lst2 carry =
        match (lst1, lst2) with
        | [], [] -> if carry = 0 then [] else [ carry ]
        | head :: tail, [] ->
            let sum = head + carry
            (sum % 10) :: addDigits tail [] (sum / 10)
        | [], head :: tail ->
            let sum = head + carry
            (sum % 10) :: addDigits [] tail (sum / 10)
        | head1 :: tail1, head2 :: tail2 ->
            let sum = head1 + head2 + carry
            (sum % 10) :: addDigits tail1 tail2 (sum / 10)

    addDigits l1 l2 0


let _ = addTwoNumbers [ 2; 4; 3 ] [ 5; 6; 4 ] // -> [7; 0; 8]
let _ = addTwoNumbers [ 0 ] [ 0 ] // -> 0

let _ =
    addTwoNumbers [ 9; 9; 9; 9; 9; 9; 9 ] [
        9
        9
        9
        9
    ]
// ->[8; 9; 9; 9; 0; 0; 0; 1]
