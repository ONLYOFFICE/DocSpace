const path = require("path");

module.exports = function replaceImport(originalPath, callingFileName) {
  // This replacement rewrites imports of ui-kit to an import using a relative
  // path pointing at the root folder.
  // This allows to import from the bundled ui-kit using
  //   import { PrimaryButton } from 'ui-kit'
  // instead of
  //   import { PrimaryButton } from '../../..'
  if (originalPath === "ui-kit" && callingFileName.endsWith(".bundlespec.js")) {
    const fromPath = path.dirname(callingFileName);
    const toPath = process.cwd();
    const relativePath = path.relative(fromPath, toPath);
    return relativePath;
  }
  return originalPath;
};
