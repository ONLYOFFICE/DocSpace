import authStore from "@appserver/common/store/AuthStore";
import { AppServerConfig } from "@appserver/common/constants";
import config from "../../package.json";
import { combineUrl } from "@appserver/common/utils";
import {
  addFileToRecentlyViewed,
  createFile,
} from "@appserver/common/api/files";
import i18n from "./i18n";
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

export const createNewFile = async (folderId, fileName, open = true) => {
  const file = await createFile(folderId, fileName);

  open && (await openDocEditor(file.id));

  return file;
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

  return Promise.resolve(
    tab
      ? (tab.location = url)
      : window.open(
          combineUrl(
            AppServerConfig.proxyURL,
            config.homepage,
            `/doceditor?fileId=${id}`
          ),
          "_blank"
        )
  );
};
