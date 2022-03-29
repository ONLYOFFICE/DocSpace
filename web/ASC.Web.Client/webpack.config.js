const { CleanWebpackPlugin } = require("clean-webpack-plugin");
const CopyPlugin = require("copy-webpack-plugin");
const HtmlWebpackPlugin = require("html-webpack-plugin");
const ModuleFederationPlugin = require("webpack").container
  .ModuleFederationPlugin;
const DefinePlugin = require("webpack").DefinePlugin;
const ExternalTemplateRemotesPlugin = require("external-remotes-plugin");
const TerserPlugin = require("terser-webpack-plugin");
const combineUrl = require("@appserver/common/utils/combineUrl");
const AppServerConfig = require("@appserver/common/constants/AppServerConfig");
const sharedDeps = require("@appserver/common/constants/sharedDependencies");

const path = require("path");
const pkg = require("./package.json");
const deps = pkg.dependencies || {};
const homepage = pkg.homepage; //combineUrl(AppServerConfig.proxyURL, pkg.homepage);
const title = pkg.title;
const version = pkg.version;

const config = {
  entry: "./src/index",
  target: "web",
  mode: "development",

  devServer: {
    devMiddleware: {
      publicPath: homepage,
    },
    static: {
      directory: path.join(__dirname, "dist"),
      publicPath: homepage,
    },
    client: {
      logging: "info",
      // Can be used only for `errors`/`warnings`
      //
      // overlay: {
      //   errors: true,
      //   warnings: true,
      // }
      overlay: {
        warnings: false,
      },
      progress: true,
    },
    port: 5001,
    historyApiFallback: {
      // Paths with dots should still use the history fallback.
      // See https://github.com/facebook/create-react-app/issues/387.
      disableDotRule: true,
      index: homepage,
    },
    hot: false,
    headers: {
      "Access-Control-Allow-Origin": "*",
      "Access-Control-Allow-Methods": "GET, POST, PUT, DELETE, PATCH, OPTIONS",
      "Access-Control-Allow-Headers":
        "X-Requested-With, content-type, Authorization",
    },

    open: {
      target: [`http://localhost:8092`],
      app: {
        name: "google-chrome",
        arguments: ["--incognito", "--new-window"],
      },
    },
  },
  resolve: {
    extensions: [".jsx", ".js", ".json"],
    fallback: {
      crypto: false,
    },
  },

  output: {
    publicPath: "auto",
    chunkFilename: "static/js/[id].[contenthash].js",
    //assetModuleFilename: "static/images/[hash][ext][query]",
    path: path.resolve(process.cwd(), "dist"),
    filename: "static/js/[name].[contenthash].bundle.js",
  },

  performance: {
    maxEntrypointSize: 512000,
    maxAssetSize: 512000,
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

  plugins: [
    new CleanWebpackPlugin(),
    new ModuleFederationPlugin({
      name: "studio",
      filename: "remoteEntry.js",
      remotes: {
        studio: `studio@${combineUrl(
          AppServerConfig.proxyURL,
          "/remoteEntry.js"
        )}`,
        people: `people@${combineUrl(
          AppServerConfig.proxyURL,
          "/products/people/remoteEntry.js"
        )}`,
        login: `login@${combineUrl(
          AppServerConfig.proxyURL,
          "/login/remoteEntry.js"
        )}`,
        files: `files@${combineUrl(
          AppServerConfig.proxyURL,
          "/products/files/remoteEntry.js"
        )}`,
      },
      exposes: {
        "./shell": "./src/Shell",
        "./store": "./src/store",
        "./Error404": "./src/components/pages/Errors/404/",
        "./Error401": "./src/components/pages/Errors/401",
        "./Error403": "./src/components/pages/Errors/403",
        "./Error520": "./src/components/pages/Errors/520",
        "./Layout": "./src/components/Layout",
        "./Layout/context": "./src/components/Layout/context.js",
        "./Main": "./src/components/Main",
        "./toastr": "./src/helpers/toastr",
        "./PreparationPortalDialog":
          "./src/components/dialogs/PreparationPortalDialog/PreparationPortalDialogWrapper.js",
      },
      shared: {
        ...deps,
        ...sharedDeps,
      },
    }),
    new ExternalTemplateRemotesPlugin(),
    new HtmlWebpackPlugin({
      template: "./public/index.html",
      publicPath: homepage,
      title: title,
      base: `${homepage}/`,
    }),
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
    new DefinePlugin({
      VERSION: JSON.stringify(version),
      BUILD_AT: DefinePlugin.runtimeValue(function () {
        const timeElapsed = Date.now();
        const today = new Date(timeElapsed);
        return JSON.stringify(today.toISOString().split(".")[0] + "Z");
      }, true),
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
