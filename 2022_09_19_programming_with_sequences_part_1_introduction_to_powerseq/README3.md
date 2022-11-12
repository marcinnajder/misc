

## Programming with sequences part 3 - leetcode problems


#### Introduction

In the first part of the series we have started from the beginning by answering a few basic questions: 
- what is an iterator pattern?
- what is the difference between a collection and a sequence?
- what is the generator function in JavaScript?

Then we have implemented operators like `filter`, `map`, `range`, `toarray` and `take`.  Simple `pipe` function allows us to combine many operators into one expression.

In the second part we have been talking about the code pattern what we can find very often. Instead of implementing them manually over and over again, we can implement them once and reuse later . We have introduced the following operators`flatmap`, `distinct`, `count`, `toobject`, `join`, `groupby` and `concat`. 

In this part we will use the knowledge we gained so far to solve few problems from the [leetcode](https://leetcode.com/) web side. In the time of writing this article (the end of 2022), leetcode contains around 2500 small programming task. In general we can implement them in any programming language we want but we try to implement few random problems in JavaScript using functional style. We will be using [powerseq](https://github.com/marcinnajder/powerseq) library introducing a bunch of new operators. Again, we could we using any JavaScipt library containing functions over collections, it does need to be the powerseq. The key point here is to show that the different style of programming exists and brings a lot of value.


#### smallerNumbersThanCurrent 

Let's start with a simple task, we don't even introduce any new operator. We have got an array of numbers and we have to return a new array of numbers with exactly the same length, for the sample array `[8, 1, 2, 2, 3]` it will be an array `[4, 0, 1, 1, 3]`. The algorithm is as follows:
- for any index `i` the value of `output[i]` is the number of items from `input` smaller than `input[i]`
- the first item is `4` because there are `4` numbers smaller than `8` ( `1, 2, 2, 3`)
- the second item is `0` because there are no numbers smaller than `1`
- the same for all remaining numbers

```javascript
var { pipe, count, map, toarray, range } = require("powerseq");

function smallerNumbersThanCurrent(numbers) {
    return pipe(
        numbers,
        map((n, i) => count(range(0, numbers.length), j => i !== j && numbers[j] < n)),
        toarray()
    )
}

smallerNumbersThanCurrent([8, 1, 2, 2, 3]) // -> [4, 0, 1, 1, 3]
smallerNumbersThanCurrent([6, 5, 4, 8]) // -> [ 2, 1, 0, 3 ]
smallerNumbersThanCurrent([7, 7, 7, 7]) // -> [ 0, 0, 0, 0 ]
```

We map `input` array into `output` array. `n` argument of the lambda function is an item, `i` argument is the index of the next item. `range` operator creates the sequence of values `0, 1, 2, ..` for all indexes. `count` operators counts the numer of items smaller than `n`. Just by looking at the names of used operators, we immediately see what is going on.

#### twoSum (find)

The next problem is very similar to previous one. We take an array of numbers `nums` and a `target` number, we have to return the array containing two numbers which are the indexes of two items from the array. The sum of those items should be equal to `target` number. For example, for `nums` equal `[2, 7, 11, 15]` and `target` equal `9`, the result will be `[0,1]` because `nums[0] + nums[1]` equals `2+7=9`.

```javascript
var { find, flatmap, range, pipe, find, } = require("powerseq");

function twoSum(nums, target) {
    return pipe(
        range(0, nums.length),
        flatmap(i => range(0, i), (i, j) => [j, i]),
        find(([j, i]) => nums[i] + nums[j] === target)
    );
}

twoSum([2, 7, 11, 15], 9) // -> [0, 1]
twoSum([3, 2, 4], 6) // -> [ 1, 2 ]
twoSum([3, 3], 6) // -> [ 0, 1 ]
```

We combine two operators `range` and `flatmap` to generate pairs of indexes like `[0,1], [0,2], [1,2], [0,3], [1,3], [2,3], ...`. A new operator `find` returns the first element for which the lambda function returns `true`.

#### firstUniqChar (findIndex, every)

In the next example, we take a string value and return the index of the first unique character, value `-1`  when there is no unique character. For the sample string `"letmein"` it returns `0` because character `"l"` is unique. For `"lifeislovepoem"` it returns `2` because character `"f"` is unique. For `"aabb"` it returns `-1` because there is no unique characters.

```javascript
var { findindex, every, range } = require("powerseq");

function firstUniqChar(s) {
    return findindex(s, (c, i) => every(range(0, s.length), j => i === j || c !== s[j])) ?? -1;
}

firstUniqChar("letmein") // -> 0
firstUniqChar("lifeislovepoem") // -> 2
firstUniqChar("aabb") // -> -1
```

We introduced two new operators. `findIndex` works almost the same as `find` but instead of returning found item, it returns the index the found item. `every` operator returns `true` if all items of the sequence matches passed predicate function, the first item for which the lambda returns `false` stops the execution returning `false` result.

#### firstUniqChar (scan)

This time the function takes two strings and returns `true` if the first string is a subsequence of the second string. `"abc"` string is a subsequence of `"ahbgdc"` because even if we omit few characters in `"ahbgdc"` we can still find `"abc"` characters in that order. `"axc"` is not a subsequence of `"ahbgdc"` string because `"x"` character can not be found in `"ahbgdc"`.

```javascript
var { pipe, scan, every, find, range } = require("powerseq");

function isSubsequence(s, t) {
    return pipe(
        s,
        scan((i, c) => find(range(i + 1, t.length - (i + 1) ), j => t[j] === c) ?? -1, -1),
        every(i => i !== -1)
    );
}

isSubsequence("abc", "ahbgdc"); // -> true
isSubsequence("axc", "ahbgdc"); // -> false
```

`scan` operator is very powerful, it allows to implement more advanced scenarios. powerseq library provides `reduce` operator which works the same as builtin `reduce` method from the `Array` type. `scan` operator is similar to `reduce`. If we execute `reduce(range(1, 3), (prev, curr) => prev + curr, 0)` the value `6` will be returned
	- first time lambda function will be called with `prev=0,curr=1`  and return `0 + 1 = 1`
	- second time lambda function will be called with `prev=1,curr=2` and return `1 + 2 = 3`
	- the finally call will take`prev=3,curr=3` and return `3 + 3 = 6`

If we execute the same code but for the `scan` operator `scan(range(1, 3), (prev, curr) => prev + curr, 0)`, the sequence `1, 2, 6` will be retuned. `reduce` operator is eager, it executes right away returning final result. `scan` operator is lazy, it returns a lazy sequence of intermediate values up to the final result. The customer of the sequence decides how many item should be produced.

Let's try to analyze what exactly happens once we execute `isSubsequence("abc", "ahbgdc")`. The type of `s` parameter is a string and we can think of a string type as a sequence of characters. We call `scan` operator passing a string `s` parameter,  the initial value  `-1`. Lambda takes `c` string parameter which is the next character from the `s` string and the number `i` which is the aggregated value (`-1` at the beginning). The goal of the lambda function is to find the first index of the current `c` character in string `t` (the second parameter of the `isSubsequence` function). The trick here is to not search from the beginning every time, but from the index of the previously found character. The aggregated value is used exactly for that reason. To understand the whole process better, let's pretend that we have replaced `every` operator with `toarray` in the code above. After the changes the execution of `isSubsequence("abc", "ahbgdc")` would return `[ 0, 2, 5 ]`, but the execution of `isSubsequence("axc", "ahbgdc")` would return `[ 0, -1, 5 ]`. Value `-1` would be  returned because `x` character does not exist in string `"ahbgdc"`. `every` operator pulls next indexes from the sequence until the first value `-1` is found or all items has been processed.

#### plusOne (reverse, skipwhile)

To understand next problem we have to remind our self the math from the primary school - the addition of two numbers using written column method. We take a number represented as an array of digits, the number 4322 would be represented as `[4, 3, 2, 2]`. The goal is to add value `1` to the input number and return the result as an array of digits, `[4, 3, 2, 3]` in our case. Depending on the numbers, sometimes we have to carry a value `1` from the right to the left. For example adding `1` to the number `[9]` should return `[1, 0]` so one additional digit has been added to the front. 

```javascript
var { scan, reverse, pipe, toarray, map, concat, skipwhile } = require("powerseq");

function plusOne(digits) {
    return pipe(
        concat([0], digits),
        reverse(),
        scan(([_, carry], digit) =>
            carry === 0 ? [digit, 0] : (digit === 9 ? [0, 1] : [digit + 1, 0]), [0, 1]),
        map(([d, _]) => d),
        reverse(),
        skipwhile(d => d === 0),
        toarray()
    );
}

plusOne([4, 3, 2, 2]) // ->  [ 4, 3, 2, 3 ]
plusOne([1, 2, 3]) // -> [1,2,4]
plusOne([9]) // -> [1, 0]
```

The expression looks scary at first but in reality it's very simple. First, let's take a look at the beginning and the end of the expression. At the beginning, digit `0` is added artificially to the front of the number and all digits are reversed using new `reverse` operator. At the end, a potentially unnecessary digit `0` is removed using `skipwhile` operator, then all digits are reversed back. New operator `skipwhile` starts from the first digit checking whether the digit is equal to `0`, so starting from the first non-zero number all successive numbers will be returned. This way all leading zeros will be skipped.

The most interesting code is located in the middle of the expression,`scan` operator plays a crucial role again. We process subsequent digits one by one storing the carry value (`1` or `0`) as an aggregated value. To be precise, the aggregated value is an array of two numbers, not a single value. The first item of the array is digit actually being processed, the second one a carry value. Just after `scan` operator `map` is used to extracts the first item of the aggregated array which is the digit of final number.

#### numberOfSteps (expand, takewhile)

Another very powerful operator next to `scan` is `expand`. Let's take a look at it in action. We have a positive number and we have to calculate the number of steps in which that will reach the value `0`. In every step the number is decreased in the following way:
- if the number is even, it is divided by 2 
- if the number is odd, it is decrement by 1

For number `14` we will reach `0` in `6` steps: `7, 6, 3, 2, 1, 0`

```javascript
var { pipe, expand, takewhile, count } = require("powerseq");

function numberOfSteps(num) {
    return pipe(
        [num],
        expand(n => n % 2 === 0 ? [n / 2] : [n - 1]),
        takewhile(n => n !== 0),
        count()
    );
}

numberOfSteps(14); // -> 6
numberOfSteps(8); // -> 4
numberOfSteps(123); // -> 12
```

In some sense `expand`operator is the opposite of `reduce`. `reduce` reduces the sequence of values into one single value. `expand` generates sequence of values starting from the single initial value (sometimes many values) . In some other programming languages `reduce` is called `fold` and the `extend` is called `unfold`. The analogies are not 100% accurate but close enough.

First, let's see how exactly`expand` operator works. When we execute the following code `expand([1, 2, 3], n => [n * 10, n * 100])` an infinite sequence of values is created `1, 2 , 3, 10, 100, 20, 200, 30, 300, 100, 1000, 1000, 10000, 200, 2000, 2000, 20000, 300, 3000, 3000, ... `. We can imagine that we have an internal queue initially filled with the values from the sequence, the queue would look like this `3 -> 2 -> 1`. We execute the lambda function for the first item in the queue `1`,  we get back a new sequence of values `[10, 100]` and those items are added to the queue `100 -> 10 -> 3 -> 2`.  `expand` operator is lazy, every time we ask for the next value, next item from the queue is return and passed to the lambda function.

Thanks to the `expand` operator the initial number `14` is converted into the sequence of numbers `14, 7, 6, 3, 2, 1, 0` . We introduced a new operator `takewhile` that returns subsequent of items while the condition is met. In our case we are taking items while there are not `0`. At the end we just count the number of items in sequence.


#### pascalsTriangle (pairwise)

This will be the final problem for today, we will generated [pascal's triangle](https://en.wikipedia.org/wiki/Pascal%27s_triangle) which looks like this:

```
     1
    1 1
   1 2 1
  1 3 3 1
 1 4 6 4 1
1 5 10 10 5 1 
...
```

It is generated from the top to bottom. The first and the last number in each row is always `1`. Other numbers are calculated from the row above as a sum of two upper numbers, upper left and upper right. For instance, the fifth row `1 4 6 4 1`was calculated in the following way `1 (1+3) (3+3) (3+1) 1`. The task is to return the list of specified number of rows, for instance execution of `pascalsTriangle(3)` returns `[ [ 1 ], [ 1, 1 ], [ 1, 2, 1 ] ]`. 

```javascript
var { pipe, take, toarray, map, expand } = require("powerseq");

function pascalsTriangle(rowNumber) {
    return pipe(
        expand([[1]], prev => [[1, ...map(pairwise(prev), ([x, y]) => x + y), 1]]),
        take(rowNumber),
        toarray());
}

pascalsTriangle(0); // -> [ ]
pascalsTriangle(1); // -> [ [ 1 ] ]
pascalsTriangle(2); // -> [ [ 1 ], [ 1, 1 ] ]
```

This implementation shows perfectly the power of declarative programming. Quick look and we immediately see what this function is doing:
- start with the first row `[1]`
- each next row will have the following shape `[1, ...map(pairwise(prev), ([x, y]) => x + y), 1]`
- take only `rowNumber` numbers of rows

`pairwise` operators converts sequence of items into sequence of overlapping pairs, `pairwise([1, 5, 10, 15])` returns `[ [1, 5], [5, 10], [10, 15] ]`.  We could accomplished the same effect using powerseq operator called `buffer` but we have implemented `pairwise` ourself which is the simplified and mire specific version of `buffer`.  We've done it only to remind us that we can always write our own operator.

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

I can hear you are saying "...but there are not a real world examples, there are just the artificial puzzles used sometimes during the job interviews! ".  Really ?? I am not so sure. Just try remind yourself when did you write last time any code processing some data or doing something repeatedly. There is a high probability that you have used many loops, ifs and variables. That the same code could have been written using one or two operators. I like such a code programming because the it is easy to understand and maintain. I read the code from the top to bottom, from the left to right. In imperative programming I have many variable, each of them can be changed in any moment. That forces me to tack carefully all variables, I have to read the code up and down many times analyzing the same function. It's hard to believe, but there are no mutations at all in my code above. Probably you are still not convinced that any day to day task can be written this way, so next to I will present you some real examples from my production code. See you then!