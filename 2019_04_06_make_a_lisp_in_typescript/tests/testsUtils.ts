
export async function executeTests(tests: { [testName: string]: Function }) {
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