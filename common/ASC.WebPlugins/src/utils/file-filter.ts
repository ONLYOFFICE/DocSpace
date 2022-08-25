import * as fs from "fs";
import * as path from "path";

const fileFilter = (req, file, cb) => {
  const pluginsDir = path.join(
    __dirname,
    "../../../../../../../public/scripts"
  );

  var files = fs.readdirSync(pluginsDir);

  const isUniqName = !files.includes(file.originalname);

  if (file.mimetype === "text/javascript" && isUniqName) return cb(null, true);

  return cb(null, false);
};

export default fileFilter;
