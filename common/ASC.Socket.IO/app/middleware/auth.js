const request = require("../requestManager.js");
const check = require("./authService.js");
const portalManager = require("../portalManager.js");
const logger = require("../log.js");

module.exports = (socket, next) => {
  const req = socket.client.request;
  const session = socket.handshake.session;

  const cookie = req?.cookies?.authorization || req?.cookies?.asc_auth_key;
  const token = req?.headers?.authorization;
  const share = socket.handshake.query?.share;

  if (!cookie && !token && !share) {
    const err = new Error("Authentication error (not token or cookie or share key)");
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

  const basePath = portalManager(req)?.replace(/\/$/g, "");
  let headers = {};

  if (cookie) {
    headers.Authorization = cookie;

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
  }

  if (share) {
    if (req?.cookies) {
      const pairs = Object.entries(req.cookies).map(([key, value]) => `${key}=${value}`);

      if (pairs.length > 0) {
        let cookie = pairs.join(';');
        cookie += ';';
        headers.Cookie = cookie;
      }
    }
    
    return request({
      method: "get",
      url: `/files/share/${share}`,
      headers,
      basePath,
    }).then(validation => {
      if (validation.status !== 0) {
        const err = new Error("Invalid share key");
        logger.error("Share key validation failure:", err);
        next(err);
      } else {
        logger.info(`Share key validation successful: key=${share}`)
        session.anonymous = true;
        session.portal = { tenantId: validation.tenantId }
        session.save();
        next();
      }
    }).catch(err => {
      logger.error(err);
      socket.disconnect("Unauthorized");
      next(err);
    })
  }
};