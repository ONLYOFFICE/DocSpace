import { diskStorage } from "multer";
import * as path from "path";

const storage = diskStorage({
  destination: function (req, file, cb) {
    const dir = path.join(__dirname, "../../../../public/scripts");

    cb(null, dir);
  },
  filename: function (req, file, cb) {
    cb(null, file.originalname);
  },
});

export default storage;
