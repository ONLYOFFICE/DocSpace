(function (DocSpace) {
  const defaultConfig = {
    src: "http://192.168.1.60:8092/products/files/",
    width: "1024px",
    height: "800px",
    name: "frameDocSpace",
    type: "desktop",
    frameId: "ds-frame",
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

    iframe.src = config.src + filter;
    iframe.width = config.width;
    iframe.height = config.height;
    iframe.name = config.name;

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
    const config = getConfigFromParams();

    const onMessage = (msg) => {
      if (msg) {
        if (msg.type === "onMessage") {
          sendCommand(msg);
        } else if (msg.type === "onCallback") {
          postMessage(window.parent, msg);
        }
      }
    };

    const target = document.getElementById(config.frameId);
    let iframe;
    let msgDispatcher;

    if (target) {
      iframe = createIframe(config);

      target.parentNode && target.parentNode.replaceChild(iframe, target);
      msgDispatcher = new MessageDispatcher(onMessage, this);

      localStorage.setItem("dsFrameConfig", JSON.stringify(config));

      postMessage(window.parent, {
        frameId: config.frameId,
        message: "Frame inserted!",
      });
    }

    const destroyFrame = () => {
      var target = document.createElement("div");

      target.setAttribute("id", config.frameId);
      target.innerHTML = config.destroyText;

      if (iframe) {
        msgDispatcher && msgDispatcher.unbindEvents();
        iframe.parentNode && iframe.parentNode.replaceChild(target, iframe);

        localStorage.removeItem("dsFrameConfig");
      }
    };

    const sendCommand = (command) => {
      if (iframe && iframe.contentWindow)
        postMessage(iframe.contentWindow, command);
    };

    /*     const getItems = () => {
      sendCommand("getItems");
    }; */

    return {
      destroyFrame,
    };
  };

  MessageDispatcher = function (fn, scope) {
    const callFunction = fn;
    const msgScope = scope;
    const eventFn = (message) => {
      onMessage(message);
    };

    const bindEvents = () => {
      window.addEventListener("message", eventFn, false);
    };

    const unbindEvents = () => {
      window.removeEventListener("message", eventFn, false);
    };

    const onMessage = (msg) => {
      if (msg && window.JSON && msgScope.frameOrigin == msg.origin) {
        try {
          const msg = window.JSON.parse(msg.data);
          if (callFunction) {
            callFunction.call(msgScope, msg);
          }
        } catch (e) {}
      }
    };

    bindEvents.call(this);

    return {
      unbindEvents: unbindEvents,
    };
  };

  const postMessage = (wnd, message) => {
    if (wnd && wnd.postMessage && window.JSON) {
      wnd.postMessage(message, "*"); //window.JSON.stringify(message)
    }
  };

  window.DocSpace = DocSpace();
})();
