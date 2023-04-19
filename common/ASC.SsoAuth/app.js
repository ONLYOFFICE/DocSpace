/*
 *
 * (c) Copyright Ascensio System Limited 2010-2023
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

"use strict";

process.env.NODE_ENV = process.env.NODE_ENV || "development";

const http = require("http"),
  express = require("express"),
  morgan = require("morgan"),
  cookieParser = require("cookie-parser"),
  bodyParser = require("body-parser"),
  session = require("express-session"),
  winston = require("./app/log.js"),
  config = require("./config").get(),
  path = require("path"),
  exphbs = require("express-handlebars"),
  favicon = require("serve-favicon"),
  cors = require("cors");

winston.stream = {
  write: (message) => winston.info(message),
};

const app = express();

// view engine setup
app.set("views", path.join(__dirname, "views"));
app.engine("handlebars", exphbs({ defaultLayout: "main" }));
app.set("view engine", "handlebars");

const machineKey = config["core"].machinekey
  ? config["core"].machinekey
  : config.app.machinekey;

app
  .use(favicon(path.join(__dirname, "public", "favicon.ico")))
  .use(morgan("combined", { stream: winston.stream }))
  .use(cookieParser())
  .use(bodyParser.json())
  .use(bodyParser.urlencoded({ extended: false }))
  .use(
    session({
      resave: true,
      saveUninitialized: true,
      secret: machineKey,
    })
  )
  .use(cors());

require("./app/middleware/saml")(app, config);
require("./app/routes")(app, config);

const httpServer = http.createServer(app);

httpServer.listen(config.app.port, function () {
  winston.info(
    `Start SSO Service Provider listening on port ${config.app.port} ` +
      `machineKey='${machineKey}' ` +
      `appsettings path='${config.app.appsettings}'`
  );
});
