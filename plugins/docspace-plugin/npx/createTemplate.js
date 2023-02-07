import * as fs from "fs";

const CURR_DIR = process.cwd();

function createTemplate(template, name, pluginName) {
  const copyActions = [];

  const copyTemplate = async (templatePath, newProjectPath) => {
    const filesToCreate = fs.readdirSync(templatePath);

    filesToCreate.forEach((file) => {
      const origFilePath = `${templatePath}/${file}`;

      // get stats about the current file
      const stats = fs.statSync(origFilePath);

      if (stats.isFile()) {
        copyActions.push(copyFile(file, origFilePath, newProjectPath));
      } else if (stats.isDirectory()) {
        fs.mkdirSync(`${CURR_DIR}/${newProjectPath}/${file}`);

        // recursive call
        copyTemplate(`${templatePath}/${file}`, `${newProjectPath}/${file}`);
      }
    });
  };

  const copyFile = async (file, origFilePath, newProjectPath) => {
    const contents = fs.readFileSync(origFilePath, "utf8");

    const writePath = `${CURR_DIR}/${newProjectPath}/${file}`;

    if (file === "package.json") {
      const newContents = JSON.parse(contents);

      newContents.name = name;

      fs.writeFileSync(writePath, JSON.stringify(newContents, null, 2), "utf8");
    } else if (file === "webpack.config.js") {
      const newContents = contents.replace("default.js", `${pluginName}.js`);

      fs.writeFileSync(writePath, newContents, "utf8");
    } else if (file === "index.ts") {
      const pluginInstance = pluginName.toLowerCase();

      const newContents = contents
        .replaceAll("ChangedName", `${pluginName}`)
        .replaceAll("pluginInstance", `${pluginInstance}`);

      fs.writeFileSync(writePath, newContents, "utf8");
    } else {
      fs.writeFileSync(writePath, contents, "utf8");
    }
  };

  copyTemplate(template, name);

  return Promise.all(copyActions);
}

export default createTemplate;
