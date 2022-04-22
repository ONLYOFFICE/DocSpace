const { merge } = require("webpack-merge");
const baseServerConfig = require("../webpack.base.server");
const path = require("path");

const serverConfig = {
  mode: "development",
  output: {
    devtoolModuleFilenameTemplate: (info) =>
      path.resolve(info.absoluteResourcePath).replace(/\\/g, "/"),
  },
};

module.exports = merge(baseServerConfig, serverConfig);
