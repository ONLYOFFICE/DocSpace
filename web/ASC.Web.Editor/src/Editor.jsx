import React, { useEffect, useState } from "react";

import Toast from "@appserver/components/toast";
import toastr from "studio/toastr";
import { toast } from "react-toastify";
import { Trans } from "react-i18next";
import Box from "@appserver/components/box";
import { regDesktop } from "@appserver/common/desktop";
import Loaders from "@appserver/common/components/Loaders";
import {
  combineUrl,
  getObjectByLocation,
  loadScript,
  //showLoader,
  //hideLoader,
} from "@appserver/common/utils";
import {
  getDocServiceUrl,
  openEdit,
  setEncryptionKeys,
  getEncryptionAccess,
  getFileInfo,
  getRecentFolderList,
  getFolderInfo,
  updateFile,
  removeFromFavorite,
  markAsFavorite,
  getPresignedUri,
  convertFile,
} from "@appserver/common/api/files";
import FilesFilter from "@appserver/common/api/files/filter";

import throttle from "lodash/throttle";
import { isIOS, deviceType } from "react-device-detect";
import { homepage } from "../package.json";

import { AppServerConfig, FolderType } from "@appserver/common/constants";
import SharingDialog from "files/SharingDialog";
import { getDefaultFileName, SaveAs, canConvert } from "files/utils";
import SelectFileDialog from "files/SelectFileDialog";
import SelectFolderDialog from "files/SelectFolderDialog";
import { StyledSelectFolder, StyledSelectFile } from "./StyledEditor";
import i18n from "./i18n";
import Text from "@appserver/components/text";
import TextInput from "@appserver/components/text-input";
import Checkbox from "@appserver/components/checkbox";
import { isMobile } from "react-device-detect";
import store from "studio/store";

const { auth: authStore } = store;

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
const url = window.location.href;
const filesUrl = url.substring(0, url.indexOf("/doceditor"));

toast.configure();

const Editor = () => {
  const urlParams = getObjectByLocation(window.location);
  const fileId = urlParams
    ? urlParams.fileId || urlParams.fileid || null
    : null;
  const version = urlParams ? urlParams.version || null : null;
  const doc = urlParams ? urlParams.doc || null : null;
  const isDesktop = window["AscDesktopEditor"] !== undefined;
  const view = url.indexOf("action=view") !== -1;

  const [isLoading, setIsLoading] = useState(true);
  const [isAuthenticated, setIsAuthenticated] = useState(true);
  const [titleSelectorFolder, setTitleSelectorFolder] = useState("");
  const [extension, setExtension] = useState();
  const [urlSelectorFolder, setUrlSelectorFolder] = useState("");
  const [openNewTab, setNewOpenTab] = useState(false);

  const throttledChangeTitle = throttle(() => changeTitle(), 500);

  useEffect(() => {
    init();
  }, []);

  const loadUsersRightsList = () => {
    SharingDialog.getSharingSettings(fileId).then((sharingSettings) => {
      docEditor.setSharingSettings({
        sharingSettings,
      });
    });
  };

  const insertImage = (link) => {
    docEditor.insertImage({
      c: "add",
      fileType: link.filetype,
      url: link.url,
    });
  };

  const mailMerge = (link) => {
    docEditor.setMailMergeRecipients({
      fileType: link.filetype,
      url: link.url,
    });
  };

  const compareFiles = (link) => {
    docEditor.setRevisedFile({
      fileType: link.filetype,
      url: link.url,
    });
  };
  const updateFavorite = (favorite) => {
    docEditor.setFavorite(favorite);
  };

  const getRecent = async (config) => {
    try {
      const recentFolderList = await getRecentFolderList();

      const filesArray = recentFolderList.files.slice(0, 25);

      const recentFiles = filesArray.filter(
        (file) =>
          file.rootFolderType !== FolderType.SHARE &&
          ((config.documentType === text && file.fileType === 7) ||
            (config.documentType === spreadSheet && file.fileType === 5) ||
            (config.documentType === presentation && file.fileType === 6))
      );

      const groupedByFolder = recentFiles.reduce((r, a) => {
        r[a.folderId] = [...(r[a.folderId] || []), a];
        return r;
      }, {});

      const requests = Object.entries(groupedByFolder).map((item) =>
        getFolderInfo(item[0])
          .then((folderInfo) =>
            Promise.resolve({
              files: item[1],
              folderInfo: folderInfo,
            })
          )
          .catch((e) => console.error(e))
      );

      let recent = [];

      let responses = await Promise.all(requests);

      for (let res of responses) {
        res.files.forEach((file) => {
          const convertedData = convertRecentData(file, res.folderInfo);
          if (Object.keys(convertedData).length !== 0)
            recent.push(convertedData);
        });
      }

      return recent;
    } catch (e) {
      console.error(e);
    }

    return null;
  };

  const initDesktop = (config) => {
    const isEncryption = config?.editorConfig["encryptionKeys"] !== undefined;

    regDesktop(
      user,
      isEncryption,
      config?.editorConfig.encryptionKeys,
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
        personal = authStore.settingsStore.personal;
        successAuth = !!user;
      } catch (e) {
        successAuth = false;
      }

      if (!doc && !successAuth) {
        window.open(
          combineUrl(AppServerConfig.proxyURL, "/login"),
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
            const needConvert = canConvert(fileInfo.fileExst);

            if (needConvert) {
              const convert = await convertFile(fileId, true);
              location.href = convert[0].result.webUrl;
            }
          }
        } catch (err) {
          console.error(err);
        }

        setIsAuthenticated(successAuth);
      }

      const config = await openEdit(fileId, version, doc, view);

      actionLink = config?.editorConfig?.actionLink;

      if (isDesktop) {
        initDesktop();
      }

      if (successAuth) {
        const recent = await getRecent(config); //TODO: too slow for 1st loading

        if (recent) {
          config.editorConfig = {
            ...config.editorConfig,
            recent: recent,
          };
        }
      }

      isSharingAccess = fileInfo && fileInfo.canShare;

      if (url.indexOf("action=view") !== -1) {
        config.editorConfig.mode = "view";
      }

      setIsLoading(false);

      loadScript(docApiUrl, "scripDocServiceAddress", () => onLoad(config));
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

  const convertRecentData = (file, folder) => {
    let obj = {};
    const folderName = folder.title;
    const fileName = file.title;

    if (+fileId !== file.id)
      obj = {
        folder: folderName,
        title: fileName,
        url: file.webUrl,
      };
    return obj;
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

  const onLoad = (config) => {
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
        onRequestCompareFile;

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
        },
      };

      const newConfig = Object.assign(config, events);

      docEditor = window.DocsAPI.DocEditor("editor", newConfig);
    } catch (error) {
      console.log(error);
      toastr.error(error.message, null, 0, true);
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
    }
  };

  const onSDKInfo = (event) => {
    console.log(
      "ONLYOFFICE Document Editor is opened in mode " + event.data.mode
    );
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
    updateFile(fileInfo.id, title);
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

  const onSDKRequestInsertImage = () => {
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
  const SelectFileHeader = () => {
    return (
      <StyledSelectFile>
        <Text className="editor-select-file_text">
          {filesType === mailMergeAction ? (
            getFileTypeTranslation()
          ) : (
            <Trans i18n={i18n} i18nKey="SelectFilesType" ns="Editor">
              Select files of type: {{ fileType: getFileTypeTranslation() }}
            </Trans>
          )}
        </Text>
      </StyledSelectFile>
    );
  };

  const insertImageActionProps = {
    isImageOnly: true,
  };

  const mailMergeActionProps = {
    isTablesOnly: true,
    searchParam: "xlsx",
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
    <Box
      widthProp="100vw"
      heightProp={isIPad() ? "calc(var(--vh, 1vh) * 100)" : "100vh"}
    >
      <Toast />

      {!isLoading ? (
        <>
          <div id="editor"></div>
          {isSharingAccess && (
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
              foldersType="exceptTrashFolder"
              {...fileTypeDetection()}
              header={<SelectFileHeader />}
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
        </>
      ) : (
        <Box paddingProp="16px">
          <Loaders.Rectangle height="96vh" />
        </Box>
      )}
    </Box>
  );
};

export default Editor;
