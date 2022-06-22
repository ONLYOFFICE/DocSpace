const conf = require("../config");
const portalInternalUrl = conf.get("core")["base-domain"] === "localhost" ? "http://localhost" : "";
module.exports = (req) => {
  if (portalInternalUrl) return portalInternalUrl;

  const xRewriterUrlInternalHeader = "x-rewriter-url-internal";
  if (req.headers && req.headers[xRewriterUrlInternalHeader]) {
    return req.headers[xRewriterUrlInternalHeader];
  }

  const xRewriterUrlHeader = "x-rewriter-url";
  if (req.headers && req.headers[xRewriterUrlHeader]) {
    return req.headers[xRewriterUrlHeader];
  }

  if (req?.headers?.origin) {
    return req.headers.origin;
  }

  return "";
};
