const ModuleFederationPlugin = require("webpack").container
  .ModuleFederationPlugin;
const ExternalTemplateRemotesPlugin = require("external-remotes-plugin");
const TerserPlugin = require("terser-webpack-plugin");
const CopyPlugin = require("copy-webpack-plugin");

const AppServerConfig = require("@appserver/common/constants/AppServerConfig");
const combineUrl = require("@appserver/common/utils/combineUrl");
const sharedDeps = require("@appserver/common/constants/sharedDependencies");
const path = require("path");
const pkg = require("./package.json");
const { proxyURL } = AppServerConfig;
const deps = pkg.dependencies || {};

for (dep in sharedDeps) {
  sharedDeps[dep].eager = true;
}

const config = {
  target: "web",
  mode: "development",
  entry: "./src/client/index.js",

  output: {
    path: path.resolve(process.cwd(), "clientBuild"),
    filename: "static/js/[name].[contenthash].bundle.js",
    publicPath: "/products/files/doceditor/",
    chunkFilename: "static/js/[id].[contenthash].js",
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
  performance: {
    maxEntrypointSize: 512000,
    maxAssetSize: 512000,
  },
  plugins: [
    //new CleanWebpackPlugin(),
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
      shared: {
        ...deps,
        ...sharedDeps,
      },
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
};

module.exports = (env, argv) => {
  if (argv.mode === "production") {
    config.mode = "production";
    config.optimization = {
      splitChunks: { chunks: "all" },
      minimize: !env.minimize,
      minimizer: [new TerserPlugin()],
    };
  } else {
    config.devtool = "cheap-module-source-map";
  }

  return config;
};
