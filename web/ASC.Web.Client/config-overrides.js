const CopyWebpackPlugin = require("copy-webpack-plugin");
const path = require("path");
const { override, babelInclude } = require("customize-cra");

module.exports = (config, env) => {
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

  return Object.assign(
    config,
    override(
      babelInclude([
        /* transpile (converting to es5) code in src/ and shared component library */
        path.resolve("src"),
        path.resolve("../components"),
        path.resolve("../common"),
      ])
    )(config, env)
  );
};
