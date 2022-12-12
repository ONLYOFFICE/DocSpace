const pks = require("../package.json");
const window = require("global/window");
const { proxy, api } = pks;

module.exports = {
  proxyURL:
    (proxy && proxy.url) ||
    (typeof window !== "undefined" && window?.DOCSPACE_PROXY_URL) ||
    "",
  apiOrigin:
    api?.origin ||
    (typeof window !== "undefined" && window?.DOCSPACE_API_ORIGIN) ||
    "",
  apiPrefix: api?.prefix || "/api/2.0",
  apiTimeout: api?.timeout || 30000,
};
