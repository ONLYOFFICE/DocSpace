const fs = require("fs");
const path = require("path");

const pathToResultFile = path.join(
  __dirname,
  "..",
  "result",
  "DifferentNameEqualMD5"
);

const findImagesWithDifferentNameButEqualMD5 = (images) => {
  const uniqueImg = new Map();

  images.forEach((i) => {
    const oldImg = uniqueImg.get(i.md5Hash);

    if (oldImg) {
      let skip = false;

      oldImg.forEach((oi) => (skip = skip || oi.fileName === i.fileName));

      if (!skip) {
        const newImg = [...oldImg, i];

        uniqueImg.set(i.md5Hash, newImg);
      }
    } else {
      uniqueImg.set(i.md5Hash, [i]);
    }
  });

  fs.writeFileSync(pathToResultFile, "");

  uniqueImg.forEach((value, key) => {
    if (value.length > 1) {
      let content = `${key}:\n`;

      value.forEach((v) => (content += `${v.path}\n`));

      fs.appendFileSync(pathToResultFile, content + `\n`);
    }
  });
};

module.exports = { findImagesWithDifferentNameButEqualMD5 };
