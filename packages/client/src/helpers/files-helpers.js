import CatalogQuestionReactSvgUrl from "PUBLIC_DIR/images/catalog.question.react.svg?url";
import AccessEditReactSvgUrl from "PUBLIC_DIR/images/access.edit.react.svg?url";
import EyeReactSvgUrl from "PUBLIC_DIR/images/eye.react.svg?url";
import AccessNoneReactSvgUrl from "PUBLIC_DIR/images/access.none.react.svg?url";
import AccessReviewReactSvgUrl from "PUBLIC_DIR/images/access.review.react.svg?url";
import AccessCommentReactSvgUrl from "PUBLIC_DIR/images/access.comment.react.svg?url";
import AccessFormReactSvgUrl from "PUBLIC_DIR/images/access.form.react.svg?url";
import CustomFilterReactSvgUrl from "PUBLIC_DIR/images/custom.filter.react.svg?url";
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
      return AccessEditReactSvgUrl;
    case 2:
      return EyeReactSvgUrl;
    case 3:
      return AccessNoneReactSvgUrl;
    case 4:
      return CatalogQuestionReactSvgUrl;
    case 5:
      return AccessReviewReactSvgUrl;
    case 6:
      return AccessCommentReactSvgUrl;
    case 7:
      return AccessFormReactSvgUrl;
    case 8:
      return CustomFilterReactSvgUrl;
    default:
      return;
  }
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
