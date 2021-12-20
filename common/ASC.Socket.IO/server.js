const express = require("express");
const { Server } = require("socket.io");
const { createServer } = require("http");
const redis = require("redis");
const expressSession = require("express-session");
const cookieParser = require("cookie-parser");
const RedisStore = require("connect-redis")(expressSession);
const MemoryStore = require("memorystore")(expressSession);
const sharedsession = require("express-socket.io-session");

//const request = require("./requestManager");
const config = require("./config");
const auth = require("./app/middleware/auth2.js");

const port = config.get("port") || 3000;
const app = express();

const secret = config.get("core.machinekey") + new Date().getTime();
const secretCookieParser = cookieParser(secret);
const baseCookieParser = cookieParser();

const redisOptions = config.get("redis");

let store;
if (redisOptions?.enabled) {
  const redisClient = redis.createClient(redisOptions);
  store = new RedisStore({ client: redisClient });
} else {
  store = new MemoryStore();
}

const session = expressSession({
  store: store,
  secret: secret,
  resave: true,
  saveUninitialized: true,
  cookie: {
    path: "/",
    httpOnly: true,
    secure: false,
    maxAge: null,
  },
  cookieParser: secretCookieParser,
  name: "socketio.sid",
});

app.use(session);

const httpServer = createServer(app);

const options = {
  cors: {
    "Content-Type": "text/html",
  },
  allowRequest: (req, fn) => {
    var cookies = baseCookieParser(req, null, () => {});
    if (
      !req.cookies ||
      (!req.cookies["asc_auth_key"] && !req.cookies["authorization"])
    ) {
      return fn("auth", false);
    }
    return fn("auth", true);
    //return io2.checkRequest(req, fn);
  },
};

const io = new Server(httpServer, options);

io.use(sharedsession(session, secretCookieParser, { autoSave: true }))
  .use((socket, next) => {
    baseCookieParser(socket.client.request, null, next);
  })
  .use((socket, next) => {
    auth(socket, next);
  });

io.on("connection", (socket) => {
  const session = socket.handshake.session;

  if (!session || !session.user || !session.portal) {
    console.log("Invalid session");
    return;
  }

  const userId = session.user.id;
  const tenantId = session.portal.tenantId;

  console.log(
    `Connected user='${userId}' on tenant='${tenantId}' socketId='${socket.id}'`
  );

  const room = tenantId;

  socket.join(room);

  socket.on("c:start-edit-file", (fileId) => {
    //const file = { id: fileId, name: "some file", fileStatus: 1 };
    console.log("Call of start-edit-file", fileId);
    socket.to(room).emit("s:start-edit-file", fileId);
  });

  socket.on("c:stop-edit-file", (fileId) => {
    //const file = { id: fileId, name: "some file", fileStatus: 0 };
    console.log("Call of stop-edit-file", fileId);
    socket.to(room).emit("s:stop-edit-file", fileId);
  });

  socket.on("c:refresh-folder", (folderId) => {
    socket.to(room).emit("s:refresh-folder", folderId);
  });
});

app.get("/", (req, res) => {
  res.send("<h1>Invalid Endpoint</h1>");
});

httpServer.listen(port, () => console.log(`Server started on port: ${port}`));

module.exports = io;
