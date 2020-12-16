import { setDocumentTitle } from "../../../helpers/utils";
import { isIOS, deviceType } from "react-device-detect";

import textIcon from "./icons/text.ico";
import presentationIcon from "./icons/presentation.ico";
import spreadsheetIcon from "./icons/spreadsheet.ico";

export const changeTitle = (docSaved, docTitle) => {
  docSaved ? setDocumentTitle(docTitle) : setDocumentTitle(`*${docTitle}`);
};

export const setFavicon = (fileType) => {
  const favicon = document.getElementById("favicon");
  if (!favicon) return;

  switch (fileType) {
    case "docx":
      favicon.href = textIcon;
      break;
    case "pptx":
      favicon.href = presentationIcon;
      break;
    case "xlsx":
      favicon.href = spreadsheetIcon;
      break;

    default:
      break;
  }
};

export const isIPad = () => {
  return isIOS && deviceType === "tablet";
};
