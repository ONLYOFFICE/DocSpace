import { initSSR } from "@appserver/common/api/client";
import { getUser } from "@appserver/common/api/people";
import { getSettings } from "@appserver/common/api/settings";
import combineUrl from "@appserver/common/utils/combineUrl";
import { AppServerConfig } from "@appserver/common/constants";
import {
  getDocServiceUrl,
  getFileInfo,
  checkFillFormDraft,
  openEdit,
} from "@appserver/common/api/files";
import pkg from "../../package.json";

export const initDocEditor = async (req) => {
  if (!req) return false;

  const { headers, url, query } = req;
  const { version, desktop: isDesktop } = query;
  let error = null;
  initSSR(headers);
  try {
    //const doc = url.indexOf("doc=") !== -1 ? url.split("doc=")[1] : null;??
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

    const doc = query?.doc || null; // TODO: need to check
    const view = url.indexOf("action=view") !== -1;
    const fileVersion = version || null;

    const [user, settings] = await Promise.all([getUser(), getSettings()]);

    const successAuth = !!user;
    const personal = settings?.personal;
    const cultureName = user.cultureName;

    if (!successAuth && !doc) {
      error = {
        unAuthorized: true,
        redirectPath: combineUrl(
          AppServerConfig.proxyURL,
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
      openEdit(fileId, fileVersion, doc, view),
      getDocServiceUrl(),
      getFileInfo(fileId),
    ]);

    if (successAuth) {
      try {
        // if (url.indexOf("#message/") > -1) {
        //   if (canConvert(fileInfo.fileExst)) {
        //     const url = await convertDocumentUrl();
        //     history.pushState({}, null, url);
        //   }
        // } TODO: move to hook?
      } catch (err) {
        error = { errorMessage: typeof err === "string" ? err : err.message };
      }
    }

    let formUrl;

    if (
      !view &&
      fileInfo &&
      fileInfo.canWebRestrictedEditing &&
      fileInfo.canFillForms &&
      !fileInfo.canEdit
    ) {
      try {
        formUrl = await checkFillFormDraft(fileId);
        // TODO: move to hook?
        // history.pushState({}, null, formUrl);
        //   url = window.location.href;
      } catch (err) {
        error = err;
      }
    }

    const needInitDesktop = false;
    if (isDesktop) {
      // initDesktop(config); TODO: move to hook
      needInitDesktop = true;
    }

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
        needInitDesktop,
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

export const getFavicon = (documentType) => {
  const { homepage } = pkg;
  let icon = null;

  switch (documentType) {
    case "text":
      icon = "text.ico";
      break;
    case "presentation":
      icon = "presentation.ico";
      break;
    case "spreadsheet":
      icon = "spreadsheet.ico";
      break;
    default:
      break;
  }

  const favicon = icon ? `${homepage}/images/${icon}` : "/favicon.ico";
  return favicon;
};
