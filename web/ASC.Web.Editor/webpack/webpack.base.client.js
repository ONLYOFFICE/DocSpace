const ModuleFederationPlugin = require("webpack").container
  .ModuleFederationPlugin;
const ExternalTemplateRemotesPlugin = require("external-remotes-plugin");
const CopyPlugin = require("copy-webpack-plugin");
const AppServerConfig = require("@appserver/common/constants/AppServerConfig");
const combineUrl = require("@appserver/common/utils/combineUrl");
const sharedDeps = require("@appserver/common/constants/sharedDependencies");
const path = require("path");
const pkg = require("../package.json");
const LoadablePlugin = require("@loadable/webpack-plugin");
const commonConfig = require("./webpack.common");
const { merge } = require("webpack-merge");
const { proxyURL } = AppServerConfig;
const deps = pkg.dependencies || {};

const BUILD_DIR = path.resolve(process.cwd(), "dist");

const getDeps = () => {
  for (dep in sharedDeps) {
    sharedDeps[dep].eager = true;
  }
  return {
    ...deps,
    ...sharedDeps,
  };
};

const clientBaseConfig = {
  target: "web",

  output: {
    path: path.resolve(process.cwd(), "dist/client"),
    filename: "static/js/[name].[contenthash].bundle.js",
    publicPath: "/products/files/doceditor/",
    chunkFilename: "static/js/[id].[contenthash].js",
  },

  module: {
    rules: [
      {
        test: /\.css$/i,
        use: ["style-loader", "css-loader"],
      },
      {
        test: /\.s[ac]ss$/i,
        use: [
          // Creates `style` nodes from JS strings
          "style-loader",
          // Translates CSS into CommonJS
          {
            loader: "css-loader",
            options: {
              url: {
                filter: (url, resourcePath) => {
                  // resourcePath - path to css file

                  // Don't handle `/static` urls
                  if (url.startsWith("/static") || url.startsWith("data:")) {
                    return false;
                  }

                  return true;
                },
              },
            },
          },
          // Compiles Sass to CSS
          "sass-loader",
        ],
      },
    ],
  },
  resolve: {
    extensions: [".jsx", ".js", ".json"],
    fallback: {
      crypto: false,
    },
  },
  performance: {
    maxEntrypointSize: 512000,
    maxAssetSize: 512000,
  },
  plugins: [
    new LoadablePlugin({
      outputAsset: false,
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
      shared: getDeps(),
    }),
    new ExternalTemplateRemotesPlugin(),
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

module.exports = merge(commonConfig, clientBaseConfig);
