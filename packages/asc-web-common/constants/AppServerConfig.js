const pks = require("../package.json");
const { proxy, api } = pks;

module.exports = {
  proxyURL: (proxy && proxy.url) || "",
  apiPrefixURL: (api && api.url) || "/api/2.0",
  apiTimeout: (api && api.timeout) || 30000,
};
