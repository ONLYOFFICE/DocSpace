const HtmlWebpackPlugin = require("html-webpack-plugin");
const ModuleFederationPlugin = require("webpack").container
  .ModuleFederationPlugin;
const path = require("path");
const pkg = require("./package.json");
const deps = pkg.dependencies;
const homepage = pkg.homepage;

const config = {
  entry: "./src/index",
  mode: "development",

  devServer: {
    contentBase: path.join(__dirname, "public"),
    port: 5001,
    historyApiFallback: {
      // Paths with dots should still use the history fallback.
      // See https://github.com/facebook/create-react-app/issues/387.
      disableDotRule: true,
      index: homepage,
    },
    proxy: [
      {
        context: "/api",
        target: "http://localhost:8092",
      },
    ],
    hot: false,
    hotOnly: false,
    headers: {
      "Access-Control-Allow-Origin": "*",
      "Access-Control-Allow-Methods": "GET, POST, PUT, DELETE, PATCH, OPTIONS",
      "Access-Control-Allow-Headers":
        "X-Requested-With, content-type, Authorization",
    },
    openPage: "http://localhost:8092",
  },
  resolve: {
    extensions: [".jsx", ".js", ".json"],
    fallback: {
      crypto: false,
    },
  },

  output: {
    publicPath: "auto",
    chunkFilename: "[id].[contenthash].js",
    path: path.resolve(process.cwd(), "dist"),
  },

  module: {
    rules: [
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
          "css-loader",
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
    new ModuleFederationPlugin({
      name: "studio",
      filename: "remoteEntry.js",
      remotes: {
        studio: `studio@${homepage}/remoteEntry.js`,
        people: `people@${homepage}/products/people//remoteEntry.js`,
        files: `files@${homepage}/products/files/remoteEntry.js`,
        login: `login@${homepage}/login/remoteEntry.js`,
      },
      exposes: {
        "./shell": "./src/Shell",
        "./store": "./src/store",
        "./Error404": "./src/components/pages/Errors/404/",
        "./Error401": "./src/components/pages/Errors/401",
        "./Error403": "./src/components/pages/Errors/403",
        "./Error520": "./src/components/pages/Errors/520",
      },
      shared: {
        ...deps,
        react: {
          singleton: true,
          requiredVersion: deps.react,
        },
        "react-dom": {
          singleton: true,
          requiredVersion: deps["react-dom"],
        },
      },
    }),
    new HtmlWebpackPlugin({
      template: "./public/index.html",
      publicPath: homepage,
    }),
  ],
};

module.exports = (env, argv) => {
  if (argv.mode === "production") {
    config.mode = "production";
  } else {
    config.devtool = "source-map";
  }

  return config;
};
