const pkg = require("../package.json");
const deps = pkg.dependencies;

module.exports = {
  react: {
    singleton: true,
    requiredVersion: deps["react"],
  },
  "react-dom": {
    singleton: true,
    requiredVersion: deps["react-dom"],
  },
  "react-router": {
    singleton: true,
    requiredVersion: deps["react-router"],
  },
  "react-router-dom": {
    singleton: true,
    requiredVersion: deps["react-router-dom"],
  },
  "styled-components": {
    singleton: true,
    requiredVersion: deps["styled-components"],
  },
  mobx: {
    singleton: true,
    requiredVersion: deps["mobx"],
  },
  "mobx-react": {
    singleton: true,
    requiredVersion: deps["mobx-react"],
  },
  moment: {
    singleton: true,
    requiredVersion: deps["moment"],
  },
  "email-addresses": {
    singleton: true,
    requiredVersion: deps["email-addresses"],
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
    requiredVersion: deps["react-toastify"],
  },
  "workbox-window": {
    singleton: true,
    requiredVersion: deps["workbox-window"],
  },
  axios: {
    singleton: true,
    requiredVersion: deps["axios"],
  },
  i18next: {
    singleton: true,
    requiredVersion: deps["i18next"],
  },
  "react-i18next": {
    singleton: true,
    requiredVersion: deps["react-i18next"],
  },
  "i18next-http-backend": {
    singleton: true,
    requiredVersion: deps["i18next-http-backend"],
  },
  "prop-types": {
    singleton: true,
    requiredVersion: deps["prop-types"],
  },
};
