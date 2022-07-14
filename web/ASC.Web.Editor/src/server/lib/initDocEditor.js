const {
  getDocServiceUrl,
  getFileInfo,
  openEdit,
  getUser,
  getSettings,
} = require("./api/init");

const combineUrl = (host = "", ...params) => {
  let url = host.replace(/\/+$/, "");

  params.forEach((part) => {
    const newPart = part.trim().replace(/^\/+/, "");
    url += newPart
      ? url.length > 0 && url[url.length - 1] === "/"
        ? newPart
        : `/${newPart}`
      : "";
  });

  return url;
};

module.exports = async (req) => {
  if (!req) return false;

  const { headers, url, query } = req;
  const { version, desktop: isDesktop } = query;
  let error = null;

  try {
    const decodedId = query.fileId || query.fileid || null;
    const fileId =
      typeof decodedId === "string" ? encodeURIComponent(decodedId) : decodedId;

    if (!fileId) {
      return {
        props: {
          needLoader: true,
        },
      };
    }

    const doc = query?.doc || null;
    const view = url.indexOf("action=view") !== -1;
    const fileVersion = version || null;

    const [user, settings] = await Promise.all([
      getUser(null, headers),
      getSettings(headers),
    ]);

    const successAuth = !!user;
    const personal = settings?.personal;

    if (!successAuth && !doc) {
      error = {
        unAuthorized: true,
        redirectPath: combineUrl(
          "", //AppServerConfig.proxyURL,
          personal ? "/sign-in" : "/login"
        ),
      };
      return {
        props: {
          error,
        },
      };
    }

    let [config, docApiUrl, fileInfo] = await Promise.all([
      openEdit(fileId, fileVersion, doc, view, headers),
      getDocServiceUrl(headers),
      getFileInfo(fileId, headers),
    ]);

    const isSharingAccess = fileInfo && fileInfo.canShare;

    if (view) {
      config.editorConfig.mode = "view";
    }

    const actionLink = config?.editorConfig?.actionLink || null;

    return {
      props: {
        fileInfo,
        docApiUrl,
        config,
        personal,
        successAuth,
        user,
        error,
        actionLink,
        isSharingAccess,
        url,
        doc,
        fileId,
        view,
      },
    };
  } catch (err) {
    error = { errorMessage: typeof err === "string" ? err : err.message };
    return {
      props: {
        error,
      },
    };
  }
};
