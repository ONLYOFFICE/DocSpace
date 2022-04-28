module.exports = () => {
  const config = require("../../config"),
    crypto = require("crypto"),
    moment = require("moment");

  const skey = config.get("core").machinekey;
  const trustInterval = 5 * 60 * 1000;

  function check(token) {
    if (!token || typeof token !== "string") return false;

    const splitted = token.split(":");
    if (splitted.length < 3) return false;

    const pkey = splitted[0].substr(4);
    const date = splitted[1];
    const orighash = splitted[2];

    const timestamp = moment.utc(date, "YYYYMMDDHHmmss");
    if (moment.utc() - timestamp > trustInterval) {
      return false;
    }

    const hasher = crypto.createHmac("sha1", skey);
    const hash = hasher.update(date + "\n" + pkey);

    if (hash.digest("base64") !== orighash) {
      return false;
    }

    return true;
  }

  return check;
};
