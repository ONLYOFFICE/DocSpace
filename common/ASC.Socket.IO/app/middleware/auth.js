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
    const err = new Error("Authentication error (not token or cookie)");
    logger.error(err);
    socket.disconnect("unauthorized");
    next(err);
    return;
  }

  if (token) {
    if (!check(token)) {
      const err = new Error("Authentication error (token check)");
      logger.error(err);
      next(err);
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

  const basePath = portalManager(req)?.replace(/\/$/g, "");

  logger.info(`API basePath='${basePath}' Authorization='${cookie}'`);

  const getUser = () => {
    return request({
      method: "get",
      url: "/people/@self?fields=id,userName,displayName",
      headers,
      basePath,
    });
  };

  const getPortal = () => {
    return request({
      method: "get",
      url: "/portal?fields=tenantId,tenantDomain",
      headers,
      basePath,
    });
  };

  return Promise.all([getUser(), getPortal()])
    .then(([user, portal]) => {
      logger.info("Get account info", { user, portal });
      session.user = user;
      session.portal = portal;
      session.save();
      next();
    })
    .catch((err) => {
      logger.error("Error of getting account info", err);
      socket.disconnect("Unauthorized");
      next(err);
    });
};
