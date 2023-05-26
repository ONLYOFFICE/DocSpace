import CloudServicesGoogleDriveReactSvgUrl from "PUBLIC_DIR/images/cloud.services.google.drive.react.svg?url";
import CloudServicesBoxReactSvgUrl from "PUBLIC_DIR/images/cloud.services.box.react.svg?url";
import CloudServicesDropboxReactSvgUrl from "PUBLIC_DIR/images/cloud.services.dropbox.react.svg?url";
import CloudServicesOnedriveReactSvgUrl from "PUBLIC_DIR/images/cloud.services.onedrive.react.svg?url";
import CloudServicesKdriveReactSvgUrl from "PUBLIC_DIR/images/cloud.services.kdrive.react.svg?url";
import CloudServicesYandexReactSvgUrl from "PUBLIC_DIR/images/cloud.services.yandex.react.svg?url";
import CloudServicesNextcloudReactSvgUrl from "PUBLIC_DIR/images/cloud.services.nextcloud.react.svg?url";
import CatalogFolderReactSvgUrl from "PUBLIC_DIR/images/catalog.folder.react.svg?url";
import CloudServicesWebdavReactSvgUrl from "PUBLIC_DIR/images/cloud.services.webdav.react.svg?url";
import authStore from "@docspace/common/store/AuthStore";
import { FileType, RoomsType } from "@docspace/common/constants";
import config from "PACKAGE_FILE";
import { combineUrl, toUrlParams } from "@docspace/common/utils";
import { addFileToRecentlyViewed } from "@docspace/common/api/files";
import i18n from "./i18n";

import { request } from "@docspace/common/api/client";

export const getFileTypeName = (fileType) => {
  switch (fileType) {
    case FileType.Unknown:
      return i18n.t("Common:Unknown");
    case FileType.Archive:
      return i18n.t("Common:Archive");
    case FileType.Video:
      return i18n.t("Common:Video");
    case FileType.Audio:
      return i18n.t("Common:Audio");
    case FileType.Image:
      return i18n.t("Common:Image");
    case FileType.Spreadsheet:
      return i18n.t("Files:Spreadsheet");
    case FileType.Presentation:
      return i18n.t("Files:Presentation");
    case FileType.Document:
    case FileType.OFormTemplate:
    case FileType.OForm:
      return i18n.t("Files:Document");
    default:
      return i18n.t("Files:Folder");
  }
};

export const getDefaultRoomName = (room, t) => {
  switch (room) {
    case RoomsType.CustomRoom:
      return t("Files:CustomRooms");

    case RoomsType.FillingFormsRoom:
      return t("Files:FillingFormRooms");

    case RoomsType.EditingRoom:
      return t("Files:CollaborationRooms");

    case RoomsType.ReviewRoom:
      return t("Common:Review");

    case RoomsType.ReadOnlyRoom:
      return t("Files:ViewOnlyRooms");
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
      return i18n.t("Common:NewDocument");
    case "xlsx":
      return i18n.t("Common:NewSpreadsheet");
    case "pptx":
      return i18n.t("Common:NewPresentation");
    case "docxf":
      return i18n.t("Common:NewMasterForm");
    default:
      return i18n.t("Common:NewFolder");
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
      window.DocSpaceConfig?.proxy?.url,
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
      baseURL: combineUrl(window.DocSpaceConfig?.proxy?.url),
      method: "get",
      url: `/filehandler.ashx?${params}`,
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
        window.DocSpaceConfig?.proxy?.url,
        config.homepage,
        `/filehandler.ashx?${params}`
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
      return CloudServicesGoogleDriveReactSvgUrl;
    case "Box":
      return CloudServicesBoxReactSvgUrl;
    case "DropboxV2":
      return CloudServicesDropboxReactSvgUrl;
    case "OneDrive":
      return CloudServicesOnedriveReactSvgUrl;
    case "SharePoint":
      return CloudServicesOnedriveReactSvgUrl;
    case "kDrive":
      return CloudServicesKdriveReactSvgUrl;
    case "Yandex":
      return CloudServicesYandexReactSvgUrl;
    case "NextCloud":
      return CloudServicesNextcloudReactSvgUrl;
    case "OwnCloud":
      return CatalogFolderReactSvgUrl;
    case "WebDav":
      return CloudServicesWebdavReactSvgUrl;
    default:
  }
};

export const getTitleWithoutExtension = (item, fromTemplate) => {
  const titleWithoutExst = item.title.split(".").slice(0, -1).join(".");
  return titleWithoutExst && item.fileExst && !fromTemplate
    ? titleWithoutExst
    : item.title;
};
