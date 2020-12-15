import { setDocumentTitle } from "../../../helpers/utils";

export const changeTitleAsync = (docSaved, docTitle) => {
  docSaved ? setDocumentTitle(docTitle) : setDocumentTitle(`*${docTitle}`);

  const resp = docSaved;
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve(resp);
    }, 500);
  });
};

export const setFavicon = (pathToIcon) => {
  const favicon = document.getElementById("favicon");
  favicon.href = pathToIcon;
};
