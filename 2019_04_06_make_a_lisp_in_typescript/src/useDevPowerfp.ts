import * as path from "path";
initEnv();

function initEnv() {
  const Module = require('module');
  const modulePrefixLength = "powerfp".length;
  const _require = Module.prototype.require;

  Module.prototype.require = function (modulePath: string) {
    if (modulePath.startsWith("powerfp")) {
      //modulePath = "/Volumes/data/github/powerfp/dist/cjs_es6/src/index";
      modulePath = path.join("/Volumes/data/github/powerfp/dist/cjs_es6/src/", modulePath.substr(modulePrefixLength));
    }
    return _require.call(this, modulePath);
  };
}

