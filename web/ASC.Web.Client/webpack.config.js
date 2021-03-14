const { CleanWebpackPlugin } = require("clean-webpack-plugin");
const CopyPlugin = require("copy-webpack-plugin");
const HtmlWebpackPlugin = require("html-webpack-plugin");
const ModuleFederationPlugin = require("webpack").container
  .ModuleFederationPlugin;
const TerserPlugin = require("terser-webpack-plugin");
const { InjectManifest } = require("workbox-webpack-plugin");
//const CompressionPlugin = require("compression-webpack-plugin");

const path = require("path");
const pkg = require("./package.json");
const deps = pkg.dependencies;
const homepage = pkg.homepage;
const title = pkg.title;

const config = {
  entry: "./src/index",
  mode: "development",

  devServer: {
    contentBase: [path.join(__dirname, "dist")],
    port: 5001,
    historyApiFallback: {
      // Paths with dots should still use the history fallback.
      // See https://github.com/facebook/create-react-app/issues/387.
      disableDotRule: true,
      index: homepage,
    },
    writeToDisk: true,
    // proxy: [
    //   {
    //     context: "/api",
    //     target: "http://localhost:8092",
    //   },
    // ],
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
    chunkFilename: "js/[id].[contenthash].js",
    assetModuleFilename: "assets/[hash][ext][query]",
    path: path.resolve(process.cwd(), "dist"),
    filename: "[name].[contenthash].bundle.js",
  },

  module: {
    rules: [
      {
        test: /\.(png|jpe?g|gif|ico)$/i,
        type: "asset/resource",
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
    new CleanWebpackPlugin(),
    new ModuleFederationPlugin({
      name: "studio",
      filename: "remoteEntry.js",
      remotes: {
        studio: `studio@${homepage}/remoteEntry.js`,
        login: `login@${homepage}/login/remoteEntry.js`,
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
        "./PeopleSelector": "./src/components/PeopleSelector",
        "./PeopleSelector/UserTooltip":
          "./src/components/PeopleSelector/sub-components/UserTooltip.js",
        "./toastr": "./src/helpers/toastr",
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
  ],
};

module.exports = (env, argv) => {
  if (argv.mode === "production") {
    config.mode = "production";
    config.optimization = {
      splitChunks: { chunks: "all" },
      minimize: true,
      minimizer: [new TerserPlugin()],
    };

    // config.plugins.push(
    //   new CompressionPlugin({
    //     filename: "[path][base].gz[query]",
    //     algorithm: "gzip",
    //     test: /\.js(\?.*)?$/i,
    //     threshold: 10240,
    //     minRatio: 0.8,
    //     deleteOriginalAssets: true,
    //   })
    // );
  } else {
    config.devtool = "cheap-module-source-map";
  }

  config.plugins.push(
    new InjectManifest({
      mode: argv.mode === "production" ? "production" : "development",
      swSrc: "./src/sw-template.js", // this is your sw template file
      swDest: "service-worker.js", // this will be created in the build step
      //globDirectory: "dist",
      // globPatterns: [
      //   "**/!(service-worker|precache-manifest.*).{js,css,html,png,svg}",
      // ],
      exclude: [
        /\.map$/,
        /manifest$/,
        /\.htaccess$/,
        /serviceWorker\.js$/,
        /sw\.js$/,
      ],
    })
  );

  return config;
};
