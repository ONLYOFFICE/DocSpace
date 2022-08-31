import { getScripts } from "./helpers";
import pkg from "../../../package.json";

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
    ? `${t("Authorization")} – ${organizationName}`
    : title;

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
        <link rel="preload" as="font"  type="font" href="/static/fonts/RjgO7rYTmqiVp7vzi-Q5URJtnKITppOI_IvcXXDNrsc.woff2" />
        <link rel="preload" as="font"  type="font" href="/static/fonts/MTP_ySUJH_bn48VBG8sNSugdm0LZdjqr5-oayXSOefg.woff2" />
        <link rel="preload" as="font"  type="font" href="/static/fonts/k3k702ZOKiLJc3WVjuplzOgdm0LZdjqr5-oayXSOefg.woff2" />
        <link rel="preload" as="font"  type="font" href="/static/fonts/cJZKeOuBrn4kERxqtaUH3VtXRa8TVwTICgirnJhmVJw.woff2" />
        <link rel="preload" as="font"  type="font" href="/static/fonts/MTP_ySUJH_bn48VBG8sNSpX5f-9o1vgP2EXwfjgl7AY.woff2" />
        <link rel="preload" as="font"  type="font" href="/static/fonts/k3k702ZOKiLJc3WVjuplzJX5f-9o1vgP2EXwfjgl7AY.woff2" />

        <link id="favicon" rel="shortcut icon" href="/favicon.ico" />
        <link rel="manifest" href="/manifest.json" />
        <!-- Tell the browser it's a PWA -->
        <!-- <meta name="mobile-web-app-capable" content="yes" /> -->
        <!-- Tell iOS it's a PWA -->
        <!-- <meta name="apple-mobile-web-app-capable" content="yes" /> -->
        <link rel="apple-touch-icon" href="/appIcon-180.png" />
        ${styleTags}   
      </head>
      <body>
        <noscript> You need to enable JavaScript to run this app. </noscript>
        <div id="root">${appComponent}</div>
        ${scripts}
      </body>
    </html>
  `;

  return page;
};

export default template;
