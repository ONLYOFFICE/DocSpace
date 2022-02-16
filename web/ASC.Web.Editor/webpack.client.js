const ModuleFederationPlugin = require("webpack").container
  .ModuleFederationPlugin;
const ExternalTemplateRemotesPlugin = require("external-remotes-plugin");
const AppServerConfig = require("@appserver/common/constants/AppServerConfig");
const combineUrl = require("@appserver/common/utils/combineUrl");
const sharedDeps = require("@appserver/common/constants/sharedDependencies");
const path = require("path");
const pkg = require("./package.json");
const { proxyURL } = AppServerConfig;
const deps = pkg.dependencies || {};

module.exports = {
  mode: "development",
  entry: {
    client: "./src/client/index.js",
  },
  output: {
    path: path.resolve(__dirname, "dist"),
    //filename: "static/js/[name].js",
    publicPath: "auto",
    filename: "static/scripts/doceditor.[name].js",
  },
  module: {
    rules: [
      {
        test: /\.(png|jpe?g|gif|ico)$/i,
        type: "asset/resource",
        generator: {
          filename: "static/images/[hash][ext][query]",
        },
      },
      {
        test: /\.m?js/,
        type: "javascript/auto",
        resolve: {
          fullySpecified: false,
        },
      },
      {
        test: /\.react.svg$/,
        use: [
          {
            loader: "@svgr/webpack",
            options: {
              svgoConfig: {
                plugins: [{ removeViewBox: false }],
              },
            },
          },
        ],
      },
      { test: /\.json$/, loader: "json-loader" },
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

      {
        test: /\.(js|jsx)$/,
        exclude: /node_modules/,
        use: [
          {
            loader: "babel-loader",
            options: {
              presets: ["@babel/preset-react", "@babel/preset-env"],
              plugins: [
                "@babel/plugin-transform-runtime",
                "@babel/plugin-proposal-class-properties",
                "@babel/plugin-proposal-export-default-from",
              ],
            },
          },
          "source-map-loader",
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
  plugins: [
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
        "./app": "./src/client.js",
      },
      shared: {
        ...deps,
        ...sharedDeps,
      },
    }),
    new ExternalTemplateRemotesPlugin(),
  ],
};
