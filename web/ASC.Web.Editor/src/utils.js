import store from "studio/store";
import { isIOS, deviceType } from "react-device-detect";

const { auth } = store;

//import textIcon from "./icons/text.ico";
//import presentationIcon from "./icons/presentation.ico";
//import spreadsheetIcon from "./icons/spreadsheet.ico";

export const changeTitle = (docSaved, docTitle) => {
  docSaved ? setDocumentTitle(docTitle) : setDocumentTitle(`*${docTitle}`);
};

export const setFavicon = (fileType) => {
  const favicon = document.getElementById("favicon");
  if (!favicon) return;

  switch (fileType) {
    case "docx":
      //favicon.href = textIcon;
      break;
    case "pptx":
      //favicon.href = presentationIcon;
      break;
    case "xlsx":
      //favicon.href = spreadsheetIcon;
      break;

    default:
      break;
  }
};

export const isIPad = () => {
  return isIOS && deviceType === "tablet";
};

export const setDocumentTitle = (subTitle = null) => {
  const { isAuthenticated, settingsStore, product: currentModule } = auth;
  const { organizationName } = settingsStore;

  let title;
  if (subTitle) {
    if (isAuthenticated && currentModule) {
      title = subTitle + " - " + currentModule.title;
    } else {
      title = subTitle + " - " + organizationName;
    }
  } else if (currentModule && organizationName) {
    title = currentModule.title + " - " + organizationName;
  } else {
    title = organizationName;
  }

  document.title = title;
};
