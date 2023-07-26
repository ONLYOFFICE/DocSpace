const presets = [
  [
    "@babel/preset-env",
    {
      modules: false,
    },
  ],
  "@babel/preset-react",
  "@babel/preset-typescript",
];

const plugins = [
  "@babel/plugin-proposal-class-properties",
  "@babel/plugin-proposal-export-namespace-from",
  "babel-plugin-styled-components",
  "@babel/plugin-proposal-export-default-from",
];

module.exports = {
  presets,
  plugins,
  env: {
    test: {
      presets: ["@babel/preset-env", "@babel/preset-react"],
    },
  },
};
