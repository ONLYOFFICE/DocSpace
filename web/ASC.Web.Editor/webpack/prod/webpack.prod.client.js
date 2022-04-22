const { merge } = require("webpack-merge");
const TerserPlugin = require("terser-webpack-plugin");
const baseClientConfig = require("../webpack.base.client");

const clientConfig = {
  mode: "production",
  entry: { client: "./src/client/index.js" },
  devtool: false,

  optimization: {
    minimize: true,
    minimizer: [new TerserPlugin()],
  },
};

module.exports = merge(baseClientConfig, clientConfig);
