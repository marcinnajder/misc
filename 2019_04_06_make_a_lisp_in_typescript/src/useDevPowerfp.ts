// to use development version of powerfp library (instead of npm version):

// 1. uncommend js code below

// import * as path from "path";
// initEnv();

// function initEnv() {
//   const Module = require('module');
//   const modulePrefixLength = "powerfp".length;
//   const _require = Module.prototype.require;

//   Module.prototype.require = function (modulePath: string) {
//     if (modulePath.startsWith("powerfp")) {
//       //modulePath = "/Volumes/data/github/powerfp/dist/cjs_es6/src/index";
//       modulePath = path.join("/Volumes/data/github/powerfp/dist/cjs_es6/src/", modulePath.substr(modulePrefixLength));
//     }
//     return _require.call(this, modulePath);
//   };
// }


// 2. add below JSON configuration to tsconfig.json inside 'compilerOptions' section

// "paths": {
//   "powerfp": [
//     "/Volumes/data/github/powerfp/dist/cjs_es6/src/index.d.ts",
//   ],
//   "powerfp/*": [
//     "/Volumes/data/github/powerfp/dist/cjs_es6/src/*",
//   ]
// }





