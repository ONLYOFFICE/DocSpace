const { CleanWebpackPlugin } = require("clean-webpack-plugin");
const CopyPlugin = require("copy-webpack-plugin");
const HtmlWebpackPlugin = require("html-webpack-plugin");
const ModuleFederationPlugin = require("webpack").container
  .ModuleFederationPlugin;
const DefinePlugin = require("webpack").DefinePlugin;
const ExternalTemplateRemotesPlugin = require("external-remotes-plugin");
const TerserPlugin = require("terser-webpack-plugin");
const combineUrl = require("@docspace/common/utils/combineUrl");
const minifyJson = require("@docspace/common/utils/minifyJson");
const AppServerConfig = require("@docspace/common/constants/AppServerConfig");
const sharedDeps = require("@docspace/common/constants/sharedDependencies");

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
    extensions: [".jsx", ".js", ".tsx", ".ts", ".json"],
    fallback: {
      crypto: false,
    },
    alias: {
      PUBLIC_DIR: path.resolve(__dirname, "../../public"),
      SRC_DIR: path.resolve(__dirname, "./src"),
      PACKAGE_FILE: path.resolve(__dirname, "package.json"),
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
        test: /\.(js|jsx|ts|tsx)$/,
        exclude: /node_modules/,
        use: [
          {
            loader: "babel-loader",
            options: {
              presets: [
                "@babel/preset-react",
                "@babel/preset-env",
                "@babel/preset-typescript",
              ],
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
    new ExternalTemplateRemotesPlugin(),

    new CopyPlugin({
      patterns: [
        {
          context: path.resolve(__dirname, "public"),
          from: "images/**/*.*",
        },
        {
          context: path.resolve(__dirname, "public"),
          from: "locales/**/*.json",
          transform: minifyJson,
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

  const remotes = {
    client: `client@${combineUrl(AppServerConfig.proxyURL, "/remoteEntry.js")}`,
    // people: `people@${combineUrl(
    //   AppServerConfig.proxyURL,
    //   "/products/people/remoteEntry.js"
    // )}`,
    // files: `files@${combineUrl(
    //   AppServerConfig.proxyURL,
    //   "/products/files/remoteEntry.js"
    // )}`,
  };

  if (!env.personal) {
    remotes.login = `login@${combineUrl(
      AppServerConfig.proxyURL,
      "/login/remoteEntry.js"
    )}`;
  }

  config.plugins.push(
    new ModuleFederationPlugin({
      name: "client",
      filename: "remoteEntry.js",
      remotes: remotes,
      exposes: {
        "./shell": "./src/Shell",
        "./store": "./src/store",
        "./Error404": "./src/pages/Errors/404/",
        "./Error401": "./src/pages/Errors/401",
        "./Error403": "./src/pages/Errors/403",
        "./Error520": "./src/pages/Errors/520",
        "./Layout": "./src/components/Layout",
        "./Layout/context": "./src/components/Layout/context.js",
        "./Main": "./src/components/Main",
        "./toastr": "./src/helpers/toastr",
        "./PreparationPortalDialog":
          "./src/components/dialogs/PreparationPortalDialog/PreparationPortalDialogWrapper.js",
        "./SharingDialog": "./src/components/panels/SharingDialog",
        "./utils": "./src/helpers/filesUtils.js",
        "./SelectFileDialog":
          "./src/components/panels/SelectFileDialog/SelectFileDialogWrapper",
        "./SelectFileInput":
          "./src/components/panels/SelectFileInput/SelectFileInputWrapper",
        "./SelectFolderDialog":
          "./src/components/panels/SelectFolderDialog/SelectFolderDialogWrapper",
        "./SelectFolderInput":
          "./src/components/panels/SelectFolderInput/SelectFolderInputWrapper",
        "./GroupSelector": "./src/components/GroupSelector",
        "./PeopleSelector": "./src/components/PeopleSelector",
        "./PeopleSelector/UserTooltip":
          "./src/components/PeopleSelector/sub-components/UserTooltip.js",
      },
      shared: {
        ...deps,
        ...sharedDeps,
      },
    })
  );

  if (!!env.hideText) {
    config.plugins.push(
      new HtmlWebpackPlugin({
        template: "./public/index.html",
        publicPath: homepage,
        title: title,
        base: `${homepage}/`,
        custom: `<style type="text/css">
          div,
          p,
          a,
          span,
          button,
          h1,
          h2,
          h3,
          h4,
          h5,
          h6,
          ::placeholder {
            color: rgba(0, 0, 0, 0) !important;
        }
        </style>`,
      })
    );
  } else {
    config.plugins.push(
      new HtmlWebpackPlugin({
        template: "./public/index.html",
        publicPath: homepage,
        title: title,
        base: `${homepage}/`,
      })
    );
  }

  const defines = {
    VERSION: JSON.stringify(version),
    BUILD_AT: DefinePlugin.runtimeValue(function () {
      const timeElapsed = Date.now();
      const today = new Date(timeElapsed);
      return JSON.stringify(today.toISOString().split(".")[0] + "Z");
    }, true),
    IS_PERSONAL: env.personal || false,
  };

  config.plugins.push(new DefinePlugin(defines));

  return config;
};
