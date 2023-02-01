const fs = require("fs");

const { imageHelperPath } = require("./constants");

const {
  getImagesByDir,
  generateImgImport,
  changeImgName,
  changeImgPath,
  generateImgCollection,
} = require("./utils");

const importImgToImageHelper = async (dir) => {
  const images = await getImagesByDir(dir);

  let importContent = "";
  let collectionContent = "";

  const imgCollection = new Map();

  images.forEach((i) => {
    const { imgPath, size } = changeImgPath(i.path);
    const imgName = changeImgName(i.fileName, size);

    const importString = generateImgImport(i.fileName, imgName, imgPath, false);

    importContent += `${importString}\n`;

    if (!!imgCollection.get(size)) {
      imgCollection.set(size, [
        ...imgCollection.get(size),
        { varName: imgName, imgName: i.fileName },
      ]);
    } else {
      imgCollection.set(size, [{ varName: imgName, imgName: i.fileName }]);
    }
  });

  imgCollection.forEach((collection, key) => {
    const collectionString = generateImgCollection(key, collection);

    collectionContent += `${collectionString}\n`;
  });

  fs.readFile(imageHelperPath, function (err, result) {
    if (err) return console.log(err);

    const newContent = importContent + "\n" + result + "\n" + collectionContent;

    // console.log(newContent);

    fs.writeFile(imageHelperPath, newContent, "utf8", function (err) {
      if (err) return console.log(err);

      console.log("The file has been saved!");
    });
  });
};

module.exports = { importImgToImageHelper };
