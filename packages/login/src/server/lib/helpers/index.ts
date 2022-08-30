import path from "path";
import fs from "fs";
import {
  getSettings,
  getBuildVersion,
  getAuthProviders,
  getCapabilities,
} from "@docspace/common/api/settings";
import { LANGUAGE } from "@docspace/common/constants";
import parser from "accept-language-parser";
import { getPortalCultures } from "@docspace/common/api/settings";

export const getAssets = (): assetsType => {
  const manifest = fs.readFileSync(
    path.join(__dirname, "client/manifest.json"),
    "utf-8"
  );

  const assets = JSON.parse(manifest);

  return assets;
};

export const getScripts = (assets: assetsType): string[] | void => {
  if (!assets || typeof assets !== "object") return;
  const regTest = /static\/js\/.*/;
  const keys = [];

  for (let key in assets) {
    if (assets.hasOwnProperty(key) && regTest.test(key)) {
      keys.push(key);
    }
  }

  return keys;
};

export const loadPath = (language: string, nameSpace: string): string => {
  let resourcePath = path.resolve(
    path.join(__dirname, "client", `locales/${language}/${nameSpace}.json`)
  );
  if (nameSpace === "Common")
    resourcePath = path.resolve(
      path.join(
        __dirname,
        `../../../public/locales/${language}/${nameSpace}.json`
      )
    );

  return resourcePath;
};

export const getInitialState = async (
  query: MatchType
): Promise<IInitialState> => {
  let portalSettings: IPortalSettings,
    buildInfo: IBuildInfo,
    providers: ProvidersType,
    capabilities: ICapabilities;

  [portalSettings, buildInfo, providers, capabilities] = await Promise.all([
    getSettings(),
    getBuildVersion(),
    getAuthProviders(),
    getCapabilities(),
  ]);

  const initialState: IInitialState = {
    portalSettings,
    buildInfo,
    providers,
    capabilities,
    match: query,
  };

  return initialState;
};

export const getCurrentLanguage = async (cookies, headers) => {
  let currentLanguage = "en";

  if (cookies && cookies[LANGUAGE]) {
    currentLanguage = cookies[LANGUAGE];
  } else {
    const availableLanguages: string[] = await getPortalCultures();
    const parsedAcceptLanguages: object[] = parser.parse(
      headers["accept-language"]
    );

    const detectedLanguage: IAcceptLanguage | any = parsedAcceptLanguages.find(
      (acceptLang: IAcceptLanguage) =>
        typeof acceptLang === "object" &&
        acceptLang?.code &&
        availableLanguages.includes(acceptLang.code)
    );

    if (typeof detectedLanguage === "object")
      currentLanguage = detectedLanguage.code;
  }

  return currentLanguage;
};
