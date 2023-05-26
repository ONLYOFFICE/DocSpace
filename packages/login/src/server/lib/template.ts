import { getScripts } from "./helpers";
import pkg from "../../../package.json";
import { getLogoFromPath } from "@docspace/common/utils";

import firstFont from "PUBLIC_DIR/fonts/RjgO7rYTmqiVp7vzi-Q5URJtnKITppOI_IvcXXDNrsc.woff2";
import secondFont from "PUBLIC_DIR/fonts/MTP_ySUJH_bn48VBG8sNSugdm0LZdjqr5-oayXSOefg.woff2";
import thirdFont from "PUBLIC_DIR/fonts/k3k702ZOKiLJc3WVjuplzOgdm0LZdjqr5-oayXSOefg.woff2";
import fourthFont from "PUBLIC_DIR/fonts/cJZKeOuBrn4kERxqtaUH3VtXRa8TVwTICgirnJhmVJw.woff2";
import fifthFont from "PUBLIC_DIR/fonts/MTP_ySUJH_bn48VBG8sNSpX5f-9o1vgP2EXwfjgl7AY.woff2";
import sixthFont from "PUBLIC_DIR/fonts/k3k702ZOKiLJc3WVjuplzJX5f-9o1vgP2EXwfjgl7AY.woff2";

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
    ? `${t("Common:Authorization")} – ${organizationName}`
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

  const scripts = `   
    <script id="__ASC_INITIAL_LOGIN_STATE__">
      window.__ASC_INITIAL_LOGIN_STATE__ = ${JSON.stringify(initLoginState)}
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
        <link rel="preload" as="font" crossorigin type="font/woff2" href="${firstFont}" />
        <link rel="preload" as="font" crossorigin type="font/woff2" href="${secondFont}" />
        <link rel="preload" as="font" crossorigin type="font/woff2" href="${thirdFont}" />
        <link rel="preload" as="font" crossorigin type="font/woff2" href="${fourthFont}" />
        <link rel="preload" as="font" crossorigin type="font/woff2" href="${fifthFont}" />
        <link rel="preload" as="font" crossorigin type="font/woff2" href="${sixthFont}" />

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

        <style>
        @media (prefers-color-scheme: light) {
          body {
            background-color: #fff;
          }

          #login-header{
            background-color: #f8f9f9;
          }

          #login-page > div > svg > path:last-child {
            fill: #333;
          }

          #login-page > div > p {
            color: #333;
          }

          #login-form {
            background-color: #fff;
            box-shadow: 0px 5px 20px rgba(4, 15, 27, 0.07);
          }

          #login, #password > div, #password > div > input {
            background: #fff;
            border-color: #D0D5DA;
          }

          #login-checkbox > svg > rect {
            fill: #fff;
            stroke: #D0D5DA;
          }

          #login-checkbox > div > span {
            color: #333;
          }

          #code-page > div > svg > path:last-child {
            fill: #333;
          }

          #workspace-title {
            color: #333;
          }

          #code-page > div > div > input {
            background: #fff;
            border-color: #d0d5da;
          }

          #code-page > div > div > input:disabled {
            background: #F8F9F9;
            border-color: #ECEEF1;
          }
        }

        @media (prefers-color-scheme: dark) {
          body {
            background-color: #333;
          }

          #login-header{
            background-color: #282828;
          }

          #login-page > div > svg > path:last-child {
            fill: #fff;
          }

          #login-page > div > p {
            color: #fff;
          }

          #login-form {
            background-color: #333;
            box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.16);
          }

          #login, #password > div, #password > div > input {
            background: #282828;
            border-color: #474747;
          }

          #login-checkbox > svg > rect {
            fill: #282828;
            stroke: #474747;
          }

          #login-checkbox > div > span {
            color: #fff;
          }

          #code-page > div > svg > path:last-child {
            fill: #fff;
          }

          #workspace-title {
            color: #fff;
          }

          #code-page > div > div > input {
            background: #282828;
            border-color: #474747;
          }

          #code-page > div > div > input:disabled {
            background: #474747;
            border-color: #474747;
          }
        }        
        </style>

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
