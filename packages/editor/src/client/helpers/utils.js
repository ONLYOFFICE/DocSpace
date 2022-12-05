import pkg from "../../../package.json";

import { combineUrl } from "@docspace/common/utils";

import { EDITOR_PROTOCOL } from "@docspace/client/src/helpers/filesConstants";

export const canConvert = (extension, filesSettings) => {
  const array = filesSettings?.extsMustConvert || [];
  const result = array.findIndex((item) => item === extension);
  return result === -1 ? false : true;
};

export const initI18n = (initialI18nStoreASC) => {
  if (!initialI18nStoreASC || window.i18n) return;

  const { homepage } = pkg;

  window.i18n = {};
  window.i18n.inLoad = [];
  window.i18n.loaded = {};

  for (let lng in initialI18nStoreASC) {
    for (let ns in initialI18nStoreASC[lng]) {
      if (ns === "Common") {
        window.i18n.loaded[`/static/locales/${lng}/${ns}.json`] = {
          namespaces: ns,
          data: initialI18nStoreASC[lng][ns],
        };
      } else {
        window.i18n.loaded[`${homepage}/locales/${lng}/${ns}.json`] = {
          namespaces: ns,
          data: initialI18nStoreASC[lng][ns],
        };
      }
    }
  }
};

export const checkProtocol = (fileId) =>
  new Promise((resolve, reject) => {
    const onBlur = () => {
      clearTimeout(timeout);
      window.removeEventListener("blur", onBlur);
      resolve();
    };

    const timeout = setTimeout(() => {
      reject();
      window.removeEventListener("blur", onBlur);
    }, 1000);

    window.addEventListener("blur", onBlur);

    window.open(
      combineUrl(
        `${EDITOR_PROTOCOL}:${window.location.origin}`,
        "/",
        `doceditor?fileId=${fileId}`
      ),
      "_self"
    );
  });
