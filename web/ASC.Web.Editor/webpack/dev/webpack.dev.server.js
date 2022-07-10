const { merge } = require("webpack-merge");
const baseConfig = require("../webpack.base.js");
const webpackNodeExternals = require("webpack-node-externals");
const path = require("path");
const ROOT_DIR = path.resolve(__dirname, "../../");
const resolvePath = (...args) => path.resolve(ROOT_DIR, ...args);
const BUILD_DIR = resolvePath("dist");

const serverConfig = {
  target: "node",
  mode: "development",
  name: "server",
  entry: {
    server: "./src/server/index.js",
  },
  resolve: {
    ...baseConfig.resolve,
  },
  module: {
    ...baseConfig.module,
  },
  output: {
    path: BUILD_DIR,
    filename: "[name].js",
    libraryTarget: "commonjs2",
    chunkFilename: "chunks/[name].js",
  },
  externals: [webpackNodeExternals()],
};

module.exports = merge(baseConfig, serverConfig);
