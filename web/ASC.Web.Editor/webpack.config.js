// const serverConfig = require("./webpack.server");
// const clientConfig = require("./webpack.client");

module.exports = (env, argv) => {
  // if (argv.mode === "production") {
  //   process.env.NODE_ENV = "production";
  // } else {
  //   process.env.NODE_ENV = "development";
  // }
  console.log([require("./webpack.client"), require("./webpack.server")]);
  //return [require("./webpack.client"), require("./webpack.server")];
  return [require("./webpack.client"), require("./webpack.server")];
};
