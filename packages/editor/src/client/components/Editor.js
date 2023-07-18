import React, { useEffect } from "react";
import { isMobile, isIOS, deviceType } from "react-device-detect";
import combineUrl from "@docspace/common/utils/combineUrl";
import { FolderType, EDITOR_ID } from "@docspace/common/constants";
import throttle from "lodash/throttle";
import Toast from "@docspace/components/toast";
import { toast } from "react-toastify";
import {
  restoreDocumentsVersion,
  getEditDiff,
  getEditHistory,
  createFile,
  updateFile,
  checkFillFormDraft,
  convertFile,
  getReferenceData,
  getSharedUsers,
  sendEditorNotify,
} from "@docspace/common/api/files";
import { EditorWrapper } from "../components/StyledEditor";
import { useTranslation } from "react-i18next";
import withDialogs from "../helpers/withDialogs";
import { assign } from "@docspace/common/utils";
import toastr from "@docspace/components/toast/toastr";
import { DocumentEditor } from "@onlyoffice/document-editor-react";
import ErrorContainer from "@docspace/common/components/ErrorContainer";
import styled from "styled-components";
import { getEditorTheme } from "../helpers/utils";

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
const ErrorContainerBody = styled(ErrorContainer)`
  position: absolute;
  height: 100%;
`;

let documentIsReady = false;
let docSaved = null;
let docTitle = null;
let docEditor;
let newConfig;
let documentserverUrl =
  typeof window !== "undefined" && window?.location?.origin;
let userAccessRights = {};
let isArchiveFolderRoot = true;
let usersInRoom = [];
function Editor({
  config,
  //personal,
  successAuth,
  // isSharingAccess,
  user,
  doc,
  error,
  // sharingDialog,
  // onSDKRequestSharingSettings,
  // loadUsersRightsList,
  // isVisible,
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

  isArchiveFolderRoot =
    fileInfo && fileInfo.rootFolderType === FolderType.Archive;

  const { t } = useTranslation(["Editor", "Common"]);

  if (fileInfo) {
    userAccessRights = fileInfo.security;
  }
  useEffect(() => {
    if (error && mfReady) {
      if (error?.unAuthorized) {
        sessionStorage.setItem("referenceUrl", window.location.href);
        window.open(
          combineUrl(window.DocSpaceConfig?.proxy?.url, "/login"),
          "_self"
        );
      }
    }
  }, [mfReady, error]);

  useEffect(() => {
    if (!config) return;

    setDocumentTitle(config?.document?.title);

    if (isIOS && deviceType === "tablet") {
      const vh = window.innerHeight * 0.01;
      document.documentElement.style.setProperty("--vh", `${vh}px`);
    }

    if (
      !view &&
      fileInfo &&
      fileInfo.viewAccessability.WebRestrictedEditing &&
      fileInfo.security.FillForms &&
      !fileInfo.security.Edit &&
      !config?.document?.isLinkedForMe
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

    init();
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
        fileInfo?.viewAccessability?.Convert &&
        fileInfo?.security?.Convert
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

  const getDefaultFileName = (withExt = false) => {
    const documentType = config?.documentType;

    const fileExt =
      documentType === "word"
        ? "docx"
        : documentType === "slide"
        ? "pptx"
        : documentType === "cell"
        ? "xlsx"
        : "docxf";

    let fileName = t("Common:NewDocument");

    switch (fileExt) {
      case "xlsx":
        fileName = t("Common:NewSpreadsheet");
        break;
      case "pptx":
        fileName = t("Common:NewPresentation");
        break;
      case "docxf":
        fileName = t("Common:NewMasterForm");
        break;
      default:
        break;
    }

    if (withExt) {
      fileName = `${fileName}.${fileExt}`;
    }

    return fileName;
  };

  const throttledChangeTitle = throttle(() => changeTitle(), 500);

  const onSDKRequestHistoryClose = () => {
    document.location.reload();
  };

  const onSDKRequestEditRights = async () => {
    console.log("ONLYOFFICE Document Editor requests editing rights");
    const url = window.location.href;

    const index = url.indexOf("&action=view");

    if (index) {
      let convertUrl = url.substring(0, index);

      if (fileInfo?.viewAccessability?.Convert && fileInfo?.security?.Convert) {
        const newUrl = await convertDocumentUrl();
        if (newUrl) {
          convertUrl = newUrl.webUrl;
        }
      }
      history.pushState({}, null, convertUrl);
      document.location.reload();
    }
  };

  const onSDKRequestReferenceData = async (event) => {
    const referenceData = await getReferenceData(
      event.data.referenceData ?? event.data
    );

    docEditor.setReferenceData(referenceData);
  };

  const onMakeActionLink = (event) => {
    const url = window.location.href;
    const actionLink = config?.editorConfig?.actionLink;

    const actionData = event.data;

    const link = generateLink(actionData);

    const urlFormation = !actionLink ? url : url.split("&anchor=")[0];

    const linkFormation = `${urlFormation}&anchor=${link}`;

    docEditor.setActionLink(linkFormation);
  };

  const generateLink = (actionData) => {
    return encodeURIComponent(JSON.stringify(actionData));
  };

  const onSDKRequestCreateNew = (event) => {
    const defaultFileName = getDefaultFileName(true);

    createFile(fileInfo.folderId, defaultFileName)
      .then((newFile) => {
        const newUrl = combineUrl(
          window.DocSpaceConfig?.proxy?.url,
          config.homepage,
          `/doceditor?fileId=${encodeURIComponent(newFile.id)}`
        );
        window.open(newUrl, "_blank");
      })
      .catch((e) => {
        toastr.error(e);
      });
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
    } catch (error) {
      let errorMessage = "";
      if (typeof error === "object") {
        errorMessage =
          error?.response?.data?.error?.message ||
          error?.statusText ||
          error?.message ||
          "";
      } else {
        errorMessage = error;
      }

      docEditor.refreshHistory({
        error: `${errorMessage}`, //TODO: maybe need to display something else.
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
    } catch (error) {
      let errorMessage = "";
      if (typeof error === "object") {
        errorMessage =
          error?.response?.data?.error?.message ||
          error?.statusText ||
          error?.message ||
          "";
      } else {
        errorMessage = error;
      }
      docEditor.refreshHistory({
        error: `${errorMessage}`, //TODO: maybe need to display something else.
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
    } catch (error) {
      let errorMessage = "";
      if (typeof error === "object") {
        errorMessage =
          error?.response?.data?.error?.message ||
          error?.statusText ||
          error?.message ||
          "";
      } else {
        errorMessage = error;
      }

      docEditor.setHistoryData({
        error: `${errorMessage}`, //TODO: maybe need to display something else.
        version,
      });
    }
  };

  const onDocumentReady = () => {
    console.log("onDocumentReady", arguments, { docEditor });
    documentIsReady = true;

    config?.errorMessage && docEditor?.showMessage(config.errorMessage);

    // if (isSharingAccess) {
    //   loadUsersRightsList(docEditor);
    // }

    assign(window, ["ASC", "Files", "Editor", "docEditor"], docEditor); //Do not remove: it's for Back button on Mobile App
  };

  // const updateFavorite = (favorite) => {
  //   docEditor.setFavorite(favorite);
  // };

  const onMetaChange = (event) => {
    const newTitle = event.data.title;
    //const favorite = event.data.favorite;

    if (newTitle && newTitle !== docTitle) {
      setDocumentTitle(newTitle);
      docTitle = newTitle;
    }

    // if (!newTitle) {
    //   const onlyNumbers = new RegExp("^[0-9]+$");
    //   const isFileWithoutProvider = onlyNumbers.test(fileId);

    //   const convertFileId = isFileWithoutProvider ? +fileId : fileId;

    //   favorite
    //     ? markAsFavorite([convertFileId])
    //         .then(() => updateFavorite(favorite))
    //         .catch((error) => console.log("error", error))
    //     : removeFromFavorite([convertFileId])
    //         .then(() => updateFavorite(favorite))
    //         .catch((error) => console.log("error", error));
    // }
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

    if (!documentIsReady) {
      docTitle = title;
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
    docEditor = window.DocEditor.instances[EDITOR_ID];

    console.log("ONLYOFFICE Document Editor is ready", docEditor);
    const url = window.location.href;

    const index = url.indexOf("#message/");

    if (index > -1) {
      const splitUrl = url.split("#message/");

      if (splitUrl.length === 2) {
        const message = decodeURIComponent(splitUrl[1]).replace(/\+/g, " ");

        docEditor.showMessage(message);
        history.pushState({}, null, url.substring(0, index));
      } else {
        if (config?.Error) docEditor.showMessage(config.Error);
      }
    }
  };

  const onSDKRequestUsers = async () => {
    try {
      const res = await getSharedUsers(fileInfo.id);

      res.map((item) => {
        return usersInRoom.push({
          email: item.email,
          name: item.name,
        });
      });

      docEditor.setUsers({
        users: usersInRoom,
      });
    } catch (e) {
      console.error(e);
    }
  };

  const onSDKRequestSendNotify = async (event) => {
    const actionData = event.data.actionLink;
    const comment = event.data.message;
    const emails = event.data.emails;

    try {
      await sendEditorNotify(fileInfo.id, actionData, emails, comment);

      if (usersInRoom.length === 0) return;

      const usersNotFound = [...emails].filter((row) =>
        usersInRoom.every((value) => {
          return row !== value.email;
        })
      );

      usersNotFound.length > 0 &&
        docEditor.showMessage(
          t("UsersWithoutAccess", {
            users: usersNotFound,
          })
        );
    } catch (e) {
      toastr.error(e);
    }
  };

  const init = () => {
    try {
      if (isMobile) {
        config.type = "mobile";
      }

      let goBack;
      const url = window.location.href;
      const search = window.location.search;

      if (fileInfo) {
        let backUrl = "";

        if (fileInfo.rootFolderType === FolderType.Rooms) {
          backUrl = `/rooms/shared/${fileInfo.folderId}/filter?folder=${fileInfo.folderId}`;
        } else {
          backUrl = `/rooms/personal/filter?folder=${fileInfo.folderId}`;
        }

        const origin = url.substring(0, url.indexOf("/doceditor"));

        const editorGoBack = new URLSearchParams(search).get("editorGoBack");

        goBack =
          editorGoBack === "false"
            ? {}
            : {
                blank: true,
                requestClose: false,
                text: t("FileLocation"),
                url: `${combineUrl(origin, backUrl)}`,
              };
      }

      config.editorConfig.customization = {
        ...config.editorConfig.customization,
        goback: { ...goBack },
      };

      config.editorConfig.customization.uiTheme = getEditorTheme(user?.theme);

      config.document.info.favorite = null;

      // if (personal && !fileInfo) {
      //   //TODO: add conditions for SaaS
      //   config.document.info.favorite = null;
      // }

      if (url.indexOf("anchor") !== -1) {
        const splitUrl = url.split("anchor=");
        const decodeURI = decodeURIComponent(splitUrl[1]);
        const obj = JSON.parse(decodeURI);

        config.editorConfig.actionLink = {
          action: obj.action,
        };
      }

      let //onRequestSharingSettings,
        onRequestRename,
        onRequestSaveAs,
        onRequestInsertImage,
        onRequestMailMergeRecipients,
        onRequestCompareFile,
        onRequestRestore,
        onRequestHistory,
        onRequestReferenceData,
        onRequestUsers,
        onRequestSendNotify,
        onRequestCreateNew;

      if (successAuth && !user.isVisitor) {
        if (isDesktopEditor) {
          onRequestCreateNew = onSDKRequestCreateNew;
        } else {
          //FireFox security issue fix (onRequestCreateNew will be blocked)
          const documentType = config?.documentType || "word";
          const defaultFileName = getDefaultFileName();

          const url = new URL(
            combineUrl(
              window.location.origin,
              window.DocSpaceConfig?.proxy?.url,
              "/filehandler.ashx"
            )
          );
          url.searchParams.append("action", "create");
          url.searchParams.append("doctype", documentType);
          url.searchParams.append("title", defaultFileName);

          config.editorConfig.createUrl = url.toString();
        }
      }

      // if (isSharingAccess) {
      //   onRequestSharingSettings = onSDKRequestSharingSettings;
      // }

      if (userAccessRights.Rename) {
        onRequestRename = onSDKRequestRename;
      }

      if (userAccessRights.ReadHistory) {
        onRequestHistory = onSDKRequestHistory;
      }

      if (successAuth && !user.isVisitor) {
        onRequestSaveAs = onSDKRequestSaveAs;
      }

      if (successAuth) {
        onRequestInsertImage = onSDKRequestInsertImage;
        onRequestMailMergeRecipients = onSDKRequestMailMergeRecipients;
        onRequestCompareFile = onSDKRequestCompareFile;
      }

      if (userAccessRights.EditHistory) {
        onRequestRestore = onSDKRequestRestore;
      }

      if (!fileInfo?.providerKey) {
        onRequestReferenceData = onSDKRequestReferenceData;
      }

      if (fileInfo?.rootFolderType !== FolderType.USER) {
        onRequestUsers = onSDKRequestUsers;
        onRequestSendNotify = onSDKRequestSendNotify;
      }

      const events = {
        events: {
          onRequestReferenceData,
          onAppReady: onSDKAppReady,
          onDocumentStateChange: onDocumentStateChange,
          onMetaChange: onMetaChange,
          onDocumentReady: onDocumentReady,
          onInfo: onSDKInfo,
          onWarning: onSDKWarning,
          onError: onSDKError,
          // onRequestSharingSettings,
          onRequestRename,
          onMakeActionLink: onMakeActionLink,
          onRequestInsertImage,
          onRequestSaveAs,
          onRequestMailMergeRecipients,
          onRequestCompareFile,
          onRequestEditRights: onSDKRequestEditRights,
          onRequestHistory: onRequestHistory,
          onRequestHistoryClose: onSDKRequestHistoryClose,
          onRequestHistoryData: onSDKRequestHistoryData,
          onRequestRestore,
          onRequestUsers,
          onRequestSendNotify,
          onRequestCreateNew,
        },
      };

      newConfig = Object.assign(config, events);
    } catch (error) {
      toastr.error(error.message, null, 0, true);
    }
  };

  const errorMessage = () => {
    if (typeof error !== "string") return error.errorMessage;

    if (error === "restore-backup") return t("Common:PreparationPortalTitle");
    return error;
  };

  const additionalComponents =
    error && !error?.unAuthorized ? (
      <ErrorContainerBody
        headerText={t("Common:Error")}
        customizedBodyText={errorMessage()}
      />
    ) : (
      <>
        {/* {sharingDialog} */}
        {selectFileDialog}
        {selectFolderDialog}
      </>
    );

  return (
    <EditorWrapper
    // isVisibleSharingDialog={isVisible}
    >
      {newConfig && (
        <DocumentEditor
          id={EDITOR_ID}
          documentServerUrl={documentserverUrl}
          config={newConfig}
          height="100%"
          width="100%"
          events_onDocumentReady={onDocumentReady}
        ></DocumentEditor>
      )}

      {additionalComponents}

      <Toast />
    </EditorWrapper>
  );
}

export default withDialogs(Editor);
