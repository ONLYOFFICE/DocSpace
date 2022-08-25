import { diskStorage } from "multer";
import * as path from "path";
import * as fs from "fs";

const storage = diskStorage({
  destination: function (req, file, cb) {
    const dir = path.join(__dirname, "../../../../../../../public/plugins");

    if (!fs.existsSync(dir)) {
      fs.mkdirSync(dir);
    }

    cb(null, dir);
  },
  filename: function (req, file, cb) {
    cb(null, file.originalname);
  },
});

export default storage;
