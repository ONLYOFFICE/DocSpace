import path from "path";
import fs from "fs";
import { initSSR } from "@appserver/common/api/client";
import { getUser } from "@appserver/common/api/people";
import { getSettings } from "@appserver/common/api/settings";
import combineUrl from "@appserver/common/utils/combineUrl";
import { AppServerConfig } from "@appserver/common/constants";
import {
  getDocServiceUrl,
  getFileInfo,
  openEdit,
  getSettingsFiles,
} from "@appserver/common/api/files";
import pkg from "../../../../package.json";

export const getFavicon = (documentType) => {
  const { homepage } = pkg;
  let icon = null;

  switch (documentType) {
    case "word":
      icon = "text.ico";
      break;
    case "slide":
      icon = "presentation.ico";
      break;
    case "cell":
      icon = "spreadsheet.ico";
      break;
    default:
      break;
  }

  const favicon = icon ? `${homepage}/images/${icon}` : "/favicon.ico";
  return favicon;
};

export const initDocEditor = async (req) => {
  if (!req) return false;
  let personal = IS_PERSONAL || null;
  const { headers, url, query } = req;
  const { version, desktop: isDesktop } = query;
  let error = null;
  initSSR(headers);

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

    const [user, settings, filesSettings] = await Promise.all([
      getUser(),
      getSettings(),
      getSettingsFiles(),
    ]);

    const successAuth = !!user;
    personal = settings?.personal;

    if (!successAuth && !doc) {
      error = {
        unAuthorized: true,
        redirectPath: combineUrl(
          AppServerConfig.proxyURL,
          personal ? "/sign-in" : "/login"
        ),
      };
      return { error };
    }

    let [config, docApiUrl, fileInfo] = await Promise.all([
      openEdit(fileId, fileVersion, doc, view),
      getDocServiceUrl(),
      getFileInfo(fileId),
    ]);

    const isSharingAccess = fileInfo && fileInfo.canShare;

    if (view) {
      config.editorConfig.mode = "view";
    }

    const actionLink = config?.editorConfig?.actionLink || null;

    return {
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
      filesSettings,
    };
  } catch (err) {
    error = { errorMessage: typeof err === "string" ? err : err.message };
    return { error };
  }
};

export const getAssets = () => {
  const manifest = fs.readFileSync(
    path.join(__dirname, "client/manifest.json"),
    "utf-8"
  );

  const assets = JSON.parse(manifest);

  return assets;
};

export const getScripts = (assets) => {
  const regTest = /static\/js\/.*/;
  const keys = [];

  for (let key in assets) {
    if (assets.hasOwnProperty(key) && regTest.test(key)) {
      keys.push(key);
    }
  }

  return keys;
};
