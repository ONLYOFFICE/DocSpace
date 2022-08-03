const { merge } = require("webpack-merge");
const baseConfig = require("./webpack.base.js");
const webpackNodeExternals = require("webpack-node-externals");
const path = require("path");
const DefinePlugin = require("webpack").DefinePlugin;
const TerserPlugin = require("terser-webpack-plugin");

const serverConfig = {
  target: "node",
  name: "server",
  entry: {
    server: "./src/server/index.tsx",
  },

  output: {
    path: path.resolve(process.cwd(), "dist/"),
    filename: "[name].js",
    libraryTarget: "commonjs2",
    chunkFilename: "chunks/[name].js",
  },
  externals: [
    webpackNodeExternals(),
    {
      express: "express",
      bufferutil: "bufferutil",
      "utf-8-validate": "utf-8-validate",
    },
  ],
};

module.exports = (env, argv) => {
  if (argv.mode === "production") {
    serverConfig.mode = "production";
    serverConfig.optimization = {
      minimize: !env.minimize,
      minimizer: [new TerserPlugin()],
    };
  } else {
    serverConfig.mode = "development";
  }
  serverConfig.plugins = [
    new DefinePlugin({
      IS_DEVELOPMENT: argv.mode !== "production",
      PORT: process.env.PORT || 9000,
    }),
  ];

  return merge(baseConfig, serverConfig);
};
