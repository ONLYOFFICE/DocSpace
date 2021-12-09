const co = require("co");

const requestManager = require("../requestManager.js");
const authService = require("./authService.js");
//const apiRequestManager = require("../requestManager.js");

module.exports = (socket, next) => {
  const req = socket.client.request;
  const session = socket.handshake.session;

  //console.log("req", req.headers);

  if (req.user) {
    next();

    return;
  }

  if (
    !req.cookies ||
    (!req.cookies["asc_auth_key"] && !req.cookies["authorization"])
  ) {
    // socket.disconnect("unauthorized");
    // next(new Error("Authentication error"));
    // return;
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

  co(() => {
    return Promise.all([
      requestManager.makeRequest(
        "people/@self.json?fields=id,userName,displayName",
        { method: "GET" /* headers */ }
      ),
      requestManager.makeRequest("portal.json?fields=tenantId,tenantDomain", {
        method: "GET" /* headers, */,
      }),
    ])
      .then((res) => {
        //console.log("RESPONSE", res);

        session.save();
        next();
      })
      .catch((err) => {
        socket.disconnect("unauthorized");
        next(new Error("Authentication error"));
      });
  });
};
