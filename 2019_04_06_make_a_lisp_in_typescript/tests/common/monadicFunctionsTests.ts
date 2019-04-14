import * as assert from "assert";
import { forM, replicateM, liftM, liftM2, filterM, reduceM, mapRS } from "../../src/utils/monadicFunctions";
import { some, none, Option } from "../../src/utils/option";
import { promiseMonadOps, optionMonadOps } from "../../src/utils/monadImplementations";

// function guard<T, P extends Option<T>["type"]>(option: Option<T>, type: P) {

//   if (option.type === type) {
//     return option;
//   }
//   // return null;
// }


executeTests();
async function executeTests() {
  const tests = {
    forM_with_nonempty_array_and_some_option,
    forM_with_empty_array_and_some_option,
    forM_with_nonempty_array_and_none_option,
    forM_with_nonempty_array_and_resolved_promise,
    forM_with_empty_array_and_resolved_promise,
    forM_with_nonempty_array_and_rejescted_promise,

    replicateM_with_zero_iteration_and_some_option,
    replicateM_with_two_iterations_and_some_option,
    replicateM_with_two_iterations_and_none_option,

    liftM_with_some_option,
    liftM2_with_some_option,

    filterM_with_some_option,
    filterM_with_none_option,

    reduceM_with_some_option,
    reduceM_with_none_option,
  };

  for (const [testName, testFunc] of Object.entries(tests)) {
    try {
      console.log(testName);
      await Promise.resolve(testFunc());
      console.log(" SUCCEEDED ");
    } catch (err) {
      console.error(" FAILED", err);
    }
  }
}

// forM + Option<T>
function forM_with_nonempty_array_and_some_option() {
  const result = forM([1, 2], item => some(item.toString()), optionMonadOps) as Option<string[]>;

  assert.equal(result.type === "some", true);
  assert.deepEqual(result.type === "some" && result.value, ["1", "2"]);
}
function forM_with_empty_array_and_some_option() {
  const result = forM([], (item: number) => some(item.toString()), optionMonadOps) as Option<string[]>;

  assert.equal(result.type === "some", true);
  assert.deepEqual(result.type === "some" && result.value, []);
}
function forM_with_nonempty_array_and_none_option() {
  const result = forM([1, 2], item => item > 1 ? none() : some(item.toString()), optionMonadOps) as Option<string[]>;

  assert.equal(result.type === "none", true);
}

// forM + Promise<T>
async function forM_with_nonempty_array_and_resolved_promise() {
  const result = await (forM([1, 2], async item => {
    await setTimeotPromise(10);
    return item.toString();
  }, promiseMonadOps) as Promise<string[]>);

  assert.deepEqual(result, ["1", "2"]);
}
async function forM_with_empty_array_and_resolved_promise() {
  const result = await (forM([], async (item: number) => {
    await setTimeotPromise(10);
    return item.toString();
  }, promiseMonadOps) as Promise<string[]>);

  assert.deepEqual(result, []);
}
async function forM_with_nonempty_array_and_rejescted_promise() {
  try {
    const result = await (forM([1, 2], async (item: number) => {
      await setTimeotPromise(10).then(_ => Promise.reject("rejected promise"));
      return item.toString();
    }, promiseMonadOps) as Promise<string[]>);

    assert.fail("Promise should me rejected.");
  }
  catch (err) {
    assert.equal(err, "rejected promise");
  }
}

// replicateM + Some<T>
function replicateM_with_zero_iteration_and_some_option() {
  const result = replicateM(0, some(2), optionMonadOps) as Option<number[]>;
  assert.equal(result.type === "some", true);
  assert.deepEqual(result.type === "some" && result.value, []);
}
function replicateM_with_two_iterations_and_some_option() {
  const result = replicateM(2, some(2), optionMonadOps) as Option<number[]>;
  assert.equal(result.type === "some", true);
  assert.deepEqual(result.type === "some" && result.value, [2, 2]);
}
function replicateM_with_two_iterations_and_none_option() {
  const result = replicateM(2, none(), optionMonadOps) as Option<number[]>;
  assert.equal(result.type === "none", true);
}


// liftM + Some<T>
function liftM_with_some_option() {
  const func = liftM(n => n.toString(), optionMonadOps);
  const result = func(some(2)) as Option<string>

  assert.equal(result.type === "some", true);
  assert.deepEqual(result.type === "some" && result.value, "2");
}

function liftM2_with_some_option() {
  const func = liftM2((n1: number, n2: number) => (n1 + n2).toString(), optionMonadOps);
  const result = func(some(2), some(1)) as Option<string>

  assert.equal(result.type === "some", true);
  assert.deepEqual(result.type === "some" && result.value, "3");
}

// filterM + Promise<T>

function filterM_with_some_option() {
  const result = filterM([1, 2, 3, 4, 5], item => some(item % 2 === 0), optionMonadOps) as Option<number[]>;
  assert.equal(result.type === "some", true);
  assert.deepEqual(result.type === "some" && result.value, [2, 4]);
}

function filterM_with_none_option() {
  const result = filterM([1, 2, 3, 4, 5], item => none(), optionMonadOps) as Option<number[]>;
  assert.equal(result.type === "none", true);
}


function reduceM_with_some_option() {
  const result = reduceM([1, 2, 3], (prev, item) => some(prev + item), 0, optionMonadOps) as Option<number>;

  assert.equal(result.type === "some", true);
  assert.deepEqual(result.type === "some" && result.value, 1 + 2 + 3);
}

function reduceM_with_none_option() {
  const result = reduceM([1, 2, 3], (prev, item) => none(), 0, optionMonadOps) as Option<number>;

  assert.equal(result.type === "none", true);
}








function setTimeotPromise(timeout: number) {
  return new Promise<void>(function (resolve, reject) {
    setTimeout(() => {
      resolve();
    }, timeout);
  });
}

// const result = forM([1, 2, 3, 4, 5], item => {
//   return item < 2 ? some(item.toString()) : none();
// }, optionMonadOps);

// // getElementsByTagName<K extends keyof HTMLElementTagNameMap>(qualifiedName: K): HTMLCollectionOf<HTMLElementTagNameMap[K]>;



// console.log(result);


