const { merge } = require("webpack-merge");
const path = require("path");
const baseConfig = require("../webpack.base.js");
const ModuleFederationPlugin = require("webpack").container
  .ModuleFederationPlugin;
const HotModuleReplacementPlugin = require("webpack")
  .HotModuleReplacementPlugin;
const DefinePlugin = require("webpack").DefinePlugin;
const { WebpackManifestPlugin } = require("webpack-manifest-plugin");
const { CleanWebpackPlugin } = require("clean-webpack-plugin");
const combineUrl = require("@appserver/common/utils/combineUrl");
const minifyJson = require("@appserver/common/utils/minifyJson");
const AppServerConfig = require("@appserver/common/constants/AppServerConfig");
const { proxyURL } = AppServerConfig;
const sharedDeps = require("@appserver/common/constants/sharedDependencies");
const ExternalTemplateRemotesPlugin = require("external-remotes-plugin");
const CopyPlugin = require("copy-webpack-plugin");

for (let dep in sharedDeps) {
  sharedDeps[dep].eager = true;
}

const clientConfig = {
  target: "web",
  mode: "development",
  entry: {
    client: ["./src/client/index.js"],
  },
  devtool: "inline-cheap-module-source-map",

  output: {
    path: path.resolve(process.cwd(), "dist"),
    filename: "static/js/[name].[contenthash].bundle.js",
    publicPath: "/products/files/doceditor/",
    chunkFilename: "static/js/[id].[contenthash].js",
  },
  resolve: {
    ...baseConfig.resolve,
  },
  module: {
    ...baseConfig.module,
  },
  plugins: [
    new CleanWebpackPlugin(),
    new ModuleFederationPlugin({
      name: "editor",
      filename: "remoteEntry.js",
      remotes: {
        studio: `studio@${combineUrl(proxyURL, "/remoteEntry.js")}`,
        files: `files@${combineUrl(
          proxyURL,
          "/products/files/remoteEntry.js"
        )}`,
      },
      exposes: {
        "./app": "./src/client/index.js",
      },
      shared: { ...sharedDeps },
    }),
    new ExternalTemplateRemotesPlugin(),
    new CopyPlugin({
      patterns: [
        {
          context: path.resolve(process.cwd(), "public"),
          from: "images/**/*.*",
        },
        {
          context: path.resolve(process.cwd(), "public"),
          from: "locales/**/*.json",
          transform: minifyJson,
        },
      ],
    }),
    new WebpackManifestPlugin(),
    new DefinePlugin({
      IS_DEVELOPMENT: process.env.NODE_ENV === "development",
      PORT: process.env.PORT,
    }),
    new HotModuleReplacementPlugin(),
  ],
  optimization: {
    runtimeChunk: "single", // creates a runtime file to be shared for all generated chunks.
    splitChunks: {
      chunks: "all", // This indicates which chunks will be selected for optimization.
      automaticNameDelimiter: "-",
      cacheGroups: {
        vendor: {
          // to convert long vendor generated large name into vendor.js
          test: /[\\/]node_modules[\\/]/,
          name: "vendor",
          chunks: "all",
        },
      },
    },
    minimize: false,
    minimizer: [],
  },
};

module.exports = merge(baseConfig, clientConfig);
