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
  resolve: {
    ...baseConfig.resolve,
  },
  module: {
    ...baseConfig.module,
  },
  output: {
    path: path.resolve(process.cwd(), "dist/server"),
    filename: "[name].js",
    libraryTarget: "commonjs2",
    chunkFilename: "chunks/[name].js",
  },
  externals: [webpackNodeExternals(), { express: "express" }],
};

module.exports = (env, argv) => {
  if (argv.mode === "production") {
    baseConfig.mode = "production";
    baseConfig.optimization = {
      splitChunks: { chunks: "all" },
      minimize: !env.minimize,
      minimizer: [new TerserPlugin()],
    };
    baseConfig.plugins = [
      new DefinePlugin({
        IS_DEVELOPMENT: false,
      }),
    ];
  } else {
    baseConfig.plugins = [
      new DefinePlugin({
        IS_DEVELOPMENT: true,
      }),
    ];
  }

  return merge(baseConfig, serverConfig);
};
