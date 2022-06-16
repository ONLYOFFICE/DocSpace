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
    showFilter: false,
    destroyText: "Frame container",
    filter: {
      folderId: "@my",
      withSubfolders: true,
      sortBy: "DateAndTime",
      sortOrder: "descending",
    },
  };

  getConfigFromParams = () => {
    const src = document.currentScript.src;

    if (!src || !src.length) return null;

    const searchUrl = src.split("?")[1];
    let object = {};

    if (searchUrl && searchUrl.length) {
      const decodedString = decodeURIComponent(searchUrl)
        .replace(/"/g, '\\"')
        .replace(/&/g, '","')
        .replace(/=/g, '":"')
        .replace(/\\/g, "\\\\");
      object = JSON.parse(`{"${decodedString}"}`);
    }

    return { ...defaultConfig, ...object };
  };

  createIframe = (config) => {
    const iframe = document.createElement("iframe");

    iframe.src = config.src;
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
        if (msg.type === "onExternalPluginMessage") {
          sendCommand(msg);
        } else if (msg.type === "onExternalPluginMessageCallback") {
          postMessage(window.parent, msg);
        } else if (msg.frameId == config.frameId) {
          const events = config.events || {};
          const handler = events[msg.event];
          let res;

          if (handler && typeof handler == "function") {
            res = handler.call(this, { target: this, data: msg.data });
          }
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

      postMessage(window.parent, {
        source: config.frameId,
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
      }
    };

    const sendCommand = (command) => {
      if (iframe && iframe.contentWindow)
        postMessage(iframe.contentWindow, command);
    };

    return {
      destroyFrame,
    };
  };

  MessageDispatcher = function (fn, scope) {
    const eventFn = (message) => {
      onMessage(message);
    };

    const bindEvents = () => {
      window.addEventListener("message", eventFn, false);
    };

    const unbindEvents = () => {
      window.removeEventListener("message", eventFn, false);
    };

    const onMessage = (message) => {
      if (message && window.JSON && scope.frameOrigin == message.origin) {
        try {
          const message = window.JSON.parse(message.data);
          if (fn) {
            fn.call(scope, message);
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
