import { getScripts } from "./helpers";
import pkg from "../../../package.json";
import { getLogoFromPath } from "@docspace/common/utils";

import fontsCssUrl from "PUBLIC_DIR/css/fonts.css?url";

const { title } = pkg;
const organizationName = "ONLYOFFICE"; //TODO: Replace to API variant

type Template = (
  initLoginState: IInitialState,
  appComponent: string,
  styleTags: string,
  initialI18nStoreASC: object,
  initialLanguage: string,
  assets: assetsType,
  t?: TFuncType
) => string;

const template: Template = (
  initLoginState,
  appComponent,
  styleTags,
  initialI18nStoreASC,
  initialLanguage,
  assets,
  t
) => {
  const documentTitle = t
    ? `${t("Common:Authorization")} - ${organizationName}`
    : title;

  const favicon = getLogoFromPath(
    initLoginState?.logoUrls && initLoginState?.logoUrls[2]?.path?.light
  );

  let clientScripts =
    assets && assets.hasOwnProperty("client.js")
      ? `<script defer="defer" src='${assets["client.js"]}'></script>`
      : "";

  if (!IS_DEVELOPMENT) {
    const productionBundleKeys = getScripts(assets);
    if (productionBundleKeys && typeof assets === "object")
      productionBundleKeys.map((key) => {
        clientScripts =
          clientScripts +
          `<script defer="defer" src='${assets[key]}'></script>`;
      });
  }

  const initialLoginStateStringify = JSON.stringify(initLoginState);

  const initialLoginStateString = initialLoginStateStringify.includes(
    "</script>"
  )
    ? initialLoginStateStringify.replace(/<\/script>/g, "<\\/script>")
    : initialLoginStateStringify;

  const scripts = `   
    <script id="__ASC_INITIAL_LOGIN_STATE__">
      window.__ASC_INITIAL_LOGIN_STATE__ = ${initialLoginStateString}
    </script>
    <script id="__ASC_INITIAL_LOGIN_I18N__">
      window.initialI18nStoreASC = ${JSON.stringify(initialI18nStoreASC)}
      window.initialLanguage = '${initialLanguage}'
    </script>
    ${clientScripts}
    <script>
      console.log("It's Login INIT");
      fetch("/static/scripts/config.json")
      .then((response) => {
        if (!response.ok) {
          throw new Error("HTTP error " + response.status);
        }
        return response.json();
      })
      .then((config) => {
        window.DocSpaceConfig = {
          ...config,
        };

        if (window.navigator.userAgent.includes("ZoomWebKit") || window.navigator.userAgent.includes("ZoomApps")) {
          window.DocSpaceConfig.editor = {
            openOnNewPage: false,
            requestClose: true
          };
        }
      })
      .catch((e) => {
        console.error(e);
      });
    </script>
`;

  const page = `
    <!DOCTYPE html>
    <html lang="en">
      <head>
        <meta charset="utf-8">
        <title> ${documentTitle} </title>
        <meta charset="utf-8" />
        <meta
          name="viewport"
          content="width=device-width, initial-scale=1, shrink-to-fit=no, user-scalable=no, viewport-fit=cover"
        />
        <meta name="theme-color" content="#000000" />
        <link  rel="stylesheet preload" href=${fontsCssUrl}  as="style" type="text/css" crossorigin/>

        <link id="favicon" rel="shortcut icon" href=${favicon} />
        <link rel="manifest" href="/manifest.json" />
        <!-- Tell the browser it's a PWA -->
        <!-- <meta name="mobile-web-app-capable" content="yes" /> -->
        <!-- Tell iOS it's a PWA -->
        <!-- <meta name="apple-mobile-web-app-capable" content="yes" /> -->
        <!-- <link rel="apple-touch-icon" href="/appIcon-180.png" /> -->

        <link rel="apple-touch-icon" href=${favicon} />
        <link rel="android-touch-icon" href=${favicon} />


        ${styleTags}   
      </head>
      <body>
        <noscript> You need to enable JavaScript to run this app. </noscript>
        <div id="root">${appComponent}</div>
        <script src="/static/scripts/browserDetector.js"></script>
        ${scripts}
      </body>
    </html>
  `;

  return page;
};

export default template;
