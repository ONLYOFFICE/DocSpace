import axios from "axios";
import config from "PACKAGE_FILE";
import { combineUrl } from "@docspace/common/utils";

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

    return await axios.post(
      `${combineUrl(
        window.DocSpaceConfig?.proxy?.url,
        config.homepage
      )}/logoUploader.ashx`,
      data
    );
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
