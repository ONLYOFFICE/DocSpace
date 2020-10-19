const CopyWebpackPlugin = require("copy-webpack-plugin");
const path = require("path");
module.exports = (config) => {
  config.plugins.push(
    new CopyWebpackPlugin([
      {
        from: path.join(
          "src",
          path.sep,
          "components",
          path.sep,
          "**",
          path.sep,
          "locales",
          path.sep,
          "**"
        ),
        to: "locales",
        transformPath(targetPath) {
          const reversedArrayOfFolders = path
            .dirname(targetPath)
            .split(path.sep)
            .reverse();
          const localePath = reversedArrayOfFolders.pop();
          const finalPath = path.join(
            path.sep,
            localePath,
            path.sep,
            reversedArrayOfFolders[2],
            path.sep,
            reversedArrayOfFolders[0],
            path.sep,
            path.basename(targetPath)
          );
          return finalPath;
        },
      },
    ])
  );

  return config;
};
