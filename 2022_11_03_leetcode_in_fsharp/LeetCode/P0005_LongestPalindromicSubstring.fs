// url: https://leetcode.com/problems/longest-palindromic-substring/
// tags: unfold
// examples: "babad" -> "bab"


module LeetCode.P0005_LongestPalindromicSubstring

let longestPalindromicSubstring (s: string) =
    let i, j =
        { 0 .. s.Length - 1 }
        |> Seq.pairwise
        |> Seq.collect (fun (i, j) -> [ (i, j); (i, j + 1) ]) // double pairs
        |> Seq.take (((s.Length - 1) * 2) - 1) // skip last pair
        |> Seq.choose (fun (i, j) ->
            (i, j)
            |> Seq.unfold (fun (i, j) ->
                if i >= 0 && j <= s.Length - 1 && s[i] = s[j] then Some((i, j), (i - 1, j + 1)) else None) // expand substring
            |> Seq.tryLast)
        |> Seq.maxBy (fun (i, j) -> j - i)

    s.Substring(i, j - i + 1)

// longestPalindromicSubstring "bazzxczxckajakteasaqwwwwwpo" // -> kajak

let _ = longestPalindromicSubstring "babad" // -> bab
let _ = longestPalindromicSubstring "cbbd" // -> bb
// let _ = longestPalindromicSubstring "bazzxczxckajakteasaqwwwwwpo" // -> kajak


let longestPalindromicSubstringOptimized (s: string) =
    let _, (i, j) =
        { 0 .. s.Length - 1 }
        |> Seq.pairwise
        |> Seq.collect (fun (i, j) -> [ (i, j); (i, j + 1) ]) // double pairs
        |> Seq.take (((s.Length - 1) * 2) - 1) // skip last pair
        |> Seq.scan
            (fun state (i, j) ->
                state
                |> Option.bind (fun (maxLen, ij) ->
                    let maxLenPossible = (j - i + 1) + 2 * (s.Length - 1 - j)

                    if maxLenPossible > maxLen then
                        let x =
                            (i, j)
                            |> Seq.unfold (fun (i, j) ->
                                if i >= 0 && j <= s.Length - 1 && s[i] = s[j] then
                                    Some((i, j), (i - 1, j + 1))
                                else
                                    None) // expand substring
                            |> Seq.tryLast

                        match x with
                        | Some (ii, jj) when jj - ii + 1 > maxLen -> Some(jj - ii + 1, (ii, jj))
                        | _ -> Some(maxLen, ij)
                    else
                        None))
            (Some(0, (0, 0)))
        |> Seq.takeWhile Option.isSome
        |> Seq.last
        |> Option.get

    s.Substring(i, j - i + 1)
