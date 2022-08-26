import * as fs from "fs";
import * as path from "path";

import * as config from "../../config";

const { path: pathToPlugins } = config.default.get("pluginsConf");

const fileFilter = (req, file, cb) => {
  const pluginsDir = path.join(__dirname, pathToPlugins);

  let files = null;
  let isUniqName = true;

  if (fs.existsSync(pluginsDir)) {
    files = fs.readdirSync(pluginsDir);

    isUniqName = !files?.includes(file.originalname);
  }

  if (file.mimetype === "text/javascript" && isUniqName) return cb(null, true);

  return cb(null, false);
};

export default fileFilter;
