import authStore from "@docspace/common/store/AuthStore";
import { AppServerConfig, RoomsType } from "@docspace/common/constants";
import config from "PACKAGE_FILE";
import { combineUrl, toUrlParams } from "@docspace/common/utils";
import { addFileToRecentlyViewed } from "@docspace/common/api/files";
import i18n from "./i18n";

import { request } from "@docspace/common/api/client";

export const getDefaultRoomName = (room, t) => {
  switch (room) {
    case RoomsType.CustomRoom:
      return t("CustomRooms");

    case RoomsType.FillingFormsRoom:
      return t("FillingFormRooms");

    case RoomsType.EditingRoom:
      return t("CollaborationRooms");

    case RoomsType.ReviewRoom:
      return t("ReviewRooms");

    case RoomsType.ReadOnlyRoom:
      return t("ViewOnlyRooms");
  }
};

export const setDocumentTitle = (subTitle = null) => {
  const { isAuthenticated, settingsStore, product: currentModule } = authStore;
  const { organizationName } = settingsStore;

  let title;
  if (subTitle) {
    if (isAuthenticated && currentModule) {
      title = subTitle + " - " + currentModule.title;
    } else {
      title = subTitle + " - " + organizationName;
    }
  } else if (currentModule && organizationName) {
    title = currentModule.title + " - " + organizationName;
  } else {
    title = organizationName;
  }

  document.title = title;
};

export const getDefaultFileName = (format) => {
  switch (format) {
    case "docx":
      return i18n.t("NewDocument");
    case "xlsx":
      return i18n.t("NewSpreadsheet");
    case "pptx":
      return i18n.t("NewPresentation");
    case "docxf":
      return i18n.t("NewMasterForm");
    default:
      return i18n.t("NewFolder");
  }
};

export const addFileToRecent = async (fileId) => {
  try {
    await addFileToRecentlyViewed(fileId);
    console.log("Pushed to recently viewed");
  } catch (e) {
    console.error(e);
  }
};
export const openDocEditor = async (
  id,
  providerKey = null,
  tab = null,
  url = null,
  isPrivacy
) => {
  if (!providerKey && id && !isPrivacy) {
    await addFileToRecent(id);
  }

  if (!url && id) {
    url = combineUrl(
      AppServerConfig.proxyURL,
      config.homepage,
      `/doceditor?fileId=${encodeURIComponent(id)}`
    );
  }

  if (tab) {
    url ? (tab.location = url) : tab.close();
  } else {
    window.open(url, "_blank");
  }

  return Promise.resolve();
};

export const getDataSaveAs = async (params) => {
  try {
    const data = await request({
      baseURL: combineUrl(AppServerConfig.proxyURL, config.homepage),
      method: "get",
      url: `/products/files/httphandlers/filehandler.ashx?${params}`,
      responseType: "text",
    });

    return data;
  } catch (e) {
    console.error("error");
  }
};
export const SaveAs = (title, url, folderId, openNewTab) => {
  const options = {
    action: "create",
    fileuri: url,
    title: title,
    folderid: folderId,
    response: openNewTab ? null : "message",
  };

  const params = toUrlParams(options, true);
  if (!openNewTab) {
    return getDataSaveAs(params);
  } else {
    window.open(
      combineUrl(
        AppServerConfig.proxyURL,
        config.homepage,
        `/httphandlers/filehandler.ashx?${params}`
      ),
      "_blank"
    );
  }
};

export const connectedCloudsTitleTranslation = (key, t) => {
  switch (key) {
    case "Box":

    case "BoxNet":
      return t("Translations:FolderTitleBoxNet");

    case "DropBox":
    case "DropboxV2":
      return t("Translations:FolderTitleDropBox");

    case "DocuSign":
      return t("Translations:FolderTitleDocuSign");

    case "Google":
    case "GoogleDrive":
      return t("Translations:FolderTitleGoogle");

    case "OneDrive":
    case "SkyDrive":
      return t("Translations:FolderTitleSkyDrive");

    case "SharePoint":
      return t("Translations:FolderTitleSharePoint");
    case "WebDav":
      return t("Translations:FolderTitleWebDav");
    case "kDrive":
      return t("Translations:FolderTitlekDrive");
    case "Yandex":
      return t("Translations:FolderTitleYandex");

    default:
      return key;
  }
};

export const connectedCloudsTypeTitleTranslation = (key, t) => {
  switch (key) {
    case "Box":
    case "BoxNet":
      return t("Translations:TypeTitleBoxNet");

    case "DropBox":
    case "DropboxV2":
      return t("Translations:TypeTitleDropBox");

    case "DocuSign":
      return t("Translations:TypeTitleDocuSign");

    case "Google":
    case "GoogleDrive":
      return t("Translations:TypeTitleGoogle");

    case "OneDrive":
    case "SkyDrive":
      return t("Translations:TypeTitleSkyDrive");

    case "SharePoint":
      return t("Translations:TypeTitleSharePoint");
    case "WebDav":
      return t("Translations:TypeTitleWebDav");
    case "kDrive":
      return t("Translations:TypeTitlekDrive");
    case "Yandex":
      return t("Translations:TypeTitleYandex");

    default:
      return key;
  }
};

export const connectedCloudsTypeIcon = (key) => {
  switch (key) {
    case "GoogleDrive":
      return "/static/images/cloud.services.google.drive.react.svg";
    case "Box":
      return "/static/images/cloud.services.box.react.svg";
    case "DropboxV2":
      return "/static/images/cloud.services.dropbox.react.svg";
    case "OneDrive":
      return "/static/images/cloud.services.onedrive.react.svg";
    case "SharePoint":
      return "/static/images/cloud.services.onedrive.react.svg";
    case "kDrive":
      return "/static/images/cloud.services.kdrive.react.svg";
    case "Yandex":
      return "/static/images/cloud.services.yandex.react.svg";
    case "NextCloud":
      return "/static/images/cloud.services.nextcloud.react.svg";
    case "OwnCloud":
      return "/static/images/catalog.folder.react.svg";
    case "WebDav":
      return "/static/images/cloud.services.webdav.react.svg";
    default:
  }
};
