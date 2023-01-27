const fs = require("fs");
const path = require("path");

const { modules, paths } = require("./constants");

const { getFilesByDir, getImagesByDir } = require("./utils");

const { findUselessImages } = require("./uselessImages");
const {
  findImagesWithDifferentMD5ButEqualName,
} = require("./differentMD5EqualName");
const {
  findImagesWithDifferentNameButEqualMD5,
} = require("./differentNameEqualMD5");
const { findImagesWithEqualMD5AndEqualName } = require("./equalMD5EqualName");
const { findWrongImagesImport } = require("./wrongImagesImport");
const { importImgToImageHelper } = require("./importImgToImageHelper");

const runAllTests = async () => {
  const actions = [];

  const images = {};

  modules.forEach((module) => {
    const callback = async () => {
      images[module] = await getImagesByDir(paths[module]);
    };

    actions.push(callback());
  });

  await Promise.all(actions);

  const allImgs = [];

  modules.forEach((module) => {
    allImgs.push(...images[module]);
  });

  const filesActions = [];

  const files = {};

  modules.forEach((module) => {
    const callback = async () => {
      files[module] = await getFilesByDir(paths[module]);
    };
    filesActions.push(callback());
  });

  await Promise.all(filesActions);

  const allFiles = [];

  modules.forEach((module) => {
    allFiles.push(...files[module]);
  });

  const resultDir = path.resolve(__dirname, "..", "result");

  if (!fs.existsSync(resultDir)) {
    fs.mkdirSync(resultDir);
  }

  // findUselessImages(allImgs, allFiles);
  // findImagesWithDifferentMD5ButEqualName(allImgs);
  // findImagesWithDifferentNameButEqualMD5(allImgs);
  // findImagesWithEqualMD5AndEqualName(allImgs);
  findWrongImagesImport(allFiles);
};

runAllTests();

// Add images to img helper from path
// importImgToImageHelper(paths.public + "/images/flags");
