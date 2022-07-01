const webpack = require("webpack");
const { merge } = require("webpack-merge");
const path = require("path");
const baseConfig = require("../webpack.base.js");
const ROOT_DIR = path.resolve(__dirname, "../../");
const resolvePath = (...args) => path.resolve(ROOT_DIR, ...args);
//const BUILD_DIR = resolvePath("dist");
const LoadablePlugin = require("@loadable/webpack-plugin");
const ModuleFederationPlugin = require("webpack").container
  .ModuleFederationPlugin;
const combineUrl = require("@appserver/common/utils/combineUrl");
const AppServerConfig = require("@appserver/common/constants/AppServerConfig");
const { proxyURL } = AppServerConfig;
const sharedDeps = require("@appserver/common/constants/sharedDependencies");
const ExternalTemplateRemotesPlugin = require("external-remotes-plugin");
const CopyPlugin = require("copy-webpack-plugin");
const pkg = require("../../package.json");
const homepage = pkg.homepage; // combineUrl(proxyURL, pkg.homepage);
const BUILD_DIR = path.resolve(process.cwd(), "dist");

for (let dep in sharedDeps) {
  sharedDeps[dep].eager = true;
}
//&path=//localhost:5013/__webpack_hmr
const clientConfig = {
  target: "web",
  mode: "development",
  entry: {
    client: [
      "webpack-hot-middleware/client?reload=true&noInfo=true&path=//localhost:5013/__webpack_hmr",
      "./src/client/index.js",
    ],
  },
  devtool: "inline-cheap-module-source-map",
  devServer: {
    //contentBase: "./dist",
    //compress: true,
    //historyApiFallback: true,
    hot: true,
    historyApiFallback: {
      // Paths with dots should still use the history fallback.
      // See https://github.com/facebook/create-react-app/issues/387.
      disableDotRule: true,
      index: homepage,
    },
    static: {
      directory: path.join(__dirname, "dist"),
      publicPath: homepage,
    },
    devMiddleware: {
      publicPath: homepage,
    },
    port: 5013,
  },
  output: {
    // path: resolvePath(BUILD_DIR, "client"),
    // publicPath: "/client/",
    // filename: "[name].js",
    // chunkFilename: "[name].js",
    // // Point sourcemap entries to original disk location (format as URL on Windows)
    // devtoolModuleFilenameTemplate: (info) =>
    //   path.resolve(info.absoluteResourcePath).replace(/\\/g, "/"),
    // assetModuleFilename: "assets/[hash][ext][query]",

    path: path.resolve(process.cwd(), "dist/client"),
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
    new CopyPlugin({
      patterns: [
        {
          from: "public",
          globOptions: {
            dot: true,
            gitignore: true,
            ignore: ["**/index.html"],
          },
        },
      ],
    }),
    new webpack.HotModuleReplacementPlugin(),
    //loadable plugin will create all the chunks
    new LoadablePlugin({
      outputAsset: false, // to avoid writing loadable-stats in the same output as client
      writeToDisk: true,
      filename: `${BUILD_DIR}/loadable-stats.json`,
    }),
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

    // you can add additional plugins here like BundleAnalyzerPlugin, Copy Plugin etc.
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
