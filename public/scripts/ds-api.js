((window, document) => {
  const defaultConfig = {
    src: "http://192.168.1.60:8092/products/files/",
    width: "1024px",
    height: "800px",
    name: "frameDocSpace",
    type: "desktop",
    frameId: "ds-frame",
    filter: {
      folderId: "@my",
      withSubfolders: true,
      sortBy: "DateAndTime",
      sortOrder: "descending",
    },
  };

  const getConfigFromParams = () => {
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

  const initDocSpace = (config = defaultConfig) => {
    const target = document.getElementById(config.frameId);
    let iframe;

    if (target) {
      iframe = createIframe(config);

      target.parentNode && target.parentNode.replaceChild(iframe, target);
    }
  };

  const createIframe = (config) => {
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

    console.log("Created frame: ", config);

    return iframe;
  };

  const config = getConfigFromParams(); // TODO: Add level recognition for params

  initDocSpace(config);
})(window, document);
