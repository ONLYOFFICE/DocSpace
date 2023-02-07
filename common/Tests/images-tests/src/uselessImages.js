const fs = require("fs");
const path = require("path");

const { findImagesIntoFiles } = require("./utils");

const pathToResultFile = path.join(__dirname, "..", "result", "UselessImages");

const findUselessImages = async (images, files) => {
  const usedImages = findImagesIntoFiles(files, images);

  const uselessImages = images.filter((img) => {
    if (usedImages.indexOf(img.fileName) === -1) {
      return true;
    }

    return false;
  });

  fs.writeFileSync(pathToResultFile, "");

  uselessImages.forEach((value) => {
    // deleteUselessImages(value.path);
    fs.appendFileSync(pathToResultFile, `${value.path}\n`);
  });
};

deleteUselessImages = (dir) => {
  // console.log(
  //   path.resolve(__dirname, "..", "..", dir.replace("C:\\GitHub\\", ""))
  // );

  fs.unlink(dir, () => {});
};

module.exports = { findUselessImages };
