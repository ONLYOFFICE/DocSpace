const { merge } = require("webpack-merge");
const commonConfig = require("./webpack.common");
const path = require("path");
const nodeExternals = require("webpack-node-externals");

const baseServerConfig = {
  target: "node",
  name: "server",
  entry: { server: "./src/index.js" },
  output: {
    path: path.resolve(process.cwd(), "dist"),
    filename: "server.js",
  },

  // externals: [
  //   nodeExternals({
  //     modulesDir: path.resolve(__dirname, "../../../node_modules"),
  //     // allowlist: ["@appserver/common", "@appserver/components"],
  //     //importType: "module",
  //   }),
  // ],
};

module.exports = merge(commonConfig, baseServerConfig);
