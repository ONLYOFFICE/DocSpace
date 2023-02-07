//const conf = require("../config");
//const portalInternalUrl = conf.get("core")["base-domain"] === "localhost" ? "http://localhost" : ""; //Do not use base-domain for portalInternalUrl
module.exports = (req) => {
  //if (portalInternalUrl) return portalInternalUrl; //TODO: Fix internal api url setup after external api domain complete

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
