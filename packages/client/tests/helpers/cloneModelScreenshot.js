var path = require("path");
var fs = require("fs");
var async = require("async");

const config = require("../../../../../config/appsettings.json");
const cultures = config.web.cultures.split(",");

function getFiles(dirPath, callback) {
  fs.readdir(dirPath, function (err, files) {
    if (err) return callback(err);

    var filePaths = [];
    async.eachSeries(
      files,
      function (fileName, eachCallback) {
        var filePath = path.join(dirPath, fileName);

        fs.stat(filePath, function (err, stat) {
          if (err) return eachCallback(err);

          if (stat.isDirectory()) {
            getFiles(filePath, function (err, subDirFiles) {
              if (err) return eachCallback(err);

              filePaths = filePaths.concat(subDirFiles);
              eachCallback(null);
            });
          } else {
            if (stat.isFile() && /\.png$/.test(filePath)) {
              filePaths.push(filePath);
            }

            eachCallback(null);
          }
        });
      },
      function (err) {
        callback(err, filePaths);
      }
    );
  });
}

function getClonePath(filePath, culture) {
  const splitFilePath = filePath.split("\\");

  const splitFileName = splitFilePath[splitFilePath.length - 1].split("-");

  splitFileName[0] = culture;

  splitFilePath[splitFilePath.length - 1] = splitFileName.join("-");

  const copyPath = splitFilePath.join("\\");

  return copyPath;
}

function copyFile(filePath) {
  cultures.forEach((culture) => {
    const copyPath = getClonePath(filePath, culture);

    fs.copyFile(filePath, copyPath, (err) => {
      if (err) console.log(err);
    });
  });
}

const pathToModels = path.resolve(__dirname, "../screenshots/translation");

function cloneModelScreenshot() {
  getFiles(pathToModels, function (err, files) {
    if (err) console.log(err);

    files.forEach((file) => {
      copyFile(file);
    });
  });
}

cloneModelScreenshot();
