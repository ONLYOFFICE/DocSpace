module.exports = {
  presets: ["@babel/preset-env", "@babel/preset-react"],
  plugins: [
    "@loadable/babel-plugin",
    "babel-plugin-styled-components",
    ["@babel/plugin-proposal-class-properties", { loose: false }],
    "@babel/plugin-proposal-export-default-from",
  ],
  //ignore: ["node_modules", "build"],
  //sourceType: "unambiguous",
};
