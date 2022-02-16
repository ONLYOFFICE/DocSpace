module.exports = (env = "production") => {
  return [require("./webpack.client"), require("./webpack.server")];
};
