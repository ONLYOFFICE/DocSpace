const { join } = require("path");
const { readdirSync, readFileSync, writeFileSync } = require("fs");
const minifyJson = require("../../packages/asc-web-common/utils/minifyJson.js");

const localesDir = join(
  __dirname,
  "../../build",
  "deploy",
  "public",
  "locales"
);

const getFileList = (dirName) => {
  let files = [];
  const items = readdirSync(dirName, { withFileTypes: true });

  for (const item of items) {
    if (item.isDirectory()) {
      files = [...files, ...getFileList(`${dirName}/${item.name}`)];
    } else {
      files.push(`${dirName}/${item.name}`);
    }
  }

  return files;
};

const files = getFileList(localesDir);

files.forEach((filePath) => {
  try {
    let content = readFileSync(filePath);
    writeFileSync(filePath, minifyJson(content, filePath));
    //console.log(`File '${filePath}' minified`);
  } catch (e) {
    console.error("Unable to minify file ", filePath, e);
  }
});
