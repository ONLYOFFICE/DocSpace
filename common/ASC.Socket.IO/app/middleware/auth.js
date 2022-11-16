const request = require("../requestManager.js");
const check = require("./authService.js");
const portalManager = require("../portalManager.js");
const logger = require("../log.js");

module.exports = (socket, next) => {
  const req = socket.client.request;
  const session = socket.handshake.session;

  const cookie = req?.cookies?.authorization || req?.cookies?.asc_auth_key;
  const token = req?.headers?.authorization;

  if (!cookie && !token) {
    socket.disconnect("unauthorized");
    next(new Error("Authentication error"));
    return;
  }

  if (token) {
    if (!check(token)) {
      next(new Error("Authentication error"));
    } else {
      session.system = true;
      session.save();
      next();
    }
    return;
  }

  let headers;
  if (cookie)
    headers = {
      Authorization: cookie,
    };

  const basePath = portalManager(req).replace(/\/$/g, "");

  const getUser = () => {
    return request({
      method: "get",
      url: "/people/@self.json?fields=id,userName,displayName",
      headers,
      basePath,
    });
  };

  const getPortal = () => {
    return request({
      method: "get",
      url: "/portal.json?fields=tenantId,tenantDomain",
      headers,
      basePath,
    });
  };

  return Promise.all([getUser(), getPortal()])
    .then(([user, portal]) => {
      session.user = user;
      session.portal = portal;
      session.save();
      next();
    })
    .catch((err) => {
      logger.error(err);
      socket.disconnect("Unauthorized");
      next(new Error("Authentication error"));
    });
};
