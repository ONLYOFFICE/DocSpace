import React, { useEffect, useState } from "react";

import Toast from "@appserver/components/toast";
import toastr from "studio/toastr";
import { toast } from "react-toastify";

import Box from "@appserver/components/box";
import { regDesktop } from "@appserver/common/desktop";
import Loaders from "@appserver/common/components/Loaders";
import {
  combineUrl,
  getObjectByLocation,
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
} from "@appserver/common/api/files";
import { checkIsAuthenticated } from "@appserver/common/api/user";
import { getUser } from "@appserver/common/api/people";
import FilesFilter from "@appserver/common/api/files/filter";

import throttle from "lodash/throttle";
import { isIOS, deviceType } from "react-device-detect";
import { homepage } from "../package.json";

import { AppServerConfig } from "@appserver/common/constants";
import SharingDialog from "files/SharingDialog";
import { getDefaultFileName } from "files/utils";
import i18n from "./i18n";
import { FolderType } from "@appserver/common/constants";

let documentIsReady = false;

const text = "text";
const spreadSheet = "spreadsheet";
const presentation = "presentation";

let docTitle = null;
let fileType = null;
let config;
let docSaved = null;
let docEditor;
let fileInfo;
let successAuth;
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

  const [isLoading, setIsLoading] = useState(true);
  const [isAuthenticated, setIsAuthenticated] = useState(true);

  const throttledChangeTitle = throttle(
    () => changeTitle(docSaved, docTitle),
    500
  );

  useEffect(() => {
    init();
  }, []);

  const updateUsersRightsList = () => {
    SharingDialog.getSharingSettings(fileId).then((sharingSettings) => {
      docEditor.setSharingSettings({
        sharingSettings,
      });
    });
  };

  const updateFavorite = (favorite) => {
    docEditor.setFavorite(favorite);
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
      successAuth = await checkIsAuthenticated();

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
        } catch (err) {
          console.error(err);
        }

        setIsAuthenticated(successAuth);
      }

      config = await openEdit(fileId, version, doc);

      if (isDesktop) {
        const isEncryption =
          config.editorConfig["encryptionKeys"] !== undefined;
        const user = await getUser();

        regDesktop(
          user,
          isEncryption,
          config.editorConfig.encryptionKeys,
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
      }

      if (successAuth) {
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

          try {
            let responses = await Promise.all(requests);

            for (let i = 0; i < responses.length; i++) {
              const res = responses[i];

              res.files.forEach((file) => {
                const convertedData = convertRecentData(file, res.folderInfo);
                if (Object.keys(convertedData).length !== 0)
                  recent.push(convertedData);
              });
            }
          } catch (e) {
            console.error(e);
          }

          config.editorConfig = {
            ...config.editorConfig,
            recent: recent,
          };
        } catch (e) {
          console.error(e);
        }
      }

      if (
        config &&
        config.document.permissions.edit &&
        config.document.permissions.modifyFilter &&
        fileInfo
      ) {
        const sharingSettings = await SharingDialog.getSharingSettings(fileId);
        config.document.info = {
          ...config.document.info,
          sharingSettings,
        };
      }

      if (url.indexOf("action=view") !== -1) {
        config.editorConfig.mode = "view";
      }

      setIsLoading(false);

      loadDocApi(docApiUrl, () => onLoad(config));
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
    const url = file.webUrl;

    if (+fileId !== file.id)
      obj = {
        folder: folderName,
        title: fileName,
        url: url,
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

  const changeTitle = (docSaved, docTitle) => {
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

  const loadDocApi = (docApiUrl, onLoadCallback) => {
    const script = document.createElement("script");
    script.setAttribute("type", "text/javascript");
    script.setAttribute("id", "scripDocServiceAddress");

    script.onload = onLoadCallback;

    script.src = docApiUrl;
    script.async = true;

    console.log("PureEditor componentDidMount: added script");
    document.body.appendChild(script);
  };

  const onLoad = (config) => {
    console.log("Editor config: ", config);

    try {
      console.log(config);

      docTitle = config.document.title;
      fileType = config.document.fileType;

      setFavicon(config.documentType);
      setDocumentTitle(docTitle);

      if (window.innerWidth < 720) {
        config.type = "mobile";
      }

      let goback;

      if (fileInfo) {
        const filterObj = FilesFilter.getDefault();
        filterObj.folder = fileInfo.folderId;
        const urlFilter = filterObj.toUrlParams();

        goback = {
          blank: true,
          requestClose: false,
          text: i18n.t("FileLocation"),
          url: `${combineUrl(filesUrl, `/filter?${urlFilter}`)}`,
        };
      }

      config.editorConfig.customization = {
        ...config.editorConfig.customization,
        goback,
      };

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

        config.editorConfig.createUrl = combineUrl(
          window.location.origin,
          AppServerConfig.proxyURL,
          "products/files/",
          `/httphandlers/filehandler.ashx?action=create&doctype=text&title=${encodeURIComponent(
            defaultFileName
          )}`
        );
      }

      let onRequestSharingSettings;
      let onRequestRename;

      if (fileInfo && config.document.permissions.modifyFilter) {
        onRequestSharingSettings = onSDKRequestSharingSettings;
        onRequestRename = onSDKRequestRename;
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
        },
      };

      const newConfig = Object.assign(config, events);

      if (!window.DocsAPI) throw new Error("DocsAPI is not defined");

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
      message && toastr.info(message);
      history.pushState({}, null, url.substring(0, index));
    }
  };

  const onSDKInfo = (event) => {
    console.log(
      "ONLYOFFICE Document Editor is opened in mode " + event.data.mode
    );
  };

  const [isVisible, setIsVisible] = useState(false);

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

    const urlFormation = !config.editorConfig.actionLink
      ? url
      : url.split("&anchor=")[0];

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
  };

  const onMetaChange = (event) => {
    const newTitle = event.data.title;
    const favorite = event.data.favorite;

    if (newTitle && newTitle !== docTitle) {
      setDocumentTitle(newTitle);
      docTitle = newTitle;
    }

    if (!newTitle)
      favorite
        ? markAsFavorite([+fileId])
            .then(() => updateFavorite(favorite))
            .catch((error) => console.log("error", error))
        : removeFromFavorite([+fileId])
            .then(() => updateFavorite(favorite))
            .catch((error) => console.log("error", error));
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
          {fileInfo && (
            <SharingDialog
              isVisible={isVisible}
              sharingObject={fileInfo}
              onCancel={onCancel}
              onSuccess={updateUsersRightsList}
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
