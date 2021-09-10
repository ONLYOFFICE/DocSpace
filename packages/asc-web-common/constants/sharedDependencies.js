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
  },
  "@babel/runtime": {
    singleton: true,
  },
  "react-toastify": {
    singleton: true,
    requiredVersion: "6.2.0",
  },
  "workbox-window": {
    singleton: true,
  },
};
