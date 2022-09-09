import { EDITOR_PROTOCOL } from "./filesConstants";
import { combineUrl } from "@docspace/common/utils";
import { homepage } from "PACKAGE_FILE";

export const presentInArray = (array, search, caseInsensitive = false) => {
  let pattern = caseInsensitive ? search.toLowerCase() : search;
  const result = array.findIndex((item) => item === pattern);
  return result === -1 ? false : true;
};

export const getAccessIcon = (access) => {
  switch (access) {
    case 1:
      return "/static/images/access.edit.react.svg";
    case 2:
      return "/static/images/eye.react.svg";
    case 3:
      return "/static/images/access.none.react.svg";
    case 4:
      return "images/catalog.question.react.svg";
    case 5:
      return "/static/images/access.review.react.svg";
    case 6:
      return "/static/images/access.comment.react.svg";
    case 7:
      return "/static/images/access.form.react.svg";
    case 8:
      return "/static/images/custom.filter.react.svg";
    default:
      return;
  }
};

export const getTitleWithoutExst = (item, fromTemplate) => {
  return item.fileExst && !fromTemplate
    ? item.title.split(".").slice(0, -1).join(".")
    : item.title;
};

export const checkProtocol = (fileId, withRedirect) =>
  new Promise((resolve, reject) => {
    const onBlur = () => {
      clearTimeout(timeout);
      window.removeEventListener("blur", onBlur);
      resolve();
    };

    const timeout = setTimeout(() => {
      reject();
      window.removeEventListener("blur", onBlur);
      withRedirect &&
        window.open(
          combineUrl("", homepage, `private?fileId=${fileId}`),
          "_blank"
        );
    }, 1000);

    window.addEventListener("blur", onBlur);

    window.open(
      combineUrl(
        `${EDITOR_PROTOCOL}:${window.location.origin}`,
        homepage,
        `doceditor?fileId=${fileId}`
      ),
      "_self"
    );
  });
