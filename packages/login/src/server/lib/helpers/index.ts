import path from "path";
import fs from "fs";
import {
  getSettings,
  getBuildVersion,
  getAuthProviders,
  getCapabilities,
} from "@docspace/common/api/settings";

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
  console.log(assets);
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
