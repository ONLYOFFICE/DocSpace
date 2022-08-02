(function (DocSpace) {
  const defaultConfig = {
    src: `${window.location.protocol}//${window.location.hostname}:8092`,
    rootPath: "/products/files/",
    width: "100%",
    height: "100%",
    name: "frameDocSpace",
    type: "desktop",
    frameId: "ds-frame",
    fileId: null,
    editorType: "embedded",
    showHeader: false,
    showArticle: false,
    showTitle: true,
    showFilter: false,
    destroyText: "Frame container",
    viewAs: "row",
    filter: {
      folder: "@my",
      count: 25,
      page: 0,
      sortorder: "descending",
      sortby: "DateAndTime",
      search: null,
      filterType: null,
      authorType: null,
      withSubfolders: true,
    },
    keysForReload: [
      "src",
      "rootPath",
      "width",
      "height",
      "name",
      "frameId",
      "fileId",
      "type",
      "editorType",
    ],
  };

  oneOfInObject = (array, object) => {
    return Object.keys(object).some((k) => array.includes(k));
  };

  getConfigFromParams = () => {
    const src = document.currentScript.src;

    if (!src || !src.length) return null;

    const searchUrl = src.split("?")[1];
    let object = {};

    if (searchUrl && searchUrl.length) {
      object = Object.fromEntries(new URLSearchParams(searchUrl));

      object.filter = {};

      for (prop in object) {
        if (prop in defaultConfig.filter) {
          object.filter[prop] = object[prop];
          delete object[prop];
        }
      }
    }

    return { ...defaultConfig, ...object };
  };

  createIframe = (config) => {
    const iframe = document.createElement("iframe");

    const filter = config.filter
      ? `filter?${new URLSearchParams(config.filter).toString()}`
      : ``;

    const editor = `doceditor/?fileId=${config.fileId}&type=${config.editorType}`;

    const pathname = config.fileId ? editor : filter;

    iframe.src = config.src + config.rootPath + pathname;
    iframe.width = config.width;
    iframe.height = config.height;
    iframe.name = config.name;
    iframe.id = config.frameId;

    iframe.align = "top";
    iframe.frameBorder = 0;
    iframe.allowFullscreen = true;
    iframe.setAttribute("allowfullscreen", "");
    iframe.setAttribute("onmousewheel", "");
    iframe.setAttribute("allow", "autoplay");

    if (config.type == "mobile") {
      iframe.style.position = "fixed";
      iframe.style.overflow = "hidden";
      document.body.style.overscrollBehaviorY = "contain";
    }

    return iframe;
  };

  DocSpace = () => {
    let config = getConfigFromParams();
    let iframe;
    let isConnected = false;

    let callbacks = [];
    let tasks = [];

    const sendMessage = (message) => {
      let mes = {
        frameId: config.frameId,
        type: "",
        data: message,
      };

      if (iframe)
        iframe.contentWindow.postMessage(JSON.stringify(mes), config.src);
    };

    const onMessage = (e) => {
      if (typeof e.data == "string") {
        let frameData = {};

        try {
          frameData = JSON.parse(e.data);
        } catch (err) {
          frameData = {};
        }

        switch (frameData.type) {
          case "onMethodReturn": {
            if (callbacks.length > 0) {
              const callback = callbacks.shift();
              callback && callback(frameData.methodReturnData);
            }

            if (tasks.length > 0) {
              sendMessage(tasks.shift());
            }
            break;
          }
          case "onCommandCallback": {
            if (callbacks.length > 0) {
              const callback = callbacks.shift();
              callback && callback();
            }

            if (tasks.length > 0) {
              sendMessage(tasks.shift());
            }
            break;
          }
          default:
            break;
        }
      }
    };

    const executeMethod = (name, params, callback) => {
      if (!isConnected) {
        console.log("Message bus is not connected with frame");
        return;
      }

      callbacks.push(callback);

      const message = {
        type: "method",
        methodName: name,
        data: params,
      };

      if (callbacks.length !== 1) {
        tasks.push(message);
        return;
      }

      sendMessage(message);
    };

    const initFrame = (frameConfig) => {
      config = { ...config, ...frameConfig };

      const target = document.getElementById(config.frameId);

      if (target) {
        iframe = createIframe(config);

        target.parentNode && target.parentNode.replaceChild(iframe, target);

        window.addEventListener("message", onMessage, false);
        isConnected = true;
      }
    };

    const destroyFrame = () => {
      var target = document.createElement("div");

      target.setAttribute("id", config.frameId);
      target.innerHTML = config.destroyText;

      if (iframe) {
        window.removeEventListener("message", onMessage, false);
        isConnected = false;

        iframe.parentNode && iframe.parentNode.replaceChild(target, iframe);
      }
    };

    const getFolderInfo = () => {
      return new Promise((resolve) =>
        executeMethod("getFolderInfo", null, (data) => resolve(data))
      );
    };

    const getSelection = () => {
      return new Promise((resolve) =>
        executeMethod("getSelection", null, (data) => resolve(data))
      );
    };

    const getFiles = () => {
      return new Promise((resolve) =>
        executeMethod("getFiles", null, (data) => resolve(data))
      );
    };

    const getFolders = () => {
      return new Promise((resolve) =>
        executeMethod("getFolders", null, (data) => resolve(data))
      );
    };

    const getItems = () => {
      return new Promise((resolve) =>
        executeMethod("getItems", null, (data) => resolve(data))
      );
    };

    const getUserInfo = () => {
      return new Promise((resolve) =>
        executeMethod("getUserInfo", null, (data) => resolve(data))
      );
    };

    const getConfig = () => config;

    const setConfig = (newConfig = {}, reload = false) => {
      if (oneOfInObject(config.keysForReload, newConfig)) reload = true;

      config = { ...config, ...newConfig };

      return new Promise((resolve) => {
        if (reload) {
          initFrame(config);
          resolve(config);
        } else {
          executeMethod("setConfig", config, (data) => resolve(data));
        }
      });
    };

    const openCrateFileModal = (format) => {
      executeMethod("openCrateItemModal", format);
    };

    const openCrateFolderModal = () => {
      executeMethod("openCrateItemModal");
    };

    const openCrateRoomModal = () => {
      executeMethod("openCrateRoomModal");
    };

    const createFile = (folderId, title, templateId, formId) => {
      return new Promise((resolve) =>
        executeMethod(
          "createFile",
          { folderId, title, templateId, formId },
          (data) => resolve(data)
        )
      );
    };

    const createFolder = (parentFolderId, title) => {
      return new Promise((resolve) =>
        executeMethod("createFolder", { parentFolderId, title }, (data) =>
          resolve(data)
        )
      );
    };

    const createRoom = (title, type) => {
      return new Promise((resolve) =>
        executeMethod("createRoom", { title, type }, (data) => resolve(data))
      );
    };

    const setItemsView = (type) => {
      executeMethod("setItemsView", type);
    };

    initFrame(config);

    return {
      initFrame,
      destroyFrame,

      getConfig,
      getFolderInfo,
      getUserInfo,
      getSelection,
      getFiles,
      getFolders,
      getItems,

      setConfig,
      setItemsView,

      openCrateFileModal,
      openCrateFolderModal,
      openCrateRoomModal,

      createFile,
      createFolder,
      createRoom,
    };
  };

  window.DocSpace = DocSpace();
})();
