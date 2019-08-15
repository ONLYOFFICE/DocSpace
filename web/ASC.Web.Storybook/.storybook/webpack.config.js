const path = require("path");

module.exports = {
  resolve: {
    alias: {
      "styled-components": path.resolve(
        __dirname,
        "../node_modules",
        "styled-components"
      )
    }
  },
  module: {
    rules: [
      {
        test: /\.stories\.js?$/,
        loaders: [require.resolve("@storybook/addon-storysource/loader")],
        enforce: "pre"
      }
    ]
  }
};