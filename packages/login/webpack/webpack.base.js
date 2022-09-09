const FilterWarningsPlugin = require("webpack-filter-warnings-plugin");

const scriptExtensions = /\.(tsx|ts|js|jsx|mjs)$/;
const imageExtensions = /\.(bmp|gif|jpg|jpeg|png)$/;
const fontsExtension = /\.(eot|otf|ttf|woff|woff2)$/;

module.exports = {
  resolve: {
    extensions: [".js", ".jsx", ".json", ".ts", ".tsx"],
    fallback: {
      crypto: false,
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
        test: /\.react.svg$/,
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
  ],
};
