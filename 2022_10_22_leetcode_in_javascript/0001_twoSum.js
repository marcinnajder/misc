// https://leetcode.com/problems/two-sum/

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


