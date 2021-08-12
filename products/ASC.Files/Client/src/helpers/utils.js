import authStore from "@appserver/common/store/AuthStore";
import {
  AppServerConfig,
  FolderType,
  ShareAccessRights,
} from "@appserver/common/constants";
import config from "../../package.json";
import { combineUrl } from "@appserver/common/utils";
import { addFileToRecentlyViewed } from "@appserver/common/api/files";
import i18n from "./i18n";
import docserviceStore from "../store/DocserviceStore";
import { isAdmin as checkAdminRights } from "@appserver/common/utils";

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
  url = null
) => {
  if (!providerKey) {
    await addFileToRecent(id);
  }

  if (!url) {
    url = combineUrl(
      AppServerConfig.proxyURL,
      config.homepage,
      `/doceditor?fileId=${id}`
    );
  }

  return Promise.resolve(
    tab ? (tab.location = url) : window.open(url, "_blank")
  );
};

export const accessEdit = (
  entryData,
  currentFolderId,
  currentFolderAccess,
  user,
  isAdmin,
  isDesktop,

  restrictedEditing
) => {
  if (!entryData) throw "entryData is undefined";

  if (user.isOutsider) return false;

  if (entryData.isFolder) {
    if (entryData.rootFolderType == FolderType.COMMON && !isAdmin) {
      return false;
    }

    if (entryData.rootFolderType == FolderType.SHARE) {
      return false;
    }

    if (entryData.rootFolderType == FolderType.Recent) {
      return false;
    }

    if (entryData.rootFolderType == FolderType.Favorites) {
      return false;
    }

    if (entryData.rootFolderType == FolderType.Templates) {
      return false;
    }

    if (entryData.rootFolderType == FolderType.TRASH) {
      return false;
    }

    if (entryData.rootFolderType == FolderType.Projects) {
      return false;
    }

    if (!isDesktop && entryData.rootFolderType == FolderType.Privacy) {
      return false;
    }
  }

  var curAccess = entryData.access;

  if (
    entryData.isFolder &&
    currentFolderId &&
    currentFolderAccess &&
    entryData.id == currentFolderId
  ) {
    curAccess = currentFolderAccess;
  }

  switch (curAccess) {
    case ShareAccessRights.None:
    case ShareAccessRights.FullAccess:
      return true;
    case ShareAccessRights.ReadOnly:
    case ShareAccessRights.DenyAccess:
      return false;
    case ShareAccessRights.CustomFilter:
    case ShareAccessRights.Review:
    case ShareAccessRights.FormFilling:
    case ShareAccessRights.Comment:
      return !!restrictedEditing;
    default:
      if (
        entryData.isFolder &&
        (entryData.rootFolderType === FolderType.SHARE ||
          entryData.rootFolderType === FolderType.Recent ||
          entryData.rootFolderType === FolderType.Favorites ||
          entryData.rootFolderType === FolderType.Templates ||
          entryData.rootFolderType === FolderType.Projects ||
          entryData.rootFolderType === FolderType.TRASH)
      ) {
        return false;
      }

      return isAdmin || entryData.createdBy == user.id;
  }
};

export const canShare = (
  entryData,
  currentFolderId,
  currentFolderAccess,
  user,
  isPersonal,
  isAdmin,
  isDesktop
) => {
  let isShareable = true;

  if (!entryData.isFolder) {
    if (
      entryData.encrypted &&
      (entryData.rootFolderType != FolderType.Privacy ||
        !isDesktop ||
        //!ASC.Desktop.encryptionSupport() ||
        isPersonal)
    ) {
      isShareable = false;
    } else if (
      isPersonal &&
      !docserviceStore.canViewedDocs(entryData.fileExst)
    ) {
      isShareable = false;
    }
  }

  if (
    isShareable &&
    ((entryData.rootFolderType == FolderType.COMMON && !isAdmin) ||
      (entryData.rootFolderType == FolderType.SHARE &&
        !accessEdit(
          entryData,
          currentFolderId,
          currentFolderAccess,
          user,
          isAdmin,
          isDesktop
        )) ||
      (entryData.rootFolderType == FolderType.Privacy &&
        (entryData.isFolder ||
          !accessEdit(
            entryData,
            currentFolderId,
            currentFolderAccess,
            user,
            isAdmin,
            isDesktop
          ))) ||
      (isPersonal && entryData.isFolder) ||
      user.isVisitor)
  ) {
    isShareable = false;
  }

  //console.log("entry", entryData, "isShareable", isShareable);

  return isShareable;
};
