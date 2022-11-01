var { takewhile, find, every, count, scan, reverse, flatmap, range, pipe, take, toarray, map, expand, find, concat, skipwhile, findindex, reduce } = require("powerseq");

// function equals(value1, value2) {
//     return JSON.stringify(value1) === JSON.stringify(value2);
// }

function distinct(...values) {
    return [...new Set(values.map(JSON.stringify))].map(JSON.parse);
}




// *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** 
// https://leetcode.com/problems/two-sum/

function twoSum(nums, target) {
    return pipe(
        range(0, nums.length),
        flatmap(i => range(0, i), (i, j) => [j, i]),
        find(([j, i]) => nums[i] + nums[j] === target)
    );
}

function twoSum_(nums, target) {
    for (var i = 1; i < nums.length; ++i) {
        const item = nums[i];
        for (var j = 0; j < i; ++j) {
            if (item + nums[j] == target) {
                return [j, i];
            }
        }
    }
}

twoSum([2, 7, 11, 15], 9) // -> [0, 1]
twoSum([3, 2, 4], 6) // -> [ 1, 2 ]
twoSum([3, 3], 6) // -> [ 0, 1 ]


// *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** 


function plusOne(digits) {
    return pipe(
        concat([0], digits),
        reverse(),
        scan(([_, carry], digit) => carry === 0 ? [digit, 0] : (digit === 9 ? [0, 1] : [digit + 1, 0]), [0, 1]),
        map(([d, _]) => d),
        reverse(),
        skipwhile(d => d === 0),
        toarray()
    );
}

function plusOne_(digits) {
    var result = new Array(digits.length);
    var [newDigit, carry] = [0, 1];

    for (var i = digits.length - 1; i >= 0; i--) {
        var digit = digits[i];
        ([newDigit, carry] = carry === 0 ? [digit, 0] : (digit === 9 ? [0, 1] : [digit + 1, 0]));
        result[i] = newDigit;
    }

    if (carry === 1) {
        result.unshift(1);
    }

    return result;
}



plusOne_([4, 3, 2, 2]) // ->  [ 4, 3, 2, 3 ]
plusOne_([1, 2, 3]) // -> [1,2,4]
plusOne_([9]) // -> [1, 0]




// *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** 

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

function pascalsTriangle(rowNumber) {
    return pipe(
        expand([[1]], prev => [[1, ...map(pairwise(prev), ([x, y]) => x + y), 1]]),
        take(rowNumber),
        toarray());
}

function pascalsTriangle_(rowNumber) {
    if (rowNumber < 1) {
        return [];
    }

    let row = [1];
    const result = [row];

    if (rowNumber === 1) {
        return result;
    }

    for (let i = 1; i < rowNumber; i++) {
        let newRow = [1];
        for (let j = 0; j < row.length - 1; j++) {
            newRow.push(row[j] + row[j + 1]);
        }
        newRow.push(1);

        row = newRow;
        result.push(row);
    }

    return result;
}



pascalsTriangle(0); // -> [ ]
pascalsTriangle(1); // -> [ [ 1 ] ]
pascalsTriangle(2); // -> [ [ 1 ], [ 1, 1 ] ]
pascalsTriangle(3); // -> [ [ 1 ], [ 1, 1 ], [ 1, 2, 1 ] ]
pascalsTriangle(4); // -> [ [ 1 ], [ 1, 1 ], [ 1, 2, 1 ], [ 1, 3, 3, 1 ] ]
pascalsTriangle(5); // -> [ [ 1 ], [ 1, 1 ], [ 1, 2, 1 ], [ 1, 3, 3, 1 ], [ 1, 4, 6, 4, 1 ] ]



// *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** 

function smallerNumbersThanCurrent(numbers) {
    return pipe(
        numbers,
        map((n, i) => count(range(0, numbers.length), j => i !== j && numbers[j] < n)),
        toarray()
    )
}

smallerNumbersThanCurrent([8, 1, 2, 2, 3]) // -> [4,0,1,1,3]
smallerNumbersThanCurrent([6, 5, 4, 8]) // -> [ 2, 1, 0, 3 ]
smallerNumbersThanCurrent([7, 7, 7, 7]) // -> [ 0, 0, 0, 0 ]



// *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** 

function firstUniqChar(s) {
    return findindex(s, (c, i) => every(range(0, s.length), j => i === j || c !== s[j])) ?? -1;
}

firstUniqChar("letmein") // -> 0
firstUniqChar("lifeislovepoem") // -> 2
firstUniqChar("aabb") // -> -1




// *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** 

function isSubsequence(s, t) {
    return pipe(
        s,
        scan((i, c) => find(range(i, t.length - i), j => t[j] === c) ?? -1, 0),
        every(i => i !== -1)
    );
}

isSubsequence("abc", "ahbgdc"); // -> true
isSubsequence("axc", "ahbgdc"); // -> false




// *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** 

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





