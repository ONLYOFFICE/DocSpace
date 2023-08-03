import axios from "axios";
import isEqual from "lodash/isEqual";

export const generateLogo = (
  width,
  height,
  text,
  fontSize = 18,
  fontColor = "#000",
  alignCenter
) => {
  const canvas = document.createElement("canvas");
  canvas.width = width;
  canvas.height = height;

  const ctx = canvas.getContext("2d");
  const x = alignCenter ? width / 2 : 0;

  ctx.fillStyle = "transparent";
  ctx.clearRect(0, 0, width, height);
  ctx.fillStyle = fontColor;
  ctx.textAlign = alignCenter ? "center" : "start";
  ctx.textBaseline = "middle";
  ctx.font = `${fontSize}px Arial`;
  ctx.fillText(text, x, height - fontSize / 2);

  return canvas.toDataURL();
};

export const getLogoOptions = (index, text) => {
  switch (index) {
    case 0:
      return { fontSize: 18, text: text, width: 211, height: 24 };
    case 1:
      return { fontSize: 32, text: text, width: 384, height: 42 };
    case 2:
      return {
        fontSize: 26,
        text: text.trim().charAt(0),
        width: 30,
        height: 30,
        alignCenter: true,
      };
    case 3:
      return { fontSize: 22, text: text, width: 154, height: 27 };
    case 4:
      return { fontSize: 22, text: text, width: 154, height: 27 };
    case 5:
      return {
        fontSize: 24,
        text: text.trim().charAt(0),
        width: 28,
        height: 28,
        alignCenter: true,
      };
    case 6:
      return { fontSize: 18, text: text, width: 211, height: 24 };
    default:
      return { fontSize: 18, text: text, width: 211, height: 24 };
  }
};

export const uploadLogo = async (file) => {
  try {
    const { width, height } = await getUploadedFileDimensions(file);
    let data = new FormData();
    data.append("file", file);
    data.append("width", width);
    data.append("height", height);

    return await axios.post("/logoUploader.ashx", data);
  } catch (error) {
    console.error(error);
  }
};

const getUploadedFileDimensions = (file) =>
  new Promise((resolve, reject) => {
    try {
      let img = new Image();

      img.onload = () => {
        const width = img.naturalWidth,
          height = img.naturalHeight;

        window.URL.revokeObjectURL(img.src);

        return resolve({ width, height });
      };

      img.src = window.URL.createObjectURL(file);
    } catch (exception) {
      return reject(exception);
    }
  });

export const getNewLogoArr = (
  logoUrlsWhiteLabel,
  defaultWhiteLabelLogoUrls
) => {
  let logosArr = [];

  for (let i = 0; i < logoUrlsWhiteLabel.length; i++) {
    const currentLogo = logoUrlsWhiteLabel[i];
    const defaultLogo = defaultWhiteLabelLogoUrls[i];

    if (!isEqual(currentLogo, defaultLogo)) {
      let value = {};

      if (!isEqual(currentLogo.path.light, defaultLogo.path.light))
        value.light = currentLogo.path.light;
      if (!isEqual(currentLogo.path.dark, defaultLogo.path.dark))
        value.dark = currentLogo.path.dark;

      logosArr.push({
        key: String(i + 1),
        value: value,
      });
    }
  }
  return logosArr;
};

export const getLogosAsText = (logoUrlsWhiteLabel, logoTextWhiteLabel) => {
  let newLogos = logoUrlsWhiteLabel;
  for (let i = 0; i < logoUrlsWhiteLabel.length; i++) {
    const options = getLogoOptions(i, logoTextWhiteLabel);
    const isDocsEditorName = logoUrlsWhiteLabel[i].name === "DocsEditor";

    const logoLight = generateLogo(
      options.width,
      options.height,
      options.text,
      options.fontSize,
      isDocsEditorName ? "#fff" : "#000",
      options.alignCenter
    );
    const logoDark = generateLogo(
      options.width,
      options.height,
      options.text,
      options.fontSize,
      "#fff",
      options.alignCenter
    );
    newLogos[i].path.light = logoLight;
    newLogos[i].path.dark = logoDark;
  }
  return newLogos;
};
