import Head from "next/head";
import { useState, useEffect } from "react";
import { useRouter } from "next/router";
import request from "@appserver/common/api/client-ssr";
import Script from "next/script";
import { isMobile } from "react-device-detect";
import FilesFilter from "@appserver/common/api/files/filter";
import combineUrl from "@appserver/common/utils/combineUrl";
import { AppServerConfig } from "@appserver/common/constants";
import { homepage } from "../../../../package.json";

import throttle from "lodash/throttle";

const getSettingsNext = async () => {
  try {
    const res = await request({
      method: "get",
      url: "/settings.json",
    });
    return res;
  } catch (error) {
    console.log(error);
  }
};

const getDocServiceUrl = () => {
  return request({ method: "get", url: `/files/docservice` });
};

const getUser = (headers) => {
  return request({
    method: "get",
    url: "/people/@self.json",
    headers: headers,
  });
};

const getFileInfo = (fileId, headers) => {
  return request({
    method: "get",
    url: `/files/file/${fileId}`,
    headers: headers,
  });
};

const checkFillFormDraft = (fileId, headers) => {
  // TODO: need headers?
  return request({
    method: "post",
    url: `files/masterform/${fileId}/checkfillformdraft`,
    data: { fileId },
    headers: headers,
  });
};

const openEdit = (fileId, version, doc, view, headers) => {
  const params = []; // doc ? `?doc=${doc}` : "";

  if (view) {
    params.push(`view=${view}`);
  }

  if (version) {
    params.push(`version=${version}`);
  }

  if (doc) {
    params.push(`doc=${doc}`);
  }

  const paramsString = params.length > 0 ? `?${params.join("&")}` : "";

  const options = {
    method: "get",
    url: `/files/file/${fileId}/openedit${paramsString}`,
    headers,
  };

  return request(options);
};

const markAsFavorite = (ids, headers) => {
  const data = { fileIds: ids };
  const options = {
    method: "post",
    url: "/files/favorites",
    data,
    headers: headers,
  };

  return request(options);
};

const removeFromFavorite = (ids) => {
  const data = { fileIds: ids };
  const options = {
    method: "delete",
    url: "/files/favorites",
    data,
  };

  return request(options);
};

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
}; // TODO: нужно подключить i18n

const onSDKRequestSharingSettings = () => {
  //setIsVisible(true); TODO: перенести шару
  console.log("onSDKRequestSharingSettings");
};

const onSDKRequestRename = (event) => {
  // const title = event.data;
  // updateFile(fileInfo.id, title);
  console.log("onSDKRequestRename");
};

const onSDKRequestRestore = async (event) => {
  // const restoreVersion = event.data.version;
  // try {
  //   const updateVersions = await restoreDocumentsVersion(
  //     fileId,
  //     restoreVersion,
  //     doc
  //   );
  //   const historyLength = updateVersions.length;
  //   docEditor.refreshHistory({
  //     currentVersion: getCurrentDocumentVersion(
  //       updateVersions,
  //       historyLength
  //     ),
  //     history: getDocumentHistory(updateVersions, historyLength),
  //   });
  // } catch (e) {
  //   docEditor.refreshHistory({
  //     error: `${e}`, //TODO: maybe need to display something else.
  //   });
  // }
  console.log("onSDKRequestRestore");
};

const onSDKRequestInsertImage = (event) => {
  // setTypeInsertImageAction(event.data);
  // setFilesType(insertImageAction);
  // setIsFileDialogVisible(true);
  console.log("onSDKRequestInsertImage");
};

const onSDKRequestMailMergeRecipients = () => {
  // setFilesType(mailMergeAction);
  // setIsFileDialogVisible(true);
  console.log("onSDKRequestMailMergeRecipients");
};

const onSDKRequestCompareFile = () => {
  // setFilesType(compareFilesAction);
  // setIsFileDialogVisible(true);
  console.log("onSDKRequestCompareFile");
};

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

const onMakeActionLink = (event) => {
  // var ACTION_DATA = event.data;

  // const link = generateLink(ACTION_DATA);

  // const urlFormation = !actionLink ? url : url.split("&anchor=")[0];

  // const linkFormation = `${urlFormation}&anchor=${link}`;

  // docEditor.setActionLink(linkFormation);
  console.log("onMakeActionLink");
};

const onSDKRequestHistory = async () => {
  // try {
  //   const fileHistory = await getEditHistory(fileId, doc);
  //   const historyLength = fileHistory.length;

  //   docEditor.refreshHistory({
  //     currentVersion: getCurrentDocumentVersion(fileHistory, historyLength),
  //     history: getDocumentHistory(fileHistory, historyLength),
  //   });
  // } catch (e) {
  //   docEditor.refreshHistory({
  //     error: `${e}`, //TODO: maybe need to display something else.
  //   });
  // }
  console.log("onSDKRequestHistory");
};

const onSDKRequestHistoryClose = () => {
  document.location.reload();
};

const onSDKRequestHistoryData = async (event) => {
  console.log("onSDKRequestHistoryData");
  // const version = event.data;

  // try {
  //   const versionDifference = await getEditDiff(fileId, version, doc);
  //   const changesUrl = versionDifference.changesUrl;
  //   const previous = versionDifference.previous;
  //   const token = versionDifference.token;

  //   docEditor.setHistoryData({
  //     ...(changesUrl && { changesUrl }),
  //     key: versionDifference.key,
  //     fileType: versionDifference.fileType,
  //     ...(previous && {
  //       previous: {
  //         fileType: previous.fileType,
  //         key: previous.key,
  //         url: previous.url,
  //       },
  //     }),
  //     ...(token && { token }),
  //     url: versionDifference.url,
  //     version,
  //   });
  // } catch (e) {
  //   docEditor.setHistoryData({
  //     error: `${e}`, //TODO: maybe need to display something else.
  //     version,
  //   });
  // }
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

const text = "text";
const presentation = "presentation";
let documentIsReady = false; // move to state?
let docSaved = null; // move to state?
let docTitle = null;

export default function Home({
  fileInfo,
  docApiUrl,
  config,
  personal,
  successAuth,
  isSharingAccess,
  docEditor,
  user,
  url,
  doc,
  fileId, //
}) {
  const [titleSelectorFolder, setTitleSelectorFolder] = useState("");
  const [urlSelectorFolder, setUrlSelectorFolder] = useState("");
  const [extension, setExtension] = useState();
  const [isFolderDialogVisible, setIsFolderDialogVisible] = useState(false);

  const throttledChangeTitle = throttle(() => changeTitle(), 500);

  const router = useRouter();
  useEffect(() => {}, []);

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

      setFavicon(config.documentType); // TODO: need to fix
      const docTitle = config.document.title;
      setDocumentTitle(docTitle);

      if (isMobile) {
        config.type = "mobile";
        // не уверен что нужно, т.к. сразу в конфиге прилетает
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
          text: "SSR TEST", ///i18n.t("FileLocation"), TODO: подкключить i18
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
        onRequestSharingSettings = onSDKRequestSharingSettings;
      }

      if (fileInfo && fileInfo.canEdit) {
        onRequestRename = onSDKRequestRename;
      }

      if (successAuth) {
        onRequestSaveAs = onSDKRequestSaveAs; //+++
        onRequestInsertImage = onSDKRequestInsertImage;
        onRequestMailMergeRecipients = onSDKRequestMailMergeRecipients;
        onRequestCompareFile = onSDKRequestCompareFile;
      }

      if (!!config.document.permissions.changeHistory) {
        onRequestRestore = onSDKRequestRestore;
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
          onRequestSharingSettings, //---
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
      //toastr.error(error.message, null, 0, true);
    }
  };

  return (
    <div style={{ height: "100vh" }}>
      <Head title="Loading...">
        <title>Loading...</title>
      </Head>
      <div id="editor"></div>
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
    </div>
  );
}

export async function getServerSideProps({ params, req, query, res }) {
  const { headers, cookies, url } = req;
  const { version, desktop: isDesktop } = query;
  let error,
    docApiUrl,
    settings,
    fileInfo,
    user,
    config,
    personal,
    successAuth,
    actionLink,
    isSharingAccess,
    doc;

  const decodedId = query.fileId || query.fileid || null;
  const fileId =
    typeof decodedId === "string" ? encodeURIComponent(decodedId) : decodedId;

  console.log(query, url);

  try {
    //const doc = url.indexOf("doc=") !== -1 ? url.split("doc=")[1] : null;??
    doc = query?.doc || null;
    const view = url.indexOf("action=view") !== -1;

    if (!fileId) {
      error = "error file id";
      return { props: { error } };
    } // TODO:

    docApiUrl = await getDocServiceUrl();
    settings = await getSettingsNext();
    user = await getUser(headers); // remove from node?

    try {
      personal = settings?.personal;
      successAuth = !!user;
    } catch (e) {
      successAuth = false;
    }

    if (!successAuth && !doc) {
      res.writeHead(301, {
        // TODO:
        Location: personal
          ? `/sign-in?fallback=${url}`
          : `/login?fallback=${url}`,
        state: { url: url },
      });
      res.end();
      return;
    }

    if (successAuth) {
      try {
        fileInfo = await getFileInfo(fileId, headers);
        // if (url.indexOf("#message/") > -1) {
        //   if (canConvert(fileInfo.fileExst)) {
        //     const url = await convertDocumentUrl();
        //     history.pushState({}, null, url);
        //   }
        // } TODO:
      } catch (err) {
        error = typeof err === "string" ? err : err.message;
      }
    }

    const fileVersion = version || null;

    config = await openEdit(fileId, fileVersion, doc, view, headers);

    // const NextRequestMetaSymbol = Reflect.ownKeys(req).find(
    //   (key) => key.toString() === "Symbol(NextRequestMeta)"
    // );
    // const targetUrl = req[NextRequestMetaSymbol].__NEXT_INIT_URL;
    // console.log(targetUrl);

    if (
      !view &&
      fileInfo &&
      fileInfo.canWebRestrictedEditing &&
      fileInfo.canFillForms &&
      !fileInfo.canEdit
    ) {
      try {
        const formUrl = await checkFillFormDraft(fileId);
        // TODO:
        // history.pushState({}, null, formUrl);
        //   url = window.location.href;
      } catch (err) {
        error = err;
      }
    }

    actionLink = config?.editorConfig?.actionLink;
    console.log(config);

    if (isDesktop) {
      // initDesktop(config); TODO:
    }

    isSharingAccess = fileInfo && fileInfo.canShare;

    if (view) {
      config.editorConfig.mode = "view";
    }
  } catch (err) {
    error = typeof err === "string" ? err : err.message;
  }

  //console.log(req);
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
    },
  };
}
