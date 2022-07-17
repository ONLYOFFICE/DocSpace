const { merge } = require("webpack-merge");
const path = require("path");
const ModuleFederationPlugin = require("webpack").container
  .ModuleFederationPlugin;
const DefinePlugin = require("webpack").DefinePlugin;
const { WebpackManifestPlugin } = require("webpack-manifest-plugin");
const { CleanWebpackPlugin } = require("clean-webpack-plugin");
const ExternalTemplateRemotesPlugin = require("external-remotes-plugin");
const CopyPlugin = require("copy-webpack-plugin");
const TerserPlugin = require("terser-webpack-plugin");
const combineUrl = require("@appserver/common/utils/combineUrl");
const minifyJson = require("@appserver/common/utils/minifyJson");
const AppServerConfig = require("@appserver/common/constants/AppServerConfig");
const { proxyURL } = AppServerConfig;
const sharedDeps = require("@appserver/common/constants/sharedDependencies");
const baseConfig = require("./webpack.base.js");

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
    path: path.resolve(process.cwd(), "dist/client"),
    filename: "static/js/[name].[contenthash].bundle.js",
    publicPath: "/products/files/doceditor/",
    chunkFilename: "static/js/[id].[contenthash].js",
  },

  performance: {
    maxEntrypointSize: 512000,
    maxAssetSize: 512000,
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
  ],
};

module.exports = (env, argv) => {
  if (argv.mode === "production") {
    clientConfig.mode = "production";
    clientConfig.optimization = {
      //   splitChunks: { chunks: "all" },
      minimize: !env.minimize,
      minimizer: [new TerserPlugin()],
    };
    clientConfig.plugins = [
      ...clientConfig.plugins,
      new DefinePlugin({
        IS_DEVELOPMENT: false,
        PORT: process.env.PORT || 5013,
      }),
    ];
  } else {
    clientConfig.devtool = "cheap-module-source-map";
    clientConfig.plugins = [
      ...clientConfig.plugins,
      new DefinePlugin({
        IS_DEVELOPMENT: true,
        PORT: process.env.PORT || 5013,
      }),
    ];
  }

  return merge(baseConfig, clientConfig);
};
