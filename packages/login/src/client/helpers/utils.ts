import pkg from "../../../package.json";

export const initI18n = (initialI18nStoreASC: IInitialI18nStoreASC): void => {
  if (!initialI18nStoreASC || window.i18n) return;

  const { homepage } = pkg;

  const i18n = {
    inLoad: [],
    loaded: {},
  };
  window.i18n = i18n;

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
