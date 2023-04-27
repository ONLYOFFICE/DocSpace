const pkg = require("../package.json");
const comPkg = require("@docspace/components/package.json");

const deps = pkg.dependencies || {};
const compDeps = comPkg.dependencies || {};

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
    requiredVersion: compDeps["email-addresses"],
  },
  "fast-deep-equal": {
    singleton: true,
    requiredVersion: deps["fast-deep-equal"],
  },
  "@babel/runtime": {
    singleton: true,
    requiredVersion: deps["@babel/runtime"],
  },
  "rc-tree": {
    singleton: true,
    requiredVersion: compDeps["rc-tree"],
  },
  "react-autosize-textarea": {
    singleton: true,
    requiredVersion: deps["react-autosize-textarea"],
  },
  "react-content-loader": {
    singleton: true,
    requiredVersion: deps["react-content-loader"],
  },
  "react-toastify": {
    singleton: true,
    requiredVersion: compDeps["react-toastify"],
  },
  "react-window-infinite-loader": {
    singleton: true,
    requiredVersion: deps["react-window-infinite-loader"],
  },
  "react-virtualized-auto-sizer": {
    singleton: true,
    requiredVersion: deps["react-virtualized-auto-sizer"],
  },
  "re-resizable": {
    singleton: true,
    requiredVersion: deps["re-resizable"],
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
  "prop-types": {
    singleton: true,
    requiredVersion: deps["prop-types"],
  },
  "react-custom-scrollbars": {
    singleton: true,
    requiredVersion: compDeps["react-custom-scrollbars"],
  },
  "react-device-detect": {
    singleton: true,
    requiredVersion: compDeps["react-device-detect"],
    // eager: true,
  },
  "react-dropzone": {
    singleton: true,
    requiredVersion: compDeps["react-dropzone"],
  },
  "react-onclickoutside": {
    singleton: true,
    requiredVersion: compDeps["react-onclickoutside"],
  },
  "react-player": {
    singleton: true,
    requiredVersion: deps["react-player"],
  },
  // "react-resize-detector": {
  //   singleton: true,
  //   requiredVersion: deps["react-resize-detector"],
  // },
  "react-svg": {
    singleton: true,
    requiredVersion: compDeps["react-svg"],
  },
  "react-text-mask": {
    singleton: true,
    requiredVersion: compDeps["react-text-mask"],
  },
  "resize-image": {
    singleton: true,
    requiredVersion: compDeps["resize-image"],
  },
  "react-tooltip": {
    singleton: true,
    requiredVersion: deps["react-tooltip"],
  },
  "react-viewer": {
    singleton: true,
    requiredVersion: deps["react-viewer"],
  },
  "react-window": {
    singleton: true,
    requiredVersion: deps["react-window"],
  },
  "react-hammerjs": {
    singleton: true,
    requiredVersion: deps["react-hammerjs"],
  },
  screenfull: {
    singleton: true,
    requiredVersion: deps["screenfull"],
  },
  sjcl: {
    singleton: true,
    requiredVersion: deps["sjcl"],
  },
  "query-string": {
    singleton: true,
    requiredVersion: deps["query-string"],
  },
  "@loadable/component": {
    singleton: true,
    requiredVersion: deps["@loadable/component"],
  },
};
