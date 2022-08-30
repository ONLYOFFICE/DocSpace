const { merge } = require("webpack-merge");
const baseConfig = require("./webpack.base.js");
const path = require("path");
const DefinePlugin = require("webpack").DefinePlugin;
const TerserPlugin = require("terser-webpack-plugin");
const CopyPlugin = require("copy-webpack-plugin");

const serverConfig = {
  target: "node",
  name: "server",
  entry: {
    server: "./src/server/index.ts",
  },

  output: {
    path: path.resolve(process.cwd(), "dist/"),
    filename: "[name].js",
    libraryTarget: "commonjs2",
    chunkFilename: "chunks/[name].js",
  },

  plugins: [
    new CopyPlugin({
      patterns: [
        {
          from: "src/server/config/",
        },
      ],
    }),
  ],
};

module.exports = (env, argv) => {
  if (argv.mode === "production") {
    serverConfig.mode = "production";
    serverConfig.optimization = {
      minimize: !env.minimize,
      minimizer: [new TerserPlugin()],
    };
  } else {
    serverConfig.mode = "development";
  }
  serverConfig.plugins = [
    ...serverConfig.plugins,
    new DefinePlugin({
      IS_DEVELOPMENT: argv.mode !== "production",
      PORT: process.env.PORT || 5011,
      IS_PERSONAL: env.personal || false,
      IS_ROOMS_MODE: env.rooms || false,
    }),
  ];

  return merge(baseConfig, serverConfig);
};
