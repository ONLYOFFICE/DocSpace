const express = require("express");
const { Server } = require("socket.io");
const { createServer } = require("http");
const cookie = require("cookie");
const expressSession = require("express-session");
const cookieParser = require("cookie-parser");
const MemoryStore = require("memorystore")(expressSession);
const sharedsession = require("express-socket.io-session");

const requestManager = require("./requestManager");
const config = require("./config");
const auth = require("./middleware/auth.js");

const port = config.get("port") || 3000;
const app = express();

const secret = config.get("core.machinekey") + new Date().getTime();
const secretCookieParser = cookieParser(secret);
const baseCookieParser = cookieParser();

const redis = {
  host: config.get("redis:host"),
  port: config.get("redis:port"),
  ttl: 3600,
};

let store;
if (redis.host && redis.port) {
  //store = new RedisStore(redis);
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

var tempHeaders = ""; //TODO:

const options = {
  cors: {
    origin: "*",
    //methods: ["GET", "POST"],
    //credentials: true,
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
  // console.log("socket.handshake", socket.user); //TODO:
  // const request = socket.client.request;

  socket.on("startFileEdit", async (fileId) => {
    const url = `files/file/${fileId}`;
    const options = { method: "GET", headers: tempHeaders }; //{ method: "POST", body: {} };
    const file = await requestManager.makeRequest(url, options, socket);
    io.emit("subFileChanges", file);
  });

  socket.on("reportFileCreation", (fileId) => {
    io.emit("getFileCreation", fileId);
  });
});

io.engine.on("headers", (headers, request) => {
  if (!request.headers.cookie) return;

  const cookies = cookie.parse(request.headers.cookie);
  headers["Authorization"] = cookies["asc_auth_key"];

  tempHeaders = headers;
});

// io.engine.on("initial_headers", (headers, req) => {
//   if (req.headers) {
//     const xRewriterUrlHeader = "x-rewriter-url";
//     const xForwardedForHeader = "x-forwarded-for";

//     if (req.headers[xRewriterUrlHeader]) {
//       headers[xRewriterUrlHeader] = req.headers[xRewriterUrlHeader];
//     }
//     if (req.headers[xForwardedForHeader]) {
//       headers[xForwardedForHeader] = req.headers[xForwardedForHeader];
//     }
//   }
// });

app.get("/", (req, res) => {
  res.send("<h1>Invalid Endpoint</h1>");
});

httpServer.listen(port, () => console.log(`Server started on port: ${port}`));

module.exports = io;
