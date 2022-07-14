const request = require("./requestManager");
const Encoder = require("./encoder");
exports.getDocServiceUrl = (headers) => {
  return request({ method: "get", url: `/files/docservice`, headers });
};

exports.getFileInfo = (fileId, headers) => {
  const options = {
    method: "get",
    url: `/files/file/${fileId}`,
    headers,
  };

  return request(options);
};

exports.openEdit = (fileId, version, doc, view, headers) => {
  const params = []; // doc ? `?doc=${doc}` : "";

  if (view) {
    params.push(`view=${view}`);
  }

  if (version) {
    params.push(`version=${version}`);
  }

  if (doc) {
    params.push(`doc=${doc}`);
  }

  const paramsString = params.length > 0 ? `?${params.join("&")}` : "";

  const options = {
    method: "get",
    url: `/files/file/${fileId}/openedit${paramsString}`,
    headers,
  };

  return request(options);
};

exports.getSettings = (headers) => {
  return request({
    method: "get",
    url: "/settings.json",
    headers,
  });
};

exports.getUser = (userName = null, headers) => {
  return request({
    method: "get",
    url: `/people/${userName || "@self"}.json`,
    skipUnauthorized: true,
    headers,
  }).then((user) => {
    if (user && user.displayName) {
      user.displayName = Encoder.htmlDecode(user.displayName);
    }
    return user;
  });
};
