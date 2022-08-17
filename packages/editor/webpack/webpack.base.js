const scriptExtensions = /\.(tsx|ts|js|jsx|mjs)$/;
const imageExtensions = /\.(bmp|gif|jpg|jpeg|png)$/;
const fontsExtension = /\.(eot|otf|ttf|woff|woff2)$/;

module.exports = {
  resolve: {
    extensions: [".js", ".jsx", ".json", ".ts", ".tsx"],
    fallback: {
      crypto: false,
    },
  },
  module: {
    rules: [
      {
        test: scriptExtensions,
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
        ],
      },
      {
        test: fontsExtension,
        type: "asset",
      },
      {
        test: /\.svg/,
        type: "asset/inline",
      },
      {
        test: imageExtensions,
        type: "asset/resource",
      },
    ],
  },
};
