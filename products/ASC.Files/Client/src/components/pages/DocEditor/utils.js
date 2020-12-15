import { setDocumentTitle } from "../../../helpers/utils";

export const changeTitle = (event, docTitle) => {
  if (event.data) {
    setDocumentTitle(`*${docTitle}`);
  } else {
    setDocumentTitle(docTitle);
  }
};
