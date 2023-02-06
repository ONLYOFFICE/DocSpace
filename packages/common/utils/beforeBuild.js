const path = require("path");
const fs = require("fs");
const { readdir } = require("fs").promises;

const appSettings = require("../../../config/appsettings.json");

const beforeBuild = async (pathsToLocales, pathToFile, additionalPath) => {
  async function* getFiles(dir) {
    const dirents = await readdir(dir, { withFileTypes: true });
    for (const dirent of dirents) {
      const res = path.resolve(dir, dirent.name);

      if (dirent.isDirectory()) {
        yield* getFiles(res);
      } else {
        yield { path: res, fileName: dirent.name };
      }
    }
  }

  const getLocalesFiles = async () => {
    const files = [];

    for await (const p of pathsToLocales) {
      for await (const f of getFiles(p)) {
        if (f) files.push(f);
      }
    }

    if (additionalPath) {
      for await (const f of getFiles(additionalPath?.path)) {
        if (f && additionalPath?.files?.indexOf(f?.fileName) > -1)
          files.push(f);
      }
    }

    return files;
  };

  const localesFiles = await getLocalesFiles();

  const cultures = appSettings.web.cultures;

  const collectionByLng = new Map();
  const truthLng = new Map();

  let importString = "\n";

  localesFiles.forEach((file) => {
    const splitPath = file.path.split(path.sep);

    const length = splitPath.length;

    const url = [
      splitPath[length - 3],
      splitPath[length - 2],
      splitPath[length - 1],
    ].join("/");

    const fileName = splitPath[length - 1].split(".")[0];

    let lng = splitPath[length - 2];

    let language = lng == "en-US" || lng == "en-GB" ? "en" : lng;

    if (cultures.indexOf(language) === -1) {
      return;
    }

    const splitted = lng.split("-");

    if (splitted.length == 2 && splitted[0] == splitted[1].toLowerCase()) {
      language = splitted[0];
    }

    truthLng.set(language, language.replace("-", ""));

    language = language.replace("-", "");

    const items = collectionByLng.get(language);

    if (items && items.length > 0) {
      items.push(fileName);
      collectionByLng.set(language, items);
    } else {
      collectionByLng.set(language, [fileName]);
    }

    const alias =
      additionalPath?.files?.indexOf(splitPath[length - 1].toString()) > -1
        ? additionalPath.alias
        : fileName.indexOf("Common") === -1
        ? "ASSETS_DIR"
        : "PUBLIC_DIR";

    importString =
      importString +
      `import ${fileName}${language}Url from "${alias}/${url}?url";\n`;
  });

  let content =
    `//THIS FILE IS AUTO GENERATED\n//DO NOT EDIT AND DELETE IT\n` +
    importString;

  let generalCollection = `\n export const translations = new Map([`;

  collectionByLng.forEach((collection, key) => {
    let collectionString = `\n const ${key} = new Map([`;

    collection.forEach((c, index) => {
      collectionString += `\n["${c}", ${c}${key}Url]`;

      if (index !== collection.length - 1) collectionString += `,`;
    });

    collectionString += `\n]);`;

    content += collectionString;
  });

  truthLng.forEach((col, key) => {
    generalCollection += `\n["${key}", ${col}],`;
  });

  generalCollection += `\n]);`;

  content += generalCollection;

  fs.writeFile(pathToFile, content, "utf8", function (err) {
    if (err) throw new Error(Error);

    console.log("The auto generated translations file has been created!");
  });
};

module.exports = beforeBuild;
