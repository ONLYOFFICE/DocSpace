const HtmlWebpackPlugin = require("html-webpack-plugin");
const path = require("path");
const deps = require("./package.json").dependencies;

module.exports = {
  entry: "./src/index",
  mode: "development",

  devServer: {
    contentBase: [path.join(__dirname, "public"), path.join(__dirname, "dist")],
    port: 5021,
    historyApiFallback: true,
    hot: false,
    hotOnly: false,
  },

  output: {
    publicPath: "auto",
    chunkFilename: "[id].[contenthash].js",
  },

  resolve: {
    extensions: [".jsx", ".js", ".json"],
    fallback: {
      crypto: false,
    },
    alias: {
      ASCWebComponents: path.resolve(
        __dirname,
        "../../packages/asc-web-components/src/"
      ),
      ASCWebCommon: path.resolve(
        __dirname,
        "../../packages/asc-web-common/src"
      ),
    },
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
        use: ["@svgr/webpack"],
      },
      { test: /\.json$/, loader: "json-loader" },
      {
        test: /\.css$/i,
        use: ["style-loader", "css-loader"],
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
    new HtmlWebpackPlugin({
      template: "./public/index.html",
    }),
  ],
};
