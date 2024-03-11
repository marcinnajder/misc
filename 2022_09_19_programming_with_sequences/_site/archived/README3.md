## Programming with sequences part 3 - leetcode problems

#### Introduction

In the first part of the series, we began at the foundational level by addressing a few fundamental questions:

- What is an iterator pattern?
- What distinguishes a collection from a sequence?
- What is a generator function in JavaScript?

Following that, we implemented operators such as `filter`, `map`, `range`, `toarray`, and `take`. The simple `pipe` function enables us to combine multiple operators into a single expression.

In the second part, we have been talking about the code patterns that we can find very often. Rather than repeatedly implementing them from scratch, we discussed the approach of implementing them once for future reuse. We introduced the following operators: `flatmap`, `distinct`, `count`, `toobject`, `join`, `groupby`, and `concat`.

In this part, we will use the knowledge we have acquired so far to solve a few problems from the [leetcode](https://leetcode.com/) web side. At the time of writing this article (the end of 2022), leetcode contained around 2500 small programming tasks. In general, we can implement them in any programming language we want. However, we will implement a few random problems in JavaScript using a functional style. We will be using the [powerseq](https://github.com/marcinnajder/powerseq) library, introducing a bunch of new operators. Once more, we could use any JavaScipt library that provides functions for working with collections, it does not have to be the powerseq library. The key point here is to show that different styles of programming exist and can offer significant value.

#### smallerNumbersThanCurrent

Let's begin with a straightforward task without introducing any new operators. We have an array of numbers, and our objective is to return a new array of numbers with the same length. For example, given the input array `[8, 1, 2, 2, 3]` the output array should be `[4, 0, 1, 1, 3]`.The algorithm for achieving this is as follows:

- For any index `i` the value of `output[i]` is the number of items from `input` that are smaller than `input[i]`.
- The first item is `4` because there are `4` numbers smaller than `8` (`1, 2, 2, 3`).
- The second item is `0` because there are no numbers smaller than `1`.
- The same rule applies for all remaining numbers.

```javascript
var { pipe, count, map, toarray, range } = require("powerseq");

function smallerNumbersThanCurrent(numbers) {
  return pipe(
    numbers,
    map((n, i) =>
      count(range(0, numbers.length), (j) => i !== j && numbers[j] < n)
    ),
    toarray()
  );
}

smallerNumbersThanCurrent([8, 1, 2, 2, 3]); // -> [4, 0, 1, 1, 3]
smallerNumbersThanCurrent([6, 5, 4, 8]); // -> [ 2, 1, 0, 3 ]
smallerNumbersThanCurrent([7, 7, 7, 7]); // -> [ 0, 0, 0, 0 ]
```

We achieve this transformation by mapping the `input` array to the `output` array. In the lambda function used for mapping, `n` represents an element from the `input` array, and `i` represents the index of the next element in the array. We utilise the `range` operator to create a sequence of values `0, 1, 2, ...` for all indexes. The `count` operator counts the number of items smaller than `n`. By simply examining the names of the operators used, we can readily understand the logic behind this operation.

#### twoSum (find)

The next problem is very similar to the previous one. We are given an array of numbers `nums` and a `target` number, and our objective is to return an array containing the indexes of two items from the array. The sum of these items should be equal to the `target` number. For example, if `nums` equal `[2, 7, 11, 15]` and `target` equal `9`, the result will be `[0,1]` because `nums[0] + nums[1]` equals `2+7=9`.

```javascript
var { find, flatmap, range, pipe, find } = require("powerseq");

function twoSum(nums, target) {
  return pipe(
    range(0, nums.length),
    flatmap(
      (i) => range(0, i),
      (i, j) => [j, i]
    ),
    find(([j, i]) => nums[i] + nums[j] === target)
  );
}

twoSum([2, 7, 11, 15], 9); // -> [0, 1]
twoSum([3, 2, 4], 6); // -> [ 1, 2 ]
twoSum([3, 3], 6); // -> [ 0, 1 ]
```

We combine the two operators `range` and `flatmap` to generate pairs of indexes like `[0,1], [0,2], [1,2], [0,3], [1,3], [2,3], ...`. A new operator `find` returns the first element for which the lambda function returns `true`.

#### firstUniqChar (findIndex, every)

In the next example, we receive a string value and return the index of the first unique character, returning `-1` when there are no unique characters. For the input string `"letmein"`, it returns `0` because the character `"l"` is unique. For `"lifeislovepoem"`, it returns `2` because character `"f"` is unique. For `"aabb"`, it returns `-1` because there are no unique characters.

```javascript
var { findindex, every, range } = require("powerseq");

function firstUniqChar(s) {
  return (
    findindex(s, (c, i) =>
      every(range(0, s.length), (j) => i === j || c !== s[j])
    ) ?? -1
  );
}

firstUniqChar("letmein"); // -> 0
firstUniqChar("lifeislovepoem"); // -> 2
firstUniqChar("aabb"); // -> -1
```

We introduced two new operators. The `findIndex` operator works almost the same as the `find` operator, but instead of returning the found item, it returns the index of the found item. The `every` operator returns `true` if all items of the sequence satisfy the condition specified by the predicate function. However, if the predicate function returns `false` for any item, the execution stops, and the operator returns `false` as the result.

#### isSubsequence (scan, some)

In this example, the function takes two strings as input and returns `true` if the first string is a subsequence of the second string. For example, the `"abc"` string is a subsequence of `"ahbgdc"` because even if we omit a few characters in `"ahbgdc"`, we can still find `"abc"` characters in that order. However, `"axc"` is not a subsequence of the `"ahbgdc"` string because the `"x"` character can not be found in `"ahbgdc"`.

```javascript
var { pipe, scan, every, find, range } = require("powerseq");

function isSubsequence(s, t) {
  return pipe(
    s,
    scan(
      (i, c) => find(range(i + 1, t.length - (i + 1)), (j) => t[j] === c) ?? -1,
      -1
    ),
    every((i) => i !== -1)
  );
}

isSubsequence("abc", "ahbgdc"); // -> true
isSubsequence("axc", "ahbgdc"); // -> false
```

The `scan` operator is very powerful, as it enables the implementation of more advanced scenarios. The powerseq library provides a `reduce` operator that works in the same manner as the built-in `reduce` method from the `Array` type. The `scan` operator is similar to `reduce`. If we execute `reduce(range(1, 3), (prev, curr) => prev + curr, 0)` the value `6` will be returned: - The first time the lambda function is called with `prev=0` and `curr=1`, resulting in `0 + 1 = 1`. - The second time the lambda function is called with `prev=1` and `curr=2`, resulting in `1 + 2 = 3`. - Finally, the last call has `prev=3` and `curr=3`, returning `3 + 3 = 6`.

If we execute the same code but with the `scan` operator, like so: `scan(range(1, 3), (prev, curr) => prev + curr, 0)`, the sequence `1, 2, 6` will be retuned. The `reduce` operator is eager, meaning it executes immediately and returns the final result. In contrast, the `scan` operator is lazy. It returns a lazy sequence of intermediate values up to the final result. The consumer of the sequence determines how many items should be produced or when to stop processing.

Let's try to analyse what exactly happens once we execute `isSubsequence("abc", "ahbgdc")`. The type of the `s` parameter is a string, and we can think of a string type as a sequence of characters. We call the `scan` operator, passing a string `s` as a parameter and initialising it with the value `-1`. The lambda function takes two parameters: `c`, which is a string representing the next character from the `s` string, and `i`, which is the aggregated value (starting at `-1`). The goal of the lambda function is to find the first index of the current `c` character in string `t` (the second parameter of the `isSubsequence` function). The trick here is to not search from the beginning every time, but from the index of the previously found character. The aggregated value is used precisely for that reason. To gain a better understanding of the entire process, let's imagine that we have replaced the `every` operator with `toarray` in the code above. After making these changes, the execution of `isSubsequence("abc", "ahbgdc")` would return `[0, 2, 5]`, but the execution of `isSubsequence("axc", "ahbgdc")` would return `[0, -1, 5]`. The value `-1` would be returned because the `x` character does not exist in the string `"ahbgdc"`. The `every` operator pulls next indexes from the sequence until the first value `-1` is found or until all items have been processed.

There is even simpler implementation of `isSubsequence` function:

```javascript
function isSubsequence2(s, t) {
  return pipe(
    t,
    scan((i, c) => (s[i] === c ? i + 1 : i), 0),
    some((i) => i === s.length)
  );
}
```

This time, we iterate over the second string parameter `t` instead of the first one `s`. The aggregated value `i` represents the index of the character from the `s` string, which is incremented every time we find the matching character from the `t` string. New operator `some` returns `true` if the index `i` reaches the length of the `s` string parameter.

#### plusOne (reverse, skipwhile)

To understand the next problem, let's recall some elementary school math – the addition of two numbers using the written column method. We represent a number as an array of digits, where the number 4322 is represented as `[4, 3, 2, 2]`. The goal is to add value `1` to the input number and return the result as an array of digits, which in our case would be `[4, 3, 2, 3]`. Depending on the numbers, sometimes we have to carry a value of `1` from right to left. For example, when adding `1` to the number `[9]`, it should return `[1, 0]`, indicating that one additional digit has been added to the front of the result.

```javascript
var {
  scan,
  reverse,
  pipe,
  toarray,
  map,
  concat,
  skipwhile,
} = require("powerseq");

function plusOne(digits) {
  return pipe(
    concat([0], digits),
    reverse(),
    scan(
      ([_, carry], digit) =>
        carry === 0 ? [digit, 0] : digit === 9 ? [0, 1] : [digit + 1, 0],
      [0, 1]
    ),
    map(([d, _]) => d),
    reverse(),
    skipwhile((d) => d === 0),
    toarray()
  );
}

plusOne([4, 3, 2, 2]); // ->  [ 4, 3, 2, 3 ]
plusOne([1, 2, 3]); // -> [1,2,4]
plusOne([9]); // -> [1, 0]
```

The expression looks scary at first, but in reality, it's quite simple. First, let's take a look at the beginning and the end of the expression. At the beginning, the digit `0` is artificially added to the front of the number and then all digits are reversed using new `reverse` operator. At the end, a potentially unnecessary digit `0` is removed using `skipwhile` operator, and then all the digits are reversed back to their original order. The new operator `skipwhile` starts from the first digit, checking whether the digit is equal to `0`. So starting from the first non-zero number, all successive numbers will be returned. This way, it effectively skips all leading zeros.

The most interesting part of the code is located in the middle of the expression, where the `scan` operator plays a crucial role once again. We process subsequent digits one by one, storing the carry value (`1` or `0`) as an aggregated value. To be precise, the aggregated value is an array of two numbers, not a single value. The first item of the array represents the digit actually being processed, while the second item represents the carry value. Immediately after the `scan` operator, a `map` operation is used to extract the first item of the aggregated array, which is the digit of the final number.

#### numberOfSteps (expand, takewhile)

Another very powerful operator next to `scan` is `expand`. Let's see it in action. We have a positive number, and we want to calculate the number of steps it takes to reach the value `0`. In each step, the number is decreased in the following way:

- If the number is even, it is divided by 2.
- If the number is odd, it is decremented by 1.

For number `14` we will reach `0` in `6` steps: `7, 6, 3, 2, 1, 0`

```javascript
var { pipe, expand, takewhile, count } = require("powerseq");

function numberOfSteps(num) {
  return pipe(
    [num],
    expand((n) => (n % 2 === 0 ? [n / 2] : [n - 1])),
    takewhile((n) => n !== 0),
    count()
  );
}

numberOfSteps(14); // -> 6
numberOfSteps(8); // -> 4
numberOfSteps(123); // -> 12
```

In some sense, the `expand` operator is the opposite of `reduce` operator. `reduce` combines a sequence of values into a single value, while `expand` generates a sequence of values starting from the single initial value (sometimes many values). In some other programming languages `reduce` is called `fold` and `expand` is called `unfold`. While these analogies may not be 100% accurate, they provide a close enough understanding of the operators' roles.

First, let's see how the `expand` operator works exactly. When we execute the following code: `expand([1, 2, 3], n => [n * 10, n * 100])`, it creates an infinite sequence of values: `1, 2 , 3, 10, 100, 20, 200, 30, 300, 100, 1000, 1000, 10000, 200, 2000, 2000, 20000, 300, 3000, 3000, ... `. We can imagine that we have an internal queue initially filled with the values from the sequence. The queue would look like this: `3 -> 2 -> 1`. We execute the lambda function for the first item in the queue `1`, and we get back a new sequence of values `[10, 100]`. These items are added to the queue, resulting in the queue looking like this: `100 -> 10 -> 3 -> 2`. The `expand` operator is lazy, every time we request the next value, the next item from the queue is returned and passed to the lambda function.

Thanks to the `expand` operator, the initial number `14` is converted into the sequence of numbers `14, 7, 6, 3, 2, 1, 0`. We introduced a new operator `takewhile`, that returns subsequent items while the condition is met. In our case, we are taking items while there are not `0`. Finally, we count the number of items in the sequence.

#### pascalsTriangle (pairwise)

This will be the final problem for today. We will generate [Pascal's Triangle](https://en.wikipedia.org/wiki/Pascal%27s_triangle), which looks like this:

```
     1
    1 1
   1 2 1
  1 3 3 1
 1 4 6 4 1
1 5 10 10 5 1
...
```

It is generated from top to bottom. The first and last number in each row is always `1`. Other numbers are calculated from the row above as the sum of the two numbers directly above them, one to the upper left and one to the upper right. For instance, the fifth row `1 4 6 4 1` was calculated in the following way: `1 (1+3) (3+3) (3+1) 1`. The task is to return a list of a specified number of rows. For example, executing `pascalsTriangle(3)` returns `[ [ 1 ], [ 1, 1 ], [ 1, 2, 1 ] ]`.

```javascript
var { pipe, take, toarray, map, expand } = require("powerseq");

function pascalsTriangle(rowNumber) {
  return pipe(
    expand([[1]], (prev) => [
      [1, ...map(pairwise(prev), ([x, y]) => x + y), 1],
    ]),
    take(rowNumber),
    toarray()
  );
}

pascalsTriangle(0); // -> [ ]
pascalsTriangle(1); // -> [ [ 1 ] ]
pascalsTriangle(2); // -> [ [ 1 ], [ 1, 1 ] ]
```

This implementation perfectly showcases the power of declarative programming. A quick look, and we can immediately understand what this function is doing:

- Start with the first row `[1]`.
- Each subsequent row will have the following shape: `[1, ...map(pairwise(prev), ([x, y]) => x + y), 1]`.
- Take only the first `rowNumber` rows.

The `pairwise` operator converts a sequence of items into a sequence of overlapping pairs. For example, `pairwise([1, 5, 10, 15])` returns `[ [1, 5], [5, 10], [10, 15] ]`. We could achieve the same effect using the powerseq operator called `buffer`. However, we implemented `pairwise` ourselves, which is a simplified and more specific version of `buffer`. We did this to remind ourselves that we can always write our own operators.

```javascript
function* pairwise(items) {
  var iterator = items[Symbol.iterator]();
  var result;

  if (!(result = iterator.next()).done) {
    var prev = result.value;
    while (!(result = iterator.next()).done) {
      yield [prev, result.value];
      var prev = result.value;
    }
  }
}
```

#### Summary

I can hear you saying _"...but there are not a real-world examples, there are just the artificial puzzles used sometimes during job interviews! "_. Really? I am not so sure. Try to recall when was the last time you wrote code to process data or perform repetitive tasks. There's a high probability that you used loops, conditional statements, and variables extensively. The same code could have been written using just one or two operators. I prefer this style of programming because it's easy to understand and maintain. I read the code from top to bottom, from left to right. In imperative programming I often deal with many variables, each of them can be changed at any moment. That forces me to tack carefully all variables, I have to read the code up and down many times analysing the same function. It may be surprising, but there are no mutations at all in the code above. Perhaps you're still not convinced that everyday tasks can be written this way. Next, I'll share some real examples from my production code to illustrate. See you then!
