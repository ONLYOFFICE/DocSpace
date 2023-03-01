const combineUrl = (host = "", ...params) => {
  let url = host.replace(/\/+$/, "");

  params.forEach((part) => {
    if (!part) return;
    const newPart = part.trim().replace(/^\/+/, "");
    url += newPart
      ? url.length > 0 && url[url.length - 1] === "/"
        ? newPart
        : `/${newPart}`
      : "";
  });

  return url;
};

module.exports = combineUrl;
