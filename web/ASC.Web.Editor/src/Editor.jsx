import React, { useEffect, useState } from "react";

import Toast from "@appserver/components/toast";
import toastr from "studio/toastr";
import { toast } from "react-toastify";
import { Trans } from "react-i18next";
import Box from "@appserver/components/box";
import { regDesktop } from "@appserver/common/desktop";
import {
  combineUrl,
  getObjectByLocation,
  loadScript,
  isRetina,
  getCookie,
  setCookie,
} from "@appserver/common/utils";
import {
  getDocServiceUrl,
  openEdit,
  setEncryptionKeys,
  getEncryptionAccess,
  getFileInfo,
  updateFile,
  removeFromFavorite,
  markAsFavorite,
  getPresignedUri,
  convertFile,
  checkFillFormDraft,
  getEditHistory,
  getEditDiff,
  restoreDocumentsVersion,
  getSettingsFiles,
} from "@appserver/common/api/files";
import FilesFilter from "@appserver/common/api/files/filter";

import throttle from "lodash/throttle";
import { isIOS, deviceType } from "react-device-detect";
import { homepage } from "../package.json";

import { AppServerConfig } from "@appserver/common/constants";
import SharingDialog from "files/SharingDialog";
import { getDefaultFileName, SaveAs } from "files/utils";
import SelectFileDialog from "files/SelectFileDialog";
import SelectFolderDialog from "files/SelectFolderDialog";
import PreparationPortalDialog from "studio/PreparationPortalDialog";
import { StyledSelectFolder } from "./StyledEditor";
import i18n from "./i18n";
import Text from "@appserver/components/text";
import TextInput from "@appserver/components/text-input";
import Checkbox from "@appserver/components/checkbox";
import { isMobile } from "react-device-detect";
import store from "studio/store";

import ThemeProvider from "@appserver/components/theme-provider";

const { auth: authStore } = store;
const theme = store.auth.settingsStore.theme;

let documentIsReady = false;

const text = "text";
const spreadSheet = "spreadsheet";
const presentation = "presentation";
const insertImageAction = "imageFileType";
const mailMergeAction = "mailMergeFileType";
const compareFilesAction = "documentsFileType";

let docTitle = null;
let actionLink;
let docSaved = null;
let docEditor;
let fileInfo;
let successAuth;
let isSharingAccess;
let user = null;
let personal;
let config;
let url = window.location.href;
const filesUrl = url.substring(0, url.indexOf("/doceditor"));
const doc = url.indexOf("doc=") !== -1 ? url.split("doc=")[1] : null;

toast.configure();

const Editor = () => {
  const urlParams = getObjectByLocation(window.location);
  const decodedId = urlParams
    ? urlParams.fileId || urlParams.fileid || null
    : null;
  let fileId =
    typeof decodedId === "string" ? encodeURIComponent(decodedId) : decodedId;
  let version = urlParams ? urlParams.version || null : null;
  const doc = urlParams ? urlParams.doc || null : null;
  const isDesktop = window["AscDesktopEditor"] !== undefined;
  const view = url.indexOf("action=view") !== -1;

  const [isLoading, setIsLoading] = useState(true);
  const [isAuthenticated, setIsAuthenticated] = useState(true);
  const [titleSelectorFolder, setTitleSelectorFolder] = useState("");
  const [extension, setExtension] = useState();
  const [urlSelectorFolder, setUrlSelectorFolder] = useState("");
  const [openNewTab, setNewOpenTab] = useState(false);
  const [typeInsertImageAction, setTypeInsertImageAction] = useState();
  const throttledChangeTitle = throttle(() => changeTitle(), 500);

  const [
    preparationPortalDialogVisible,
    setPreparationPortalDialogVisible,
  ] = useState(false);

  let filesSettings;

  useEffect(() => {
    const tempElm = document.getElementById("loader");
    tempElm.style.backgroundColor = theme.backgroundColor;
  }, []);

  useEffect(() => {
    if (isRetina() && getCookie("is_retina") == null) {
      setCookie("is_retina", true, { path: "/" });
    }

    init();
  }, []);

  const canConvert = (extension) => {
    const array = filesSettings?.extsMustConvert || [];
    const result = array.findIndex((item) => item === extension);
    return result === -1 ? false : true;
  };

  const loadUsersRightsList = () => {
    SharingDialog.getSharingSettings(fileId).then((sharingSettings) => {
      docEditor.setSharingSettings({
        sharingSettings,
      });
    });
  };

  const insertImage = (link) => {
    const token = link.token;

    docEditor.insertImage({
      ...typeInsertImageAction,
      fileType: link.filetype,
      ...(token && { token }),
      url: link.url,
    });
  };

  const mailMerge = (link) => {
    const token = link.token;

    docEditor.setMailMergeRecipients({
      fileType: link.filetype,
      ...(token && { token }),
      url: link.url,
    });
  };

  const compareFiles = (link) => {
    const token = link.token;

    docEditor.setRevisedFile({
      fileType: link.filetype,
      ...(token && { token }),
      url: link.url,
    });
  };
  const updateFavorite = (favorite) => {
    docEditor.setFavorite(favorite);
  };

  const initDesktop = (cfg) => {
    const encryptionKeys = cfg?.editorConfig?.encryptionKeys;

    regDesktop(
      user,
      !!encryptionKeys,
      encryptionKeys,
      (keys) => {
        setEncryptionKeys(keys);
      },
      true,
      (callback) => {
        getEncryptionAccess(fileId)
          .then((keys) => {
            var data = {
              keys,
            };

            callback(data);
          })
          .catch((error) => {
            console.log(error);
            toastr.error(
              typeof error === "string" ? error : error.message,
              null,
              0,
              true
            );
          });
      },
      i18n.t
    );
  };

  const convertDocumentUrl = async () => {
    const convert = await convertFile(fileId, null, true);
    return convert && convert[0]?.result;
  };

  const init = async () => {
    try {
      if (!fileId) return;

      console.log(
        `Editor componentDidMount fileId=${fileId}, version=${version}, doc=${doc}`
      );

      if (isIPad()) {
        const vh = window.innerHeight * 0.01;
        document.documentElement.style.setProperty("--vh", `${vh}px`);
      }

      //showLoader();
      const docApiUrl = await getDocServiceUrl();

      try {
        await authStore.init(true);
        user = authStore.userStore.user;
        if (user) filesSettings = await getSettingsFiles();
        personal = authStore.settingsStore.personal;
        successAuth = !!user;

        const { socketHelper } = authStore.settingsStore;
        socketHelper.emit({
          command: "subscribe",
          data: "backup-restore",
        });
        socketHelper.on("restore-backup", () => {
         
          setPreparationPortalDialogVisible(true);
        });
      } catch (e) {
        successAuth = false;
      }

      if (!doc && !successAuth) {
        localStorage.setItem("redirectPath", window.location.href);

        window.open(
          combineUrl(
            AppServerConfig.proxyURL,
            personal ? "/sign-in" : "/login"
          ),
          "_self",
          "",
          true
        );
        return;
      }
      if (successAuth) {
        try {
          fileInfo = await getFileInfo(fileId);

          if (url.indexOf("#message/") > -1) {
            if (canConvert(fileInfo.fileExst)) {
              const result = await convertDocumentUrl();

              const splitUrl = url.split("#message/");

              if (result) {
                const newUrl = `${result.webUrl}#message/${splitUrl[1]}`;

                history.pushState({}, null, newUrl);

                fileInfo = result;
                url = newUrl;
                fileId = result.id;
                version = result.version;
              }
            }
          }
        } catch (err) {
          console.error(err);
        }

        setIsAuthenticated(successAuth);
      }

      config = await openEdit(fileId, version, doc, view);

      if (
        !view &&
        fileInfo &&
        fileInfo.canWebRestrictedEditing &&
        fileInfo.canFillForms &&
        !fileInfo.canEdit
      ) {
        try {
          const formUrl = await checkFillFormDraft(fileId);
          history.pushState({}, null, formUrl);

          document.location.reload();
        } catch (err) {
          console.error(err);
        }
      }

      actionLink = config?.editorConfig?.actionLink;

      if (isDesktop) {
        initDesktop(config);
      }

      isSharingAccess = fileInfo && fileInfo.canShare;

      if (view) {
        config.editorConfig.mode = "view";
      }

      setIsLoading(false);

      loadScript(docApiUrl, "scripDocServiceAddress", () => onLoad());
    } catch (error) {
      console.log(error);
      toastr.error(
        typeof error === "string" ? error : error.message,
        null,
        0,
        true
      );
    }
  };

  const isIPad = () => {
    return isIOS && deviceType === "tablet";
  };

  const setFavicon = (documentType) => {
    const favicon = document.getElementById("favicon");
    if (!favicon) return;
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

    if (icon) favicon.href = `${homepage}/images/${icon}`;
  };

  const changeTitle = () => {
    docSaved ? setDocumentTitle(docTitle) : setDocumentTitle(`*${docTitle}`);
  };

  const setDocumentTitle = (subTitle = null) => {
    //const { isAuthenticated, settingsStore, product: currentModule } = auth;
    //const { organizationName } = settingsStore;
    const organizationName = "ONLYOFFICE"; //TODO: Replace to API variant
    const moduleTitle = "Documents"; //TODO: Replace to API variant

    let title;
    if (subTitle) {
      if (isAuthenticated && moduleTitle) {
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

  const onLoad = () => {
    try {
      if (!window.DocsAPI) throw new Error("DocsAPI is not defined");

      console.log("Editor config: ", config);

      docTitle = config.document.title;

      setFavicon(config.documentType);
      setDocumentTitle(docTitle);

      if (isMobile) {
        config.type = "mobile";
      }

      let goBack;

      if (fileInfo) {
        const filterObj = FilesFilter.getDefault();
        filterObj.folder = fileInfo.folderId;
        const urlFilter = filterObj.toUrlParams();

        goBack = {
          blank: true,
          requestClose: false,
          text: i18n.t("FileLocation"),
          url: `${combineUrl(filesUrl, `/filter?${urlFilter}`)}`,
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

      docEditor = window.DocsAPI.DocEditor("editor", newConfig);
    } catch (error) {
      console.log(error);
      toastr.error(error.message, null, 0, true);
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

  const onSDKRequestHistoryClose = () => {
    document.location.reload();
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

  const onSDKAppReady = () => {
    console.log("ONLYOFFICE Document Editor is ready");

    const index = url.indexOf("#message/");
    if (index > -1) {
      const splitUrl = url.split("#message/");
      const message = decodeURIComponent(splitUrl[1]).replaceAll("+", " ");
      history.pushState({}, null, url.substring(0, index));
      docEditor.showMessage(message);
    } else {
      if (config?.Error) docEditor.showMessage(config.Error);
    }

    const tempElm = document.getElementById("loader");
    if (tempElm) {
      tempElm.outerHTML = "";
    }
  };

  const onSDKInfo = (event) => {
    console.log(
      "ONLYOFFICE Document Editor is opened in mode " + event.data.mode
    );
  };

  const onSDKRequestEditRights = async () => {
    console.log("ONLYOFFICE Document Editor requests editing rights");
    const index = url.indexOf("&action=view");

    if (index) {
      let convertUrl = url.substring(0, index);

      if (canConvert(fileInfo.fileExst)) {
        const newUrl = await convertDocumentUrl();
        if (newUrl) {
          convertUrl = newUrl.webUrl;
        }
      }

      history.pushState({}, null, convertUrl);
      document.location.reload();
    }
  };

  const [isVisible, setIsVisible] = useState(false);
  const [isFileDialogVisible, setIsFileDialogVisible] = useState(false);
  const [isFolderDialogVisible, setIsFolderDialogVisible] = useState(false);
  const [filesType, setFilesType] = useState("");

  const onSDKRequestSharingSettings = () => {
    setIsVisible(true);
  };

  const onSDKRequestRename = (event) => {
    const title = event.data;

    updateFile(fileId, title);
  };

  const onMakeActionLink = (event) => {
    var ACTION_DATA = event.data;

    const link = generateLink(ACTION_DATA);

    const urlFormation = !actionLink ? url : url.split("&anchor=")[0];

    const linkFormation = `${urlFormation}&anchor=${link}`;

    docEditor.setActionLink(linkFormation);
  };

  const generateLink = (actionData) => {
    return encodeURIComponent(JSON.stringify(actionData));
  };

  const onCancel = () => {
    setIsVisible(false);
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

  const onDocumentStateChange = (event) => {
    if (!documentIsReady) return;

    docSaved = !event.data;
    throttledChangeTitle();
  };

  const onDocumentReady = () => {
    documentIsReady = true;

    if (isSharingAccess) {
      loadUsersRightsList();
    }
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

      const convertFileId = isFileWithoutProvider
        ? +fileId
        : decodeURIComponent(fileId);

      favorite
        ? markAsFavorite([convertFileId])
            .then(() => updateFavorite(favorite))
            .catch((error) => console.log("error", error))
        : removeFromFavorite([convertFileId])
            .then(() => updateFavorite(favorite))
            .catch((error) => console.log("error", error));
    }
  };

  const onSDKRequestInsertImage = (event) => {
    setTypeInsertImageAction(event.data);
    setFilesType(insertImageAction);
    setIsFileDialogVisible(true);
  };

  const onSDKRequestMailMergeRecipients = () => {
    setFilesType(mailMergeAction);
    setIsFileDialogVisible(true);
  };

  const onSDKRequestCompareFile = () => {
    setFilesType(compareFilesAction);
    setIsFileDialogVisible(true);
  };
  const onSelectFile = async (file) => {
    try {
      const link = await getPresignedUri(file.id);

      if (filesType === insertImageAction) insertImage(link);
      if (filesType === mailMergeAction) mailMerge(link);
      if (filesType === compareFilesAction) compareFiles(link);
    } catch (e) {
      console.error(e);
    }
  };

  const onCloseFileDialog = () => {
    setIsFileDialogVisible(false);
  };

  const onSDKRequestSaveAs = (event) => {
    setTitleSelectorFolder(event.data.title);
    setUrlSelectorFolder(event.data.url);
    setExtension(event.data.title.split(".").pop());

    setIsFolderDialogVisible(true);
  };

  const onCloseFolderDialog = () => {
    setIsFolderDialogVisible(false);
    setNewOpenTab(false);
  };

  const getSavingInfo = async (title, folderId) => {
    const savingInfo = await SaveAs(
      title,
      urlSelectorFolder,
      folderId,
      openNewTab
    );

    if (savingInfo) {
      const convertedInfo = savingInfo.split(": ").pop();
      docEditor.showMessage(convertedInfo);
    }
  };
  const onClickSaveSelectFolder = (e, folderId) => {
    const currentExst = titleSelectorFolder.split(".").pop();

    const title =
      currentExst !== extension
        ? titleSelectorFolder.concat(`.${extension}`)
        : titleSelectorFolder;

    if (openNewTab) {
      SaveAs(title, urlSelectorFolder, folderId, openNewTab);
    } else {
      getSavingInfo(title, folderId);
    }
  };

  const onChangeInput = (e) => {
    setTitleSelectorFolder(e.target.value);
  };

  const onClickCheckbox = () => {
    setNewOpenTab(!openNewTab);
  };

  const getFileTypeTranslation = () => {
    switch (filesType) {
      case mailMergeAction:
        return i18n.t("MailMergeFileType");
      case insertImageAction:
        return i18n.t("ImageFileType");
      case compareFilesAction:
        return i18n.t("DocumentsFileType");
    }
  };
  const selectFilesListTitle = () => {
    return (
      <>
        {filesType === mailMergeAction ? (
          getFileTypeTranslation()
        ) : (
          <Trans i18n={i18n} i18nKey="SelectFilesType" ns="Editor">
            Select files of type: {{ fileType: getFileTypeTranslation() }}
          </Trans>
        )}
      </>
    );
  };

  const insertImageActionProps = {
    isImageOnly: true,
  };

  const mailMergeActionProps = {
    isTablesOnly: true,
    searchParam: ".xlsx",
  };
  const compareFilesActionProps = {
    isDocumentsOnly: true,
  };

  const fileTypeDetection = () => {
    if (filesType === insertImageAction) {
      return insertImageActionProps;
    }
    if (filesType === mailMergeAction) {
      return mailMergeActionProps;
    }
    if (filesType === compareFilesAction) {
      return compareFilesActionProps;
    }
  };

  return (
    <ThemeProvider theme={theme}>
      <Box
        widthProp="100vw"
        heightProp={isIPad() ? "calc(var(--vh, 1vh) * 100)" : "100vh"}
      >
        <Toast />
        {!isLoading ? (
          <>
            <div id="editor"></div>
            {isSharingAccess && isVisible && (
              <SharingDialog
                isVisible={isVisible}
                sharingObject={fileInfo}
                onCancel={onCancel}
                onSuccess={loadUsersRightsList}
              />
            )}

            {isFileDialogVisible && (
              <SelectFileDialog
                resetTreeFolders
                onSelectFile={onSelectFile}
                isPanelVisible={isFileDialogVisible}
                onClose={onCloseFileDialog}
                foldersType="exceptPrivacyTrashFolders"
                {...fileTypeDetection()}
                titleFilesList={selectFilesListTitle()}
                headerName={i18n.t("SelectFileTitle")}
              />
            )}

            {isFolderDialogVisible && (
              <SelectFolderDialog
                resetTreeFolders
                showButtons
                isPanelVisible={isFolderDialogVisible}
                isSetFolderImmediately
                asideHeightContent="calc(100% - 50px)"
                onClose={onCloseFolderDialog}
                foldersType="exceptSortedByTags"
                onSave={onClickSaveSelectFolder}
                header={
                  <StyledSelectFolder>
                    <Text className="editor-select-folder_text">
                      {i18n.t("FileName")}
                    </Text>
                    <TextInput
                      className="editor-select-folder_text-input"
                      scale
                      onChange={onChangeInput}
                      value={titleSelectorFolder}
                    />
                  </StyledSelectFolder>
                }
                headerName={i18n.t("FolderForSave")}
                {...(extension !== "fb2" && {
                  footer: (
                    <StyledSelectFolder>
                      <Checkbox
                        className="editor-select-folder_checkbox"
                        label={i18n.t("OpenSavedDocument")}
                        onChange={onClickCheckbox}
                        isChecked={openNewTab}
                      />
                    </StyledSelectFolder>
                  ),
                })}
              />
            )}

            {preparationPortalDialogVisible && (
              <PreparationPortalDialog visible={preparationPortalDialogVisible} />
            )}
          </>
        ) : (
          <></>
        )}
      </Box>
    </ThemeProvider>
  );
};

export default Editor;
