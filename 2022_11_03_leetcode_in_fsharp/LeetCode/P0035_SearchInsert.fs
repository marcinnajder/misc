// url: https://leetcode.com/problems/search-insert-position/
// tags: rec
// examples: [| 1; 3; 5; 6 |] 5 -> 2

module LeetCode.P0035_SearchInsert

let searchInsert nums target =
    { 0 .. (Array.length nums) - 1 }
    |> Seq.tryPick (fun i -> if nums[i] >= target then Some i else None)
    |> Option.defaultValue nums.Length

let _ = searchInsert [| 1; 3; 5; 6 |] 5 // -> 2
let _ = searchInsert [| 1; 3; 5; 6 |] 2 // -> 1
let _ = searchInsert [| 1; 3; 5; 6 |] 7 // -> 4

// "You must write an algorithm with O(log n) runtime complexity."
let searchInsert' (nums: int []) target =
    let rec search i j =
        if i = j then
            if target < nums[i] then i else i + 1
        else if i + 1 = j then
            if target <= nums[i] then i
            else if target <= nums[j] then j
            else j + 1
        else
            let middleIndex = (j + i) / 2
            let middleValue = nums[middleIndex]

            if target = middleValue then middleIndex
            else if target < middleValue then search i middleIndex
            else search middleIndex j
    search 0 (nums.Length - 1)
