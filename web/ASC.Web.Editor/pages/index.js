import Head from "next/head";
import { useState, useEffect } from "react";
import { useRouter } from "next/router";
import { initSSR } from "@appserver/common/api/client";
import Script from "next/script";
import { isMobile } from "react-device-detect";
import FilesFilter from "@appserver/common/api/files/filter";
import combineUrl from "@appserver/common/utils/combineUrl";
import { AppServerConfig } from "@appserver/common/constants";
import { homepage } from "../package.json";
import throttle from "lodash/throttle";
import Loader from "@appserver/components/loader";

// import Toast from "@appserver/components/toast";
// import toastr from "../../../../../ASC.Web.Client/src/helpers/toastr";
// import { toast } from "react-toastify";

import {
  getDocServiceUrl,
  getFileInfo,
  checkFillFormDraft,
  restoreDocumentsVersion,
  openEdit,
  markAsFavorite,
  removeFromFavorite,
  getEditDiff,
  getEditHistory,
  updateFile,
} from "@appserver/common/api/files";
import { getSettings } from "@appserver/common/api/settings";
import { getUser } from "@appserver/common/api/people";

const LoaderComponent = (
  <Loader
    type="rombs"
    style={{
      position: "absolute",
      bottom: "42%",
      height: "170px",
      left: "50%",
    }}
  />
);
const loadScript = (url, id, onLoad, onError) => {
  try {
    const script = document.createElement("script");
    script.setAttribute("type", "text/javascript");
    script.setAttribute("id", id);

    if (onLoad) script.onload = onLoad;
    if (onError) script.onerror = onError;

    script.src = url;
    script.async = true;

    document.body.appendChild(script);
  } catch (e) {
    console.error(e);
  }
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
  console.log(icon);
  if (icon) favicon.href = `${homepage}/images/${icon}`;
}; //TODO: to fix

const getDefaultFileName = (format) => {
  switch (format) {
    case "docx":
      return "SSR New Document";
    case "xlsx":
      return "SSR New Spreadsheet";
    case "pptx":
      return "SSR New Presentation";
    case "docxf":
      return "SSR New MasterForm";
    default:
      return " SSR New Folder";
  }
}; // TODO:  i18n

const onSDKInfo = (event) => {
  console.log(
    "ONLYOFFICE Document Editor is opened in mode " + event.data.mode
  );
};

const onSDKRequestEditRights = async () => {
  console.log("ONLYOFFICE Document Editor requests editing rights");
  // const index = url.indexOf("&action=view");

  // if (index) {
  //   let convertUrl = url.substring(0, index);

  //   if (canConvert(fileInfo.fileExst)) {
  //     convertUrl = await convertDocumentUrl();
  //   }

  //   history.pushState({}, null, convertUrl);
  //   document.location.reload();
  // }
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
          // toastr.error(
          //   typeof error === "string" ? error : error.message,
          //   null,
          //   0,
          //   true
          // );
        });
    },
    i18n.t
  );
};

const text = "text";
const presentation = "presentation";
const insertImageAction = "imageFileType";
let documentIsReady = false; // move to state?
let docSaved = null; // move to state?
let docTitle = null;
let docEditor;

export default function Home({
  fileInfo,
  docApiUrl,
  config,
  personal,
  successAuth,
  isSharingAccess,
  //docEditor: docEditorSSR,
  user,
  url,
  doc,
  fileId, //
  actionLink,
  error,
  needLoader,
}) {
  const [titleSelectorFolder, setTitleSelectorFolder] = useState("");
  const [urlSelectorFolder, setUrlSelectorFolder] = useState("");
  const [extension, setExtension] = useState();
  const [isFolderDialogVisible, setIsFolderDialogVisible] = useState(false);
  const [typeInsertImageAction, setTypeInsertImageAction] = useState();
  const [filesType, setFilesType] = useState("");
  const [isFileDialogVisible, setIsFileDialogVisible] = useState(false); // посмотреть
  const [isVisible, setIsVisible] = useState(false);
  const [isLoaded, setIsLoaded] = useState(false);

  const throttledChangeTitle = throttle(() => changeTitle(), 500);
  //docEditor = docEditorSSR;
  //console.log("render editor", docEditor, docEditorSSR);
  const router = useRouter();
  useEffect(() => {}, []);

  const onSDKRequestHistoryClose = () => {
    document.location.reload();
  };

  const onSDKRequestEditRights = async () => {
    console.log("ONLYOFFICE Document Editor requests editing rights");
    const index = url.indexOf("&action=view");

    if (index) {
      let convertUrl = url.substring(0, index);

      // if (canConvert(fileInfo.fileExst)) {
      //   convertUrl = await convertDocumentUrl();
      // } // TODO: need move can canConvert from docsevicestore

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

  const onSDKRequestSharingSettings = () => {
    setIsVisible(true);
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

  const onSDKRequestCompareFile = () => {
    setFilesType(compareFilesAction);
    setIsFileDialogVisible(true);
  };

  const onSDKRequestMailMergeRecipients = () => {
    setFilesType(mailMergeAction);
    setIsFileDialogVisible(true);
  };

  const onSDKRequestInsertImage = (event) => {
    setTypeInsertImageAction(event.data);
    setFilesType(insertImageAction);
    setIsFileDialogVisible(true);
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
  }; // +++

  const getCurrentDocumentVersion = (fileHistory, historyLength) => {
    return url.indexOf("&version=") !== -1
      ? +url.split("&version=")[1]
      : fileHistory[historyLength - 1].version;
  }; // +++

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
  }; // +++

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
  }; // +++

  const loadUsersRightsList = () => {
    // SharingDialog.getSharingSettings(fileId).then((sharingSettings) => {
    //   docEditor.setSharingSettings({
    //     sharingSettings,
    //   });
    // }); TODO:
    console.log("loadUsersRightsList");
  };

  const onDocumentReady = () => {
    documentIsReady = true;

    if (isSharingAccess) {
      loadUsersRightsList();
    }
  };

  const updateFavorite = (favorite) => {
    docEditor.setFavorite(favorite);
  }; //+++

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
  }; // +++

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
  }; //+++

  const changeTitle = () => {
    docSaved ? setDocumentTitle(docTitle) : setDocumentTitle(`*${docTitle}`);
  }; // +++

  const onDocumentStateChange = (event) => {
    if (!documentIsReady) return;

    docSaved = !event.data;
    throttledChangeTitle();
  }; //+++

  const onSDKRequestSaveAs = (event) => {
    setTitleSelectorFolder(event.data.title);
    setUrlSelectorFolder(event.data.url);
    setExtension(event.data.title.split(".").pop());
    setIsFolderDialogVisible(true);
  }; // +++

  const onSDKAppReady = () => {
    console.log("ONLYOFFICE Document Editor is ready");

    const index = url.indexOf("#message/");
    if (index > -1) {
      const splitUrl = url.split("#message/");
      const message = decodeURIComponent(splitUrl[1]).replaceAll("+", " ");
      history.pushState({}, null, url.substring(0, index));
      docEditor.showMessage(message);
    }

    // const tempElm = document.getElementById("loader");
    // if (tempElm) {
    //   tempElm.outerHTML = "";
    // } not need to ssr
  }; // +++

  const onLoad = () => {
    try {
      if (!window.DocsAPI) throw new Error("DocsAPI is not defined");

      console.log("Editor config: ", config);

      // TODO: need to fix
      const docTitle = config.document.title;
      setDocumentTitle(docTitle);

      if (isMobile) {
        config.type = "mobile";
        // not sure what is needed, as it arrives in the config
      }

      let goBack;
      const url = window.location.href;

      if (fileInfo) {
        const filterObj = FilesFilter.getDefault();
        filterObj.folder = fileInfo.folderId;
        const urlFilter = filterObj.toUrlParams();

        const filesUrl = url.substring(0, url.indexOf("/doceditor"));

        goBack = {
          blank: true,
          requestClose: false,
          text: "SSR TEST", ///i18n.t("FileLocation"), TODO: connect i18
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

      //let url = window.location.href;
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
        onRequestSharingSettings = onSDKRequestSharingSettings; // +++
      }

      if (fileInfo && fileInfo.canEdit) {
        onRequestRename = onSDKRequestRename; // +++
      }

      if (successAuth) {
        onRequestSaveAs = onSDKRequestSaveAs; //+++
        onRequestInsertImage = onSDKRequestInsertImage; // +++
        onRequestMailMergeRecipients = onSDKRequestMailMergeRecipients; // +++
        onRequestCompareFile = onSDKRequestCompareFile; // +++
      }

      if (!!config.document.permissions.changeHistory) {
        onRequestRestore = onSDKRequestRestore; // +++
      }
      const events = {
        events: {
          onAppReady: onSDKAppReady, // +++
          onDocumentStateChange: onDocumentStateChange, // +++
          onMetaChange: onMetaChange, // +++
          onDocumentReady: onDocumentReady, // ++-
          onInfo: onSDKInfo, // +++
          onWarning: onSDKWarning, // +++
          onError: onSDKError, // +++
          onRequestSharingSettings, // +++
          onRequestRename, // +++
          onMakeActionLink: onMakeActionLink, // +++
          onRequestInsertImage, //+++
          onRequestSaveAs, // +++
          onRequestMailMergeRecipients, // +++
          onRequestCompareFile, // +++
          onRequestEditRights: onSDKRequestEditRights, // // TODO: need move can canConvert from docsevicestore
          onRequestHistory: onSDKRequestHistory, // +++
          onRequestHistoryClose: onSDKRequestHistoryClose, // +++
          onRequestHistoryData: onSDKRequestHistoryData, // +++
          onRequestRestore, // +++
        },
      };

      const newConfig = Object.assign(config, events);

      docEditor = window.DocsAPI.DocEditor("editor", newConfig);
      console.log("docEditor", docEditor);
      setIsLoaded(true);
    } catch (error) {
      console.log(error, "init error");
      //toastr.error(error.message, null, 0, true);
    }
  };

  useEffect(() => {
    console.log("useEffect", isLoaded);
    setFavicon(config?.documentType);

    if (error) {
      error?.unAuthorized &&
        error?.redirectPath &&
        (window.location.href = error?.redirectPath);
    }
  });

  return (
    <div style={{ height: "100vh" }}>
      <Head title="Loading...">
        <title>Loading...</title>
        <link id="favicon" rel="shortcut icon" href="/favicon.ico" />
      </Head>
      {needLoader ? (
        LoaderComponent
      ) : (
        <>
          <div id="editor"></div>
          {!isLoaded && LoaderComponent}
          <Script
            async
            defer
            type={"text/javascript"}
            id="scriptDocServiceAddress"
            src={docApiUrl}
            onLoad={() => onLoad()}
            onError={() => console.log("error load")}
            //strategy={"beforeInteractive"}
          />
        </>
      )}
    </div>
  );
}

export async function getServerSideProps({ params, req, query }) {
  const { headers, url } = req;
  const { version, desktop: isDesktop } = query;
  let error = null;

  initSSR(headers);

  try {
    //const doc = url.indexOf("doc=") !== -1 ? url.split("doc=")[1] : null;??
    const decodedId = query.fileId || query.fileid || null;
    const fileId =
      typeof decodedId === "string" ? encodeURIComponent(decodedId) : decodedId;

    if (!fileId) {
      return { props: { needLoader: true } };
    } // TODO: t()

    const doc = query?.doc || null; // TODO: need to check
    const view = url.indexOf("action=view") !== -1;
    const fileVersion = version || null;

    const [user, settings] = await Promise.all([getUser(), getSettings()]);

    const successAuth = !!user;
    const personal = settings?.personal;

    if (!successAuth && !doc) {
      error = {
        unAuthorized: true,
        redirectPath: combineUrl(
          AppServerConfig.proxyURL,
          personal ? "/sign-in" : "/login"
        ),
      };
      return { props: { error } };
    }

    let [config, docApiUrl, fileInfo] = await Promise.all([
      openEdit(fileId, fileVersion, doc, view),
      getDocServiceUrl(),
      getFileInfo(fileId),
    ]);

    const actionLink = config?.editorConfig?.actionLink;

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
    return { props: { error } };
  }
}
