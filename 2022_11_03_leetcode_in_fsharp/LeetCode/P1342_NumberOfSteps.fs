// url: https://leetcode.com/problems/number-of-steps-to-reduce-a-number-to-zero/
// tags: unfold, takeWhile
// examples: 14 -> 6

module LeetCode.P1342_NumberOfSteps

open Utils

let numberOfSteps num =
    2 * num // +1 to the final result
    |> Seq.unfold (fun n ->
        let x = if n % 2 = 0 then n / 2 else n - 1
        Some(x, x))
    |> Seq.takeWhile (fun n -> n <> 0)
    |> Seq.length


numberOfSteps 14 === 6
numberOfSteps 8 === 4
numberOfSteps 123 === 12
numberOfSteps 8 === 4
numberOfSteps 123 === 12
