const fs = require("fs");
const path = require("path");

const { wrongImportImages } = require("./constants");

const pathToResultFile = path.join(
  __dirname,
  "..",
  "result",
  "WrongImagesImport"
);

const findWrongImagesImport = async (files) => {
  let wrongImports = "";

  files.forEach((file) => {
    const data = fs.readFileSync(file.path, "utf8");

    wrongImportImages.forEach((i) => {
      const idx = data.indexOf(i);

      //   console.log(file.path.indexOf("\\webpack"));

      if (
        idx > 0 &&
        file.fileName.indexOf("webpack") === -1 &&
        file.path.indexOf("common\\utils\\index.ts") === -1 &&
        file.path.indexOf("context-menu\\sub-components\\sub-menu.js") === -1 &&
        file.path.indexOf("drop-down-item\\index.js") === -1 &&
        file.path.indexOf("common\\utils\\index.ts") === -1 &&
        file.path.indexOf(".html") === -1 &&
        file.path.indexOf("storybook-static") === -1
      ) {
        wrongImports = wrongImports + `${file.path}\n`;
      }
    });
  });

  fs.writeFileSync(pathToResultFile, "");

  fs.appendFileSync(pathToResultFile, `${wrongImports}\n`);
};

module.exports = { findWrongImagesImport };
