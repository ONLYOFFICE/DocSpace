const { merge } = require("webpack-merge");
const baseServerConfig = require("../webpack.base.server");

const serverConfig = {
  mode: "production",
};

module.exports = merge(baseServerConfig, serverConfig);
