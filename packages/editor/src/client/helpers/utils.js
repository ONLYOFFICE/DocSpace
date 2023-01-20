import pkg from "../../../package.json";

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
