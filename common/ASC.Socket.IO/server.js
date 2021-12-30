const express = require("express");
const { Server } = require("socket.io");
const { createServer } = require("http");
const logger = require("morgan");
const redis = require("redis");
const expressSession = require("express-session");
const cookieParser = require("cookie-parser");
const RedisStore = require("connect-redis")(expressSession);
const MemoryStore = require("memorystore")(expressSession);
const sharedsession = require("express-socket.io-session");

const config = require("./config");
const auth = require("./app/middleware/auth.js");
const winston = require("./app/log.js");

winston.stream = {
  write: (message) => winston.info(message),
};

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

app.use(logger("dev", { stream: winston.stream }));
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

app.get("/", (req, res) => {
  res.send("<h1>Invalid Endpoint</h1>");
});

const filesHub = require("./app/hubs/files.js")(io);

app.use("/controller", require("./app/controllers")(filesHub));

httpServer.listen(port, () => winston.info(`Server started on port: ${port}`));

module.exports = io;
