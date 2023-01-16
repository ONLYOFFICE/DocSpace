const CopyPlugin = require("copy-webpack-plugin");
const path = require("path");

const pathToAssets = path.resolve(__dirname, "../../../public/images");

module.exports = ({ config }) => {
  const rules = config.module.rules;

  const fileLoaderRule = rules.find((rule) => rule.test.test(".svg"));
  fileLoaderRule.exclude = /\.svg$/;

  config.output.assetModuleFilename = (pathData) => {
    //console.log({ pathData });

    let result = pathData.filename
      .substr(pathData.filename.indexOf("public/"))
      .split("/")
      .slice(1);

    result.pop();

    let folder = result.join("/");

    console.log("call " + folder);

    return `${folder}/[name].[contenthash][ext]`; //`${folder}/[name][ext]?hash=[contenthash]`;
  };

  // rules.push({
  //   test: /\.(png|jpe?g|gif|ico)$/i,
  //   type: "asset/resource",
  // });

  rules.push({
    test: /\.svg$/i,
    type: "asset/resource",
    resourceQuery: /url/, // *.svg?url
  });

  rules.push({
    test: /\.svg$/i,
    resourceQuery: { not: [/url/] }, // exclude react component if *.svg?url
    use: [
      {
        loader: "@svgr/webpack",
        options: {
          svgoConfig: {
            plugins: [{ removeViewBox: false }],
          },
        },
      },
    ],
  });

  // config.plugins.push(
  //   new CopyPlugin({
  //     patterns: [
  //       {
  //         from: "../../../public/images",
  //         to: "./images",
  //         toType: "dir",
  //         context: "storybook-static",
  //       },
  //     ],
  //   })
  // );

  return config;
};
