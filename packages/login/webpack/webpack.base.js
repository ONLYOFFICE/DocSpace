const path = require("path");
const PrebuildWebpackPlugin = require("prebuild-webpack-plugin");
const FilterWarningsPlugin = require("webpack-filter-warnings-plugin");
const beforeBuild = require("@docspace/common/utils/beforeBuild");

const scriptExtensions = /\.(tsx|ts|js|jsx|mjs)$/;
const imageExtensions = /\.(bmp|gif|jpg|jpeg|png)$/;
const fontsExtension = /\.(eot|otf|ttf|woff|woff2)$/;

module.exports = {
  resolve: {
    extensions: [".js", ".jsx", ".json", ".ts", ".tsx"],
    fallback: {
      crypto: false,
    },
    alias: {
      PUBLIC_DIR: path.resolve(__dirname, "../../../public"),
      ASSETS_DIR: path.resolve(__dirname, "../public"),
      SRC_DIR: path.resolve(__dirname, "../src"),
      PACKAGE_FILE: path.resolve(__dirname, "../package.json"),
    },
  },
  module: {
    rules: [
      {
        test: scriptExtensions,
        exclude: /node_modules/,
        use: [
          {
            loader: "babel-loader",
            options: {
              presets: [
                "@babel/preset-react",
                "@babel/preset-env",
                "@babel/preset-typescript",
              ],
              plugins: [
                "@babel/plugin-transform-runtime",
                "@babel/plugin-proposal-class-properties",
                "@babel/plugin-proposal-export-default-from",
                ["styled-components", { ssr: true }],
              ],
            },
          },
        ],
      },
      {
        test: fontsExtension,
        type: "asset",
      },
      {
        test: /\.(png|jpe?g|gif|ico)$/i,
        type: "asset/resource",
      },
      {
        test: /\.svg$/i,
        type: "asset/resource",
        resourceQuery: /url/, // *.svg?url
      },
      {
        test: /\.react.svg$/,
        issuer: /\.[jt]sx?$/,
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
      },
      {
        test: imageExtensions,
        type: "asset/resource",
      },
    ],
  },
  plugins: [
    //ignore the drivers you don't want. This is the complete list of all drivers -- remove the suppressions for drivers you want to use.
    new FilterWarningsPlugin({
      exclude: [/Critical dependency/],
    }),
    new PrebuildWebpackPlugin({
      build: async (compiler, compilation, matchedFiles) => {
        const error = await beforeBuild(
          [
            path.join(__dirname, "../public/locales"),
            path.join(__dirname, "../../../public/locales"),
          ],
          path.join(__dirname, "../src/translations.ts")
        );

        if (error) {
          throw new Error(error);
        }
      },
    }),
  ],
};
