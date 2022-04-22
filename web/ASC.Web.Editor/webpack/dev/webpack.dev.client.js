const webpack = require("webpack");
const { merge } = require("webpack-merge");
const baseClientConfig = require("../webpack.base.client");
const path = require("path");
const { CleanWebpackPlugin } = require("clean-webpack-plugin");
const pkg = require("../../package.json");
const homepage = pkg.homepage;

const clientConfig = {
  mode: "development",
  entry: {
    client: [
      "webpack-hot-middleware/client?reload=true&noInfo=true",
      "./src/client/index.js",
    ],
  },
  output: {
    devtoolModuleFilenameTemplate: (info) =>
      path.resolve(info.absoluteResourcePath).replace(/\\/g, "/"),
  },
  devtool: "inline-cheap-module-source-map",
  devServer: {
    devMiddleware: {
      publicPath: homepage,
    },
    static: {
      directory: path.join(__dirname, "dist/client"),
      publicPath: homepage,
    },
    port: 5013,
    historyApiFallback: {
      // Paths with dots should still use the history fallback.
      // See https://github.com/facebook/create-react-app/issues/387.
      disableDotRule: true,
      index: homepage,
    },
    hot: true,
    headers: {
      "Access-Control-Allow-Origin": "*",
      "Access-Control-Allow-Methods": "GET, POST, PUT, DELETE, PATCH, OPTIONS",
      "Access-Control-Allow-Headers":
        "X-Requested-With, content-type, Authorization",
    },
  },

  plugins: [new CleanWebpackPlugin(), new webpack.HotModuleReplacementPlugin()],
};

module.exports = merge(baseClientConfig, clientConfig);
