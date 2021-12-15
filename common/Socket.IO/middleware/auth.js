const request = require("../requestManager.js");
const authService = require("./authService.js");

module.exports = (socket, next) => {
  const req = socket.client.request;
  const session = socket.handshake.session;
  const cookie = req.cookies["asc_auth_key"];

  // if (req.user) {
  //   next();

  //   return;
  // }

  if (!req.cookies || !cookie) {
    socket.disconnect("unauthorized");
    next(new Error("Authentication error"));
    return;
  }

  // if (
  //   session &&
  //   session.user &&
  //   session.portal
  // ) {
  //   req.user = session.user;
  //   req.portal = session.portal;
  //   next();
  //   return;
  // }

  // if (req.cookies["authorization"]) {
  //   if (!authService(req)) {
  //     next(new Error("Authentication error"));
  //   } else {
  //     next();
  //   }
  //   return;
  // }

  let headers;
  if (cookie)
    headers = {
      Authorization: cookie,
    };

  const getUser = () => {
    return request({
      method: "get",
      //url: "/people/@self.json?fields=id,userName,displayName",
      url: "/people/@self.json",
      headers,
    });
  };

  const getPortal = () => {
    return request({
      method: "get",
      url: "/portal.json?fields=tenantId,tenantDomain",
      headers,
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
      socket.disconnect("Unauthorized");
      next(new Error("Authentication error"));
    });
};
