
 pattern-matching (11) list (10)  rec (9) unfold (7)  takeWhile (3)  forall (3) scan (2)  fold (2) collect (1)  find (1)  zip (1)  groupBy (1)  foldBack (1) forall (1) rec (1) rev (1)  scan (1)  pairwise (1)  take (1) indexed (1)  tryFindIndex (1) mapi (1)  filter (1) 

1. [P0001_TwoSum](https://leetcode.com/problems/two-sum/) `[| 2; 7; 11; 15 |] -> 9` collect, find
2. [P0002_AddTwoNumbers](https://leetcode.com/problems/add-two-numbers/) `[ 2; 4; 3 ] [ 5; 6; 4 ] // -> [7; 0; 8]` list, pattern-matching, rec
3. [P0003_LengthOfLongestSubstring](https://leetcode.com/problems/longest-substring-without-repeating-characters/) `"abcabcbb" -> 3` scan, takeWhile
4. [P0005_LongestPalindromicSubstring](https://leetcode.com/problems/longest-palindromic-substring/) `"babad" -> "bab"` unfold
5. [P0006_ZigzagConversion](https://leetcode.com/problems/zigzag-conversion/) `"PAYPALISHIRING" 3 -> 'PAHNAPLSIIGYIR"` unfold, zip, groupBy
6. [P0007_ReverseInteger](https://leetcode.com/problems/reverse-integer/) `123 -> 321` unfold, fold
7. [P0008_StringToIntegerAtoi](https://leetcode.com/problems/string-to-integer-atoi/) `"4193 with words" -> 4193` list, pattern-matching, rec, foldBack
8. [P0009_PalindromeNumber](https://leetcode.com/problems/palindrome-number/) `121 -> true` unfold, forall
9. [P0011_ContainerWithMostWater](https://leetcode.com/problems/container-with-most-water/) `[| 1; 8; 6; 2; 5; 4; 8; 3; 7 |] -> 49` list, pattern-matching, rec
10. [P0012_IntegerToRoman](https://leetcode.com/problems/integer-to-roman/) `3 -> "III"` unfold, fold, pattern-matching
11. [P0013_RomanToInt](https://leetcode.com/problems/roman-to-integer/) `"III" -> 3` list, pattern-matching
12. [P0014_LongestCommonPrefix](https://leetcode.com/problems/longest-common-prefix/) `[| "flower"; "flow"; "flight" |] -> "fl"` forall, takeWhile
13. [P0019_RemoveNthNodeFromEndOfList](https://leetcode.com/problems/remove-nth-node-from-end-of-list/) `[1,2,3,4,5] 2 -> [1,2,3,5]` list, pattern-matching, rec
14. [P0021_MergeTwoSortedLists](https://leetcode.com/problems/merge-two-sorted-lists/) `[ 1; 2; 4 ] [ 1; 3; 4 ] -> [1; 1; 2; 3; 4; 4]` list, pattern-matching, rec
15. [P0023_MergeKSortedLists](https://leetcode.com/problems/merge-k-sorted-lists/) `[[1,4,5],[1,3,4],[2,6]] -> [1,1,2,3,4,4,5,6]` list, pattern-matching, rec
16. [P0024_SwapNodesInPairs](https://leetcode.com/problems/swap-nodes-in-pairs/) `[1,2,3,4] -> [2,1,4,3]` list, pattern-matching, rec
17. [P0025_ReverseNodesInKGroup](https://leetcode.com/problems/reverse-nodes-in-k-group/) `[1,2,3,4,5] 2 -> [2,1,4,3,5]` list, pattern-matching, rec
18. [P0035_SearchInsert](https://leetcode.com/problems/search-insert-position/) `[| 1; 3; 5; 6 |] 5 -> 2` rec
19. [P0066_PlusOne](https://leetcode.com/problems/plus-one/) `[ 4; 3; 2; 2 ] -> [|4; 3; 2; 3|]` rev, scan
20. [P0118_PascalsTriangle](https://leetcode.com/problems/pascals-triangle/) `2 -> [ [ 1 ], [ 1, 1 ] ]` unfold, pairwise, take
21. [P0328_OddEvenLinkedList](https://leetcode.com/problems/odd-even-linked-list/) `[1,2,3,4,5] -> [1,3,5,2,4]` list, pattern-matching, rec
22. [P0387_FirstUniqChar](https://leetcode.com/problems/first-unique-character-in-a-string/) `"letmein" -> 0` indexed, tryFindIndex, forall
23. [P0392_IsSubsequence](https://leetcode.com/problems/is-subsequence/) `"abc" "ahbgdc" -> true` scan, forall
24. [P1342_NumberOfSteps](https://leetcode.com/problems/number-of-steps-to-reduce-a-number-to-zero/) `14 -> 6` unfold, takeWhile
25. [P1365_SmallerNumbersThanCurrent](https://leetcode.com/problems/how-many-numbers-are-smaller-than-the-current-number/) `[| 8; 1; 2; 2; 3 |] -> [4;0;1;1;3]` mapi, filter

