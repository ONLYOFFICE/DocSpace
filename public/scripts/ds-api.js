(function (DocSpace) {
  const defaultConfig = {
    src: "http://192.168.1.60:8092/",
    rootPath: "products/files/",
    width: "100%",
    height: "100%",
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

    iframe.src = config.src + config.rootPath + filter;
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

    const initFrame = (frameConfig) => {
      config = { ...config, ...frameConfig };

      const target = document.getElementById(config.frameId);

      if (target) {
        iframe = createIframe(config);

        target.parentNode && target.parentNode.replaceChild(iframe, target);

        localStorage.setItem("dsFrameConfig", JSON.stringify(config));
      }
    };

    const destroyFrame = () => {
      var target = document.createElement("div");

      target.setAttribute("id", config.frameId);
      target.innerHTML = config.destroyText;

      if (iframe) {
        iframe.parentNode && iframe.parentNode.replaceChild(target, iframe);
      }
    };

    const getFolderInfo = () => {
      return "test";
    };

    const getUserInfo = () => {
      return "test";
    };

    const getConfig = () => config;

    initFrame(config);

    return {
      initFrame,
      destroyFrame,
      getConfig,
      getFolderInfo,
      getUserInfo,
    };
  };

  window.DocSpace = DocSpace();
})();
