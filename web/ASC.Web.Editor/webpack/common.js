const resolvers = require("./resolvers");
const path = require("path");
const plugins = require("./plugins");

module.exports = {
  target: "web",
  output: {
    path: path.resolve(process.cwd(), "dist"),
    filename: "static/js/[name].[contenthash].bundle.js",
    publicPath: "auto",
    chunkFilename: "static/js/[id].[contenthash].js",
    //assetModuleFilename: "static/images/[hash][ext][query]",
  },
  resolve: { ...resolvers },
  plugins: [...plugins.client],
};
