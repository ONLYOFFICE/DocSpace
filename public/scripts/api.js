(function () {
  const defaultConfig = {
    src: new URL(document.currentScript.src).origin,
    rootPath: "/rooms/personal/",
    width: "100%",
    height: "100%",
    name: "frameDocSpace",
    type: "desktop", // TODO: ["desktop", "mobile"]
    frameId: "ds-frame",
    mode: "manager", //TODO: ["manager", "editor", "viewer", "file selector", "folder selector", "user picker"]
    fileId: null,
    editorType: "embedded", //TODO: ["desktop", "embedded"]
    showHeader: false,
    showTitle: true,
    showMenu: false,
    showFilter: false,
    showAction: false,
    destroyText: "Frame container",
    viewAs: "row", //TODO: ["row", "table", "tile"]
    filter: {
      folder: "@my",
      count: 25,
      page: 0,
      sortorder: "descending", //TODO: ["descending", "ascending"]
      sortby: "DateAndTime", //TODO: ["DateAndTime", "AZ", "Type", "Size", "DateAndTimeCreation", "Author"]
      search: "",
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
      "mode",
    ],
    events: {
      onSelectCallback: (e) => console.log("onCloseCallback", e),
      onCloseCallback: null,
    },
  };

  const getConfigFromParams = () => {
    const src = document.currentScript.src;

    if (!src || !src.length) return null;

    const searchUrl = src.split("?")[1];
    let object = {};

    if (searchUrl && searchUrl.length) {
      object = JSON.parse(
        `{"${searchUrl.replace(/&/g, '","').replace(/=/g, '":"')}"}`,
        (k, v) => (v === "true" ? true : v === "false" ? false : v)
      );

      object.filter = defaultConfig.filter;

      for (prop in object) {
        if (prop in defaultConfig.filter) {
          object.filter[prop] = object[prop];
          delete object[prop];
        }
      }
    }

    return { ...defaultConfig, ...object };
  };

  class DocSpace {
    #iframe;
    #isConnected = false;
    #callbacks = [];
    #tasks = [];

    constructor(config) {
      this.config = config;
    }

    #oneOfExistInObject = (array, object) => {
      return Object.keys(object).some((k) => array.includes(k));
    };

    #createIframe = (config) => {
      const iframe = document.createElement("iframe");

      let path = "";

      switch (config.mode) {
        case "manager": {
          if (config.filter) {
            const filterString = new URLSearchParams(config.filter).toString();
            path = `${config.rootPath}filter?${filterString}`;
          }
          break;
        }

        case "editor": {
          path = `/doceditor/?fileId=${config.fileId}&type=${config.editorType}`;
          break;
        }

        case "viewer": {
          path = `/doceditor/?fileId=${config.fileId}&type=${config.editorType}&action=view`;
          break;
        }

        default:
          path = config.rootPath;
      }

      iframe.src = config.src + path;
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

    #sendMessage = (message) => {
      let mes = {
        frameId: this.config.frameId,
        type: "",
        data: message,
      };

      if (this.#iframe)
        this.#iframe.contentWindow.postMessage(
          JSON.stringify(mes),
          this.config.src
        );
    };

    #onMessage = (e) => {
      if (typeof e.data == "string") {
        let frameData = {};

        try {
          frameData = JSON.parse(e.data);
        } catch (err) {
          frameData = {};
        }

        switch (frameData.type) {
          case "onMethodReturn": {
            if (this.#callbacks.length > 0) {
              const callback = this.#callbacks.shift();
              callback && callback(frameData.methodReturnData);
            }

            if (this.#tasks.length > 0) {
              this.#sendMessage(this.#tasks.shift());
            }
            break;
          }
          case "onCallCommand": {
            this[frameData.commandName].call(this, frameData.commandData);
            break;
          }
          default:
            break;
        }
      }
    };
    #executeMethod = (methodName, params, callback) => {
      if (!this.#isConnected) {
        console.log("Message bus is not connected with frame");
        return;
      }

      this.#callbacks.push(callback);

      const message = {
        type: "method",
        methodName,
        data: params,
      };

      if (this.#callbacks.length !== 1) {
        this.#tasks.push(message);
        return;
      }

      this.#sendMessage(message);
    };

    initFrame(frameConfig) {
      this.config = { ...this.config, ...frameConfig };

      const target = document.getElementById(this.config.frameId);

      if (target) {
        this.#iframe = this.#createIframe(this.config);

        target.parentNode &&
          target.parentNode.replaceChild(this.#iframe, target);

        window.addEventListener("message", this.#onMessage, false);
        this.#isConnected = true;
      }
    }

    destroyFrame() {
      const target = document.createElement("div");

      target.setAttribute("id", this.config.frameId);
      target.innerHTML = this.config.destroyText;

      if (this.#iframe) {
        window.removeEventListener("message", this.#onMessage, false);
        this.#isConnected = false;

        this.#iframe.parentNode &&
          this.#iframe.parentNode.replaceChild(target, this.#iframe);
      }
    }

    #getMethodPromise = (methodName, params = null, withReload = false) => {
      return new Promise((resolve) => {
        if (withReload) {
          this.initFrame(this.config);
          resolve(this.config);
        } else {
          this.#executeMethod(methodName, params, (data) => resolve(data));
        }
      });
    };

    getFolderInfo() {
      return this.#getMethodPromise("getFolderInfo");
    }

    getSelection() {
      return this.#getMethodPromise("getSelection");
    }

    getFiles() {
      return this.#getMethodPromise("getFiles");
    }

    getFolders() {
      return this.#getMethodPromise("getFolders");
    }

    getList() {
      return this.#getMethodPromise("getList");
    }

    getUserInfo() {
      return this.#getMethodPromise("getUserInfo");
    }

    getConfig() {
      return this.config;
    }

    setConfig(newConfig = {}, reload = false) {
      if (this.#oneOfExistInObject(this.config.keysForReload, newConfig))
        reload = true;

      this.config = { ...this.config, ...newConfig };

      return this.#getMethodPromise("setConfig", this.config, reload);
    }

    openModal(type, options) {
      return this.#getMethodPromise("openModal", { type, options });
    }

    createFile(folderId, title, templateId, formId) {
      return this.#getMethodPromise("createFile", {
        folderId,
        title,
        templateId,
        formId,
      });
    }

    createFolder(parentFolderId, title) {
      return this.#getMethodPromise("createFolder", {
        parentFolderId,
        title,
      });
    }

    createRoom(title, type) {
      return this.#getMethodPromise("createRoom", {
        title,
        type,
      });
    }

    setListView(type) {
      return this.#getMethodPromise("setItemsView", type);
    }
  }

  const config = getConfigFromParams();

  window.DocSpace = new DocSpace(config);

  window.DocSpace.initFrame();
})();
