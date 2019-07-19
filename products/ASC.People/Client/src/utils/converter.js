export const objectToUrlQuery = obj => {
  let str = "";
  for (var key in obj) {
    if (str !== "") {
      str += "&";
    }
    str += key + "=" + encodeURIComponent(obj[key]);
  }

  return str;
};
