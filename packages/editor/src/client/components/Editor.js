import React, { useEffect } from "react";
import { isMobile, isIOS, deviceType } from "react-device-detect";
import combineUrl from "@docspace/common/utils/combineUrl";
import { AppServerConfig, FolderType } from "@docspace/common/constants";
import throttle from "lodash/throttle";
import Toast from "@docspace/components/toast";
import { toast } from "react-toastify";
import {
  restoreDocumentsVersion,
  markAsFavorite,
  removeFromFavorite,
  getEditDiff,
  getEditHistory,
  updateFile,
  checkFillFormDraft,
  convertFile,
} from "@docspace/common/api/files";
import { EditorWrapper } from "../components/StyledEditor";
import { useTranslation } from "react-i18next";
import withDialogs from "../helpers/withDialogs";
import { canConvert } from "../helpers/utils";
import { assign } from "@docspace/common/utils";
import toastr from "@docspace/components/toast/toastr";

toast.configure();

const onSDKInfo = (event) => {
  console.log(
    "ONLYOFFICE Document Editor is opened in mode " + event.data.mode
  );
};

const onSDKWarning = (event) => {
  console.log(
    "ONLYOFFICE Document Editor reports a warning: code " +
      event.data.warningCode +
      ", description " +
      event.data.warningDescription
  );
};

const onSDKError = (event) => {
  console.log(
    "ONLYOFFICE Document Editor reports an error: code " +
      event.data.errorCode +
      ", description " +
      event.data.errorDescription
  );
};

const text = "text";
const presentation = "presentation";
let documentIsReady = false;
let docSaved = null;
let docTitle = null;
let docEditor;

function Editor({
  config,
  personal,
  successAuth,
  isSharingAccess,
  user,
  doc,
  actionLink,
  error,
  sharingDialog,
  onSDKRequestSharingSettings,
  loadUsersRightsList,
  isVisible,
  selectFileDialog,
  onSDKRequestInsertImage,
  onSDKRequestMailMergeRecipients,
  onSDKRequestCompareFile,
  selectFolderDialog,
  onSDKRequestSaveAs,
  isDesktopEditor,
  initDesktop,
  view,
  mfReady,
  fileId,
  url,
  filesSettings,
}) {
  const fileInfo = config?.file;
  const { t } = useTranslation(["Editor", "Common"]);

  useEffect(() => {
    if (error && mfReady) {
      if (error?.unAuthorized && error?.redirectPath) {
        localStorage.setItem("redirectPath", window.location.href);
        window.location.href = error?.redirectPath;
      }
      const errorText = typeof error === "string" ? error : error.errorMessage;
      toastr.error(errorText);
    }
  }, [mfReady, error]);

  useEffect(() => {
    if (config) {
      document.getElementById("scripDocServiceAddress").onload = onLoad();
      setDocumentTitle(config?.document?.title);

      if (isIOS && deviceType === "tablet") {
        const vh = window.innerHeight * 0.01;
        document.documentElement.style.setProperty("--vh", `${vh}px`);
      }

      if (
        !view &&
        fileInfo &&
        fileInfo.canWebRestrictedEditing &&
        fileInfo.canFillForms &&
        !fileInfo.canEdit
      ) {
        try {
          initForm();
        } catch (err) {
          console.error(err);
        }
      }

      if (view) {
        config.editorConfig.mode = "view";
      }
    }
  }, []);

  useEffect(() => {
    if (config) {
      if (isDesktopEditor) {
        initDesktop(config, user, fileId, t);
      }
    }
  }, [isDesktopEditor]);

  useEffect(() => {
    try {
      const url = window.location.href;

      if (
        successAuth &&
        url.indexOf("#message/") > -1 &&
        fileInfo &&
        fileInfo?.fileExst &&
        canConvert(fileInfo.fileExst, filesSettings)
      ) {
        showDocEditorMessage(url);
      }
    } catch (err) {
      console.error(err);
    }
  }, [url, fileInfo?.fileExst]);

  const initForm = async () => {
    const formUrl = await checkFillFormDraft(fileId);
    history.pushState({}, null, formUrl);

    document.location.reload();
  };

  const convertDocumentUrl = async () => {
    const convert = await convertFile(fileId, null, true);
    return convert && convert[0]?.result;
  };

  const showDocEditorMessage = async (url) => {
    const result = await convertDocumentUrl();
    const splitUrl = url.split("#message/");

    if (result) {
      const newUrl = `${result.webUrl}#message/${splitUrl[1]}`;

      history.pushState({}, null, newUrl);
    }
  };

  const getDefaultFileName = (format) => {
    switch (format) {
      case "docx":
        return t("NewDocument");
      case "xlsx":
        return t("NewSpreadsheet");
      case "pptx":
        return t("NewPresentation");
      case "docxf":
        return t("NewMasterForm");
      default:
        return t("NewFolder");
    }
  };

  const throttledChangeTitle = throttle(() => changeTitle(), 500);

  const onSDKRequestHistoryClose = () => {
    document.location.reload();
  };

  const onSDKRequestEditRights = async () => {
    console.log("ONLYOFFICE Document Editor requests editing rights");
    const index = url.indexOf("&action=view");

    if (index) {
      let convertUrl = url.substring(0, index);

      if (canConvert(fileInfo.fileExst, filesSettings)) {
        const newUrl = await convertDocumentUrl();
        if (newUrl) {
          convertUrl = newUrl.webUrl;
        }
      }
      history.pushState({}, null, convertUrl);
      document.location.reload();
    }
  };

  const onMakeActionLink = (event) => {
    const actionData = event.data;

    const link = generateLink(actionData);

    const urlFormation = !actionLink ? url : url.split("&anchor=")[0];

    const linkFormation = `${urlFormation}&anchor=${link}`;

    docEditor.setActionLink(linkFormation);
  };

  const generateLink = (actionData) => {
    return encodeURIComponent(JSON.stringify(actionData));
  };

  const onSDKRequestRename = (event) => {
    const title = event.data;
    updateFile(fileInfo.id, title);
  };

  const onSDKRequestRestore = async (event) => {
    const restoreVersion = event.data.version;
    try {
      const updateVersions = await restoreDocumentsVersion(
        fileId,
        restoreVersion,
        doc
      );
      const historyLength = updateVersions.length;
      docEditor.refreshHistory({
        currentVersion: getCurrentDocumentVersion(
          updateVersions,
          historyLength
        ),
        history: getDocumentHistory(updateVersions, historyLength),
      });
    } catch (e) {
      docEditor.refreshHistory({
        error: `${e}`, //TODO: maybe need to display something else.
      });
    }
  };

  const getDocumentHistory = (fileHistory, historyLength) => {
    let result = [];

    for (let i = 0; i < historyLength; i++) {
      const changes = fileHistory[i].changes;
      const serverVersion = fileHistory[i].serverVersion;
      const version = fileHistory[i].version;
      const versionGroup = fileHistory[i].versionGroup;

      let obj = {
        ...(changes.length !== 0 && { changes }),
        created: `${new Date(fileHistory[i].created).toLocaleString(
          config.editorConfig.lang
        )}`,
        ...(serverVersion && { serverVersion }),
        key: fileHistory[i].key,
        user: {
          id: fileHistory[i].user.id,
          name: fileHistory[i].user.name,
        },
        version,
        versionGroup,
      };

      result.push(obj);
    }
    return result;
  };

  const getCurrentDocumentVersion = (fileHistory, historyLength) => {
    return url.indexOf("&version=") !== -1
      ? +url.split("&version=")[1]
      : fileHistory[historyLength - 1].version;
  };

  const onSDKRequestHistory = async () => {
    try {
      const fileHistory = await getEditHistory(fileId, doc);
      const historyLength = fileHistory.length;

      docEditor.refreshHistory({
        currentVersion: getCurrentDocumentVersion(fileHistory, historyLength),
        history: getDocumentHistory(fileHistory, historyLength),
      });
    } catch (e) {
      docEditor.refreshHistory({
        error: `${e}`, //TODO: maybe need to display something else.
      });
    }
  };

  const onSDKRequestHistoryData = async (event) => {
    const version = event.data;

    try {
      const versionDifference = await getEditDiff(fileId, version, doc);
      const changesUrl = versionDifference.changesUrl;
      const previous = versionDifference.previous;
      const token = versionDifference.token;

      docEditor.setHistoryData({
        ...(changesUrl && { changesUrl }),
        key: versionDifference.key,
        fileType: versionDifference.fileType,
        ...(previous && {
          previous: {
            fileType: previous.fileType,
            key: previous.key,
            url: previous.url,
          },
        }),
        ...(token && { token }),
        url: versionDifference.url,
        version,
      });
    } catch (e) {
      docEditor.setHistoryData({
        error: `${e}`, //TODO: maybe need to display something else.
        version,
      });
    }
  };

  const onDocumentReady = () => {
    documentIsReady = true;

    if (isSharingAccess) {
      loadUsersRightsList(docEditor);
    }
  };

  const updateFavorite = (favorite) => {
    docEditor.setFavorite(favorite);
  };

  const onMetaChange = (event) => {
    const newTitle = event.data.title;
    const favorite = event.data.favorite;

    if (newTitle && newTitle !== docTitle) {
      setDocumentTitle(newTitle);
      docTitle = newTitle;
    }

    if (!newTitle) {
      const onlyNumbers = new RegExp("^[0-9]+$");
      const isFileWithoutProvider = onlyNumbers.test(fileId);

      const convertFileId = isFileWithoutProvider ? +fileId : fileId;

      favorite
        ? markAsFavorite([convertFileId])
            .then(() => updateFavorite(favorite))
            .catch((error) => console.log("error", error))
        : removeFromFavorite([convertFileId])
            .then(() => updateFavorite(favorite))
            .catch((error) => console.log("error", error));
    }
  };

  const setDocumentTitle = (subTitle = null) => {
    //const { isAuthenticated, settingsStore, product: currentModule } = auth;
    //const { organizationName } = settingsStore;
    const organizationName = "ONLYOFFICE"; //TODO: Replace to API variant
    const moduleTitle = "Documents"; //TODO: Replace to API variant

    let title;
    if (subTitle) {
      if (successAuth && moduleTitle) {
        title = subTitle + " - " + moduleTitle;
      } else {
        title = subTitle + " - " + organizationName;
      }
    } else if (moduleTitle && organizationName) {
      title = moduleTitle + " - " + organizationName;
    } else {
      title = organizationName;
    }
    document.title = title;
  };

  const changeTitle = () => {
    docSaved ? setDocumentTitle(docTitle) : setDocumentTitle(`*${docTitle}`);
  };

  const onDocumentStateChange = (event) => {
    if (!documentIsReady) return;

    docSaved = !event.data;
    throttledChangeTitle();
  };

  const onSDKAppReady = () => {
    console.log("ONLYOFFICE Document Editor is ready");
    const url = window.location.href;

    const index = url.indexOf("#message/");

    if (index > -1) {
      const splitUrl = url.split("#message/");
      if (splitUrl.length === 2) {
        const message = decodeURIComponent(raw).replace(/\+/g, " ");
        docEditor.showMessage(message);
        history.pushState({}, null, url.substring(0, index));
      } else {
        if (config?.Error) docEditor.showMessage(config.Error);
      }
    }
  };

  const onLoad = () => {
    try {
      if (!window.DocsAPI) throw new Error("DocsAPI is not defined");

      if (isMobile) {
        config.type = "mobile";
      }

      let goBack;
      const url = window.location.href;

      if (fileInfo) {
        let backUrl = "";

        if (fileInfo.rootFolderType === FolderType.Rooms) {
          backUrl = `/rooms/shared/${fileInfo.folderId}/filter?folder=${fileInfo.folderId}`;
        } else {
          backUrl = `/rooms/personal/filter?folder=${fileInfo.folderId}`;
        }

        const origin = url.substring(0, url.indexOf("/doceditor"));

        goBack = {
          blank: true,
          requestClose: false,
          text: t("FileLocation"),
          url: `${combineUrl(origin, backUrl)}`,
        };
      }

      config.editorConfig.customization = {
        ...config.editorConfig.customization,
        goback: goBack,
      };

      if (personal && !fileInfo) {
        //TODO: add conditions for SaaS
        config.document.info.favorite = null;
      }

      if (url.indexOf("anchor") !== -1) {
        const splitUrl = url.split("anchor=");
        const decodeURI = decodeURIComponent(splitUrl[1]);
        const obj = JSON.parse(decodeURI);

        config.editorConfig.actionLink = {
          action: obj.action,
        };
      }

      if (successAuth) {
        const documentType = config.documentType;
        const fileExt =
          documentType === text
            ? "docx"
            : documentType === presentation
            ? "pptx"
            : "xlsx";

        const defaultFileName = getDefaultFileName(fileExt);

        if (!user.isVisitor)
          config.editorConfig.createUrl = combineUrl(
            window.location.origin,
            AppServerConfig.proxyURL,
            "products/files/",
            `/httphandlers/filehandler.ashx?action=create&doctype=text&title=${encodeURIComponent(
              defaultFileName
            )}`
          );
      }

      let onRequestSharingSettings,
        onRequestRename,
        onRequestSaveAs,
        onRequestInsertImage,
        onRequestMailMergeRecipients,
        onRequestCompareFile,
        onRequestRestore;

      if (isSharingAccess) {
        onRequestSharingSettings = onSDKRequestSharingSettings;
      }

      if (fileInfo && fileInfo.canEdit) {
        onRequestRename = onSDKRequestRename;
      }

      if (successAuth) {
        onRequestSaveAs = onSDKRequestSaveAs;
        onRequestInsertImage = onSDKRequestInsertImage;
        onRequestMailMergeRecipients = onSDKRequestMailMergeRecipients;
        onRequestCompareFile = onSDKRequestCompareFile;
      }

      if (!!config.document.permissions.changeHistory) {
        onRequestRestore = onSDKRequestRestore;
      }
      const events = {
        events: {
          onAppReady: onSDKAppReady,
          onDocumentStateChange: onDocumentStateChange,
          onMetaChange: onMetaChange,
          onDocumentReady: onDocumentReady,
          onInfo: onSDKInfo,
          onWarning: onSDKWarning,
          onError: onSDKError,
          onRequestSharingSettings,
          onRequestRename,
          onMakeActionLink: onMakeActionLink,
          onRequestInsertImage,
          onRequestSaveAs,
          onRequestMailMergeRecipients,
          onRequestCompareFile,
          onRequestEditRights: onSDKRequestEditRights,
          onRequestHistory: onSDKRequestHistory,
          onRequestHistoryClose: onSDKRequestHistoryClose,
          onRequestHistoryData: onSDKRequestHistoryData,
          onRequestRestore,
        },
      };

      const newConfig = Object.assign(config, events);

      docEditor = window.docEditor = window.DocsAPI.DocEditor(
        "editor",
        newConfig
      );

      assign(window, ["ASC", "Files", "Editor", "docEditor"], docEditor); //Do not remove: it's for Back button on Mobile App
    } catch (error) {
      toastr.error(error.message, null, 0, true);
    }
  };

  return (
    <EditorWrapper isVisibleSharingDialog={isVisible}>
      <div id="editor"></div>
      {sharingDialog}
      {selectFileDialog}
      {selectFolderDialog}
      <Toast />
    </EditorWrapper>
  );
}

export default withDialogs(Editor);
