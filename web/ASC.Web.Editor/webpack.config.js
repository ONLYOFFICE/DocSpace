const ModuleFederationPlugin = require("webpack").container
  .ModuleFederationPlugin;
const TerserPlugin = require("terser-webpack-plugin");
const combineUrl = require("@appserver/common/utils/combineUrl");
const AppServerConfig = require("@appserver/common/constants/AppServerConfig");
const sharedDeps = require("@appserver/common/constants/sharedDependencies");

const { proxyURL } = AppServerConfig;

const path = require('path');
const pkg = require('./package.json');
const deps = pkg.dependencies || {};
const homepage = pkg.homepage; // combineUrl(AppServerConfig.proxyURL, pkg.homepage);

const { merge } = require("webpack-merge");
const common = require("./webpack/common");
const { client: clientLoaders } = require("./webpack/loaders");

const config = merge(common, {
  entry: "./src/index",
  mode: "development",

  devServer: {
    devMiddleware: {
      publicPath: homepage,
    },
    static: {
      directory: path.join(__dirname, 'dist'),
      publicPath: homepage,
    },
    port: 5013,
    historyApiFallback: {
      // Paths with dots should still use the history fallback.
      // See https://github.com/facebook/create-react-app/issues/387.
      disableDotRule: true,
      index: homepage,
    },
    hot: false,
    headers: {
      'Access-Control-Allow-Origin': '*',
      'Access-Control-Allow-Methods': 'GET, POST, PUT, DELETE, PATCH, OPTIONS',
      'Access-Control-Allow-Headers': 'X-Requested-With, content-type, Authorization',
    },
  },

  performance: {
    maxEntrypointSize: 512000,
    maxAssetSize: 512000,
  },

  module: {
    rules: clientLoaders,
  },

  plugins: [
    new ModuleFederationPlugin({
      name: 'editor',
      filename: 'remoteEntry.js',
      remotes: {
        studio: `studio@${combineUrl(proxyURL, '/remoteEntry.js')}`,
        files: `files@${combineUrl(proxyURL, '/products/files/remoteEntry.js')}`,
      },
      exposes: {
        './app': './src/Editor.jsx',
      },
      shared: {
        ...deps,
        ...sharedDeps,
      },
    }),
  ],
});

module.exports = (env, argv) => {
  if (argv.mode === 'production') {
    config.mode = 'production';
    config.optimization = {
      splitChunks: { chunks: 'all' },
      minimize: !env.minimize,
      minimizer: [new TerserPlugin()],
    };
  } else {
    config.devtool = 'cheap-module-source-map';
  }

  return config;
};
