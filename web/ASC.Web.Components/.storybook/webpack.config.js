const path = require("path");
const webpack = require('webpack'); //to access built-in plugins
/*
Code reference:
  https://storybook.js.org/docs/configurations/custom-webpack-config/#full-control-mode
*/

// Export a function. Accept the base config as the only param.
module.exports = async ({ config, mode }) => {
  // `mode` has a value of 'DEVELOPMENT' or 'PRODUCTION'
  // You can change the configuration based on that.
  // 'PRODUCTION' is used when building the static version of storybook.

  /* ********* SASS *********
  If you want to work with sass, enable the following module:

  config.module.rules.push({
    test: /\.scss$/,
    loaders: [
      'style-loader',
      'css-loader',
      {
        loader: 'postcss-loader',
        options: { plugins: () => [require('autoprefixer')()] }
      },
      'sass-loader'
    ],
    include: path.resolve(__dirname, '../')
  });


  and don't forget to install as devDependencies:

  - style-loader
  - css-loader
  - postcss-loader
  - autoprefixer
  */

  /* resolver to make story panel work */
  config.resolve.alias = {
    "styled-components": path.resolve(
      __dirname,
      "../node_modules",
      "styled-components"
    )
  };

  config.module.rules.push({
    test: /\.stories.js?$/,
    loaders: [require.resolve('@storybook/addon-storysource/loader')],
    enforce: 'pre',
  });

  config.module.rules.push({
    test: /\.react.svg$/,
    loader: 'svg-inline-loader',
  });

  config.plugins.push(
    new webpack.ProvidePlugin({
        "React": "react",
    }));

  // Return the altered config
  return config;
};
