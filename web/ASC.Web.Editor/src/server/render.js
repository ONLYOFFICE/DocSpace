import { getFavicon } from "../helpers/utils";
import pkg from "../../package.json";

export default function template(
  initialEditorState = {},
  appComponent = "",
  styleTags,
  initialI18nStoreASC,
  initialLanguage,
  assets
) {
  const { title } = pkg;
  const { docApiUrl, error } = initialEditorState;

  const faviconHref = getFavicon(initialEditorState?.config?.documentType);
  const docApi = error
    ? ""
    : `<script type='text/javascript' id='scripDocServiceAddress' src="${docApiUrl}" async></script>`;
  const scripts = `   
    <script id="__ASC_INITIAL_EDITOR_STATE__">
      window.__ASC_INITIAL_EDITOR_STATE__ = ${JSON.stringify(
        initialEditorState
      )}
    </script>
    <script id="__ASC_INITIAL_EDITOR_I18N__">
      window.initialI18nStoreASC = ${JSON.stringify(initialI18nStoreASC)}
      window.initialLanguage = '${initialLanguage}'
    </script>

    <script defer="defer" src='${assets["client.js"]}'></script>
    ${docApi}
    
`;
  // <script defer="defer" src='${assets["runtime.js"]}'></script>
  // <script defer="defer" src='${assets["vendor.js"]}'></script>
  const page = `
    <!DOCTYPE html>
      <html lang="en">
        <head>
          <meta charset="utf-8">
          <title> ${title} </title>
          <meta charset="utf-8" />
          <meta
            name="viewport"
            content="width=device-width, initial-scale=1, shrink-to-fit=no, user-scalable=no, viewport-fit=cover"
          />
          <meta name="theme-color" content="#000000" />
          <link id="favicon" rel="shortcut icon" href=${faviconHref} type="image/x-icon"/>
          <link rel="manifest" href="/manifest.json" />
          <meta name="mobile-web-app-capable" content="yes" />
          <meta name="apple-mobile-web-app-capable" content="yes" />
          <link rel="apple-touch-icon" href="/appIcon.png" />
          ${styleTags}   
          
        </head>
        <body>
          <div id="root">${appComponent}</div>
          <noscript> You need to enable JavaScript to run this app. </noscript>
          
            ${scripts}

          </body>
      </html>
  `;

  return page;
}
