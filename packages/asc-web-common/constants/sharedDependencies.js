const pkg = require("../package.json");
const deps = pkg.dependencies;

module.exports = {
  react: {
    singleton: true,
    requiredVersion: deps.react,
  },
  "react-dom": {
    singleton: true,
    requiredVersion: deps["react-dom"],
  },
  "email-addresses": {
    singleton: true,
  },
  "fast-deep-equal": {
    singleton: true,
    requiredVersion: deps["fast-deep-equal"],
  },
  "@babel/runtime": {
    singleton: true,
    requiredVersion: deps["@babel/runtime"],
  },
  "react-toastify": {
    singleton: true,
    requiredVersion: "6.2.0",
  },
  "workbox-window": {
    singleton: true,
    requiredVersion: deps["workbox-window"],
  },
  axios: {
    singleton: true,
    requiredVersion: deps.axios,
  },
  i18next: {
    singleton: true,
    requiredVersion: deps.i18next,
  },
  "react-i18next": {
    singleton: true,
    requiredVersion: deps["react-i18next"],
  },
  "i18next-http-backend": {
    singleton: true,
    requiredVersion: deps["i18next-http-backend"],
  },
};
