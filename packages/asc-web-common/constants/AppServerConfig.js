const pks = require("../package.json");
const window = require("global/window");
const { proxy, api } = pks;

module.exports = {
  proxyURL:
    (proxy && proxy.url) || (window && window.APPSERVER_PROXY_URL) || "",
  apiPrefixURL: (api && api.url) || "/api/2.0",
  apiTimeout: (api && api.timeout) || 30000,
};
