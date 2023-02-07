const { join } = require("path");
const { readdirSync, readFileSync, writeFileSync } = require("fs");
const minifyJson = require("../../packages/common/utils/minifyJson.js");

const localesDir = join(
  __dirname,
  "../../build",
  "deploy",
  "public",
  "locales"
);

//console.log("localesDir", localesDir);

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
    if (filePath.endsWith(".DS_Store")) return;

    let content = readFileSync(filePath);
    writeFileSync(filePath, minifyJson(content, filePath));
    //console.log(`File '${filePath}' minified`);
  } catch (e) {
    console.error("Unable to minify file ", filePath, e);
  }
});
