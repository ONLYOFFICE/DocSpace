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
const combineUrl = require("@docspace/common/utils/combineUrl");
const minifyJson = require("@docspace/common/utils/minifyJson");
const AppServerConfig = require("@docspace/common/constants/AppServerConfig");
const { proxyURL } = AppServerConfig;
const sharedDeps = require("@docspace/common/constants/sharedDependencies");
const baseConfig = require("./webpack.base.js");
const pkg = require("../package.json");
const deps = pkg.dependencies || {};

for (let dep in sharedDeps) {
  sharedDeps[dep].eager = true;
}

const clientConfig = {
  target: "web",
  entry: {
    client: ["./src/client/index.ts"],
  },

  output: {
    path: path.resolve(process.cwd(), "dist/client"),
    filename: "static/js/[name].[contenthash].bundle.js",
    publicPath: "/login/",
    chunkFilename: "static/js/[id].[contenthash].js",
  },

  performance: {
    maxEntrypointSize: 512000,
    maxAssetSize: 512000,
  },

  module: {
    rules: [
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

  plugins: [
    new CleanWebpackPlugin(),
    new ModuleFederationPlugin({
      name: "login",
      filename: "remoteEntry.js",
      remotes: {
        studio: `studio@${combineUrl(proxyURL, "/remoteEntry.js")}`,
      },
      exposes: {
        "./login": "./src/client/App.tsx",
        // "./roomsLogin": "../src/RoomsLogin.tsx",
        // "./codeLogin": "../src/CodeLogin.tsx",
        // "./moreLogin": "../src/sub-components/more-login.tsx",
      },
      shared: { ...sharedDeps, ...deps },
    }),
    new ExternalTemplateRemotesPlugin(),
    new CopyPlugin({
      patterns: [
        {
          context: path.resolve(__dirname, "../public"),
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
      splitChunks: { chunks: "all" },
      minimize: !env.minimize,
      minimizer: [new TerserPlugin()],
    };
  } else {
    clientConfig.mode = "development";
    clientConfig.devtool = "cheap-module-source-map";
  }

  clientConfig.plugins = [
    ...clientConfig.plugins,
    new DefinePlugin({
      IS_DEVELOPMENT: argv.mode !== "production",
      PORT: process.env.PORT || 5011,
    }),
  ];

  return merge(baseConfig, clientConfig);
};
