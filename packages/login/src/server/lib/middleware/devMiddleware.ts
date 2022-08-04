// import winston from "../logger";
import { Response, NextFunction } from "express";
import { getAssets } from "../helpers";

// winston.stream = {
//   write: (message) => winston.info(message), //ts check
// };

export default async (req: DevRequest, res: Response, next: NextFunction) => {
  try {
    const assets = await getAssets();

    req.assets = assets;
  } catch (e) {
    let message: string | unknown = e;
    if (e instanceof Error) {
      message = e.message;
    }
    console.log(message);
  }
  next();
};
