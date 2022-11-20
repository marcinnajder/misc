// https://leetcode.com/problems/number-of-steps-to-reduce-a-number-to-zero/

module P1342_NumberOfSteps

let numberOfSteps num =
    2 * num // +1 to the final result
    |> Seq.unfold (fun n ->
        let x = if n % 2 = 0 then n / 2 else n - 1
        Some(x, x))
    |> Seq.takeWhile (fun n -> n <> 0)
    |> Seq.length


let _ = numberOfSteps 14 // -> 6
let _ = numberOfSteps 8 // -> 4
let _ = numberOfSteps 123 // -> 12
let _ = numberOfSteps 8 // -> 4
let _ = numberOfSteps 123 // -> 12
