import path from "path";
import fs from "fs";
import { initSSR } from "@docspace/common/api/client";
import { getUser } from "@docspace/common/api/people";
import {
  getSettings,
  getBuildVersion,
  getCurrentCustomSchema,
  getAppearanceTheme,
  getLogoUrls,
} from "@docspace/common/api/settings";
import {
  openEdit,
  getSettingsFiles,
  // getShareFiles,
} from "@docspace/common/api/files";

import { getLogoFromPath } from "@docspace/common/utils";

export const getFavicon = (logoUrls) => {
  const favicon = logoUrls[2]?.path?.light;
  return favicon;
};

export const initDocEditor = async (req) => {
  if (!req) return false;
  let personal = IS_PERSONAL || null;
  const { headers, url, query, type } = req;
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

    const [
      user,
      settings,
      filesSettings,
      versionInfo,
      customNames,
      appearanceTheme,
      logoUrls,
    ] = await Promise.all([
      getUser(),
      getSettings(),
      getSettingsFiles(),
      getBuildVersion(),
      getCurrentCustomSchema("Common"),
      getAppearanceTheme(),
      getLogoUrls(),
    ]);

    const successAuth = !!user;

    personal = settings?.personal;

    if (!successAuth && !doc) {
      error = {
        unAuthorized: true,
        // redirectPath: combineUrl(
        //   window?.DocSpaceConfig?.proxy?.url,
        //   personal ? "/sign-in" : "/login"
        // ),
      };
      return { error };
    }

    const config = await openEdit(fileId, fileVersion, doc, view);

    //const sharingSettings = await getShareFiles([+fileId], []);

    // const isSharingAccess = false; //TODO: temporary disable sharing (many errors). Restore => config?.file && config?.file?.canShare;

    if (view) {
      config.editorConfig.mode = "view";
    }

    if (type) {
      config.type = type;
    }

    logoUrls.forEach((logo) => {
      logo.path.dark = getLogoFromPath(logo.path.dark);
      logo.path.light = getLogoFromPath(logo.path.dark);
    });

    return {
      config,
      personal,
      successAuth,
      user,
      error,
      //isSharingAccess,
      url,
      doc,
      fileId,
      view,
      filesSettings,
      //sharingSettings,
      portalSettings: settings,
      versionInfo,
      customNames,
      appearanceTheme,
      logoUrls,
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
