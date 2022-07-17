const { merge } = require("webpack-merge");
const baseConfig = require("./webpack.base.js");
const webpackNodeExternals = require("webpack-node-externals");
const TerserPlugin = require("terser-webpack-plugin");
const path = require("path");
const DefinePlugin = require("webpack").DefinePlugin;

const serverConfig = {
  target: "node",
  mode: "development",
  name: "server",
  entry: {
    server: "./src/server/index.js",
  },

  output: {
    path: path.resolve(process.cwd(), "dist/"),
    filename: "[name].js",
    libraryTarget: "commonjs2",
    chunkFilename: "chunks/[name].js",
  },
  externals: [webpackNodeExternals(), { express: "express" }],
};

module.exports = (env, argv) => {
  if (argv.mode === "production") {
    serverConfig.plugins = [
      new DefinePlugin({
        IS_DEVELOPMENT: false,
        PORT: process.env.PORT || 5013,
      }),
    ];
  } else {
    serverConfig.plugins = [
      new DefinePlugin({
        IS_DEVELOPMENT: true,
        PORT: process.env.PORT || 5013,
      }),
    ];
  }

  return merge(baseConfig, serverConfig);
};
