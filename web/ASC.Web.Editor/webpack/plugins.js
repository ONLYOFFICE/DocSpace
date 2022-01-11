const { CleanWebpackPlugin } = require("clean-webpack-plugin");
const ExternalTemplateRemotesPlugin = require("external-remotes-plugin");
const HtmlWebpackPlugin = require("html-webpack-plugin");
const CopyPlugin = require("copy-webpack-plugin");

const pkg = require("../package.json");
const homepage = pkg.homepage; // combineUrl(AppServerConfig.proxyURL, pkg.homepage);
const title = pkg.title;

const client = [
  new CleanWebpackPlugin(),
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
];

module.exports = {
  client,
};
