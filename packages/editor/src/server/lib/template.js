import { getFavicon, getScripts } from "./helpers";
import pkg from "../../../package.json";

export default function template(
  initialEditorState = {},
  appComponent = "",
  styleTags,
  initialI18nStoreASC,
  initialLanguage,
  assets
) {
  const { title } = pkg;
  const { error } = initialEditorState;
  const editorUrl = initialEditorState?.config?.editorUrl;
  const faviconHref = getFavicon(initialEditorState?.logoUrls);

  let clientScripts =
    assets && assets.hasOwnProperty("client.js")
      ? `<script defer="defer" src='${assets["client.js"]}'></script>`
      : "";

  const editorApiScript =
    error || !editorUrl
      ? ""
      : `<script type='text/javascript' id='onlyoffice-api-script' src="${editorUrl}" async></script>`;

  if (!IS_DEVELOPMENT) {
    const productionBundleKeys = getScripts(assets);
    productionBundleKeys.map((key) => {
      clientScripts =
        clientScripts + `<script defer="defer" src='${assets[key]}'></script>`;
    });
  }

  const initialEditorStateStringify = JSON.stringify(initialEditorState);

  const initialEditorStateString = initialEditorStateStringify.includes(
    "</script>"
  )
    ? initialEditorStateStringify.replaceAll("</script>", "<\\/script>")
    : initialEditorStateStringify;

  const scripts = `   
    <script id="__ASC_INITIAL_EDITOR_STATE__">
      window.__ASC_INITIAL_EDITOR_STATE__ = ${initialEditorStateString}
    </script>
    <script id="__ASC_INITIAL_EDITOR_I18N__">
      window.initialI18nStoreASC = ${JSON.stringify(initialI18nStoreASC)}
      window.initialLanguage = '${initialLanguage}'
    </script>
    ${clientScripts}
    <script>
      const tempElm = document.getElementById("loader");
      tempElm.style.backgroundColor =
        localStorage.theme === "Dark" ? "#333333" : "#f4f4f4";
      console.log("It's Editor INIT");
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
    ${editorApiScript} 

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
          <link rel="apple-touch-icon" href=${faviconHref} />
          <link rel="android-touch-icon" href=${faviconHref} />
          ${styleTags}   
          <style type="text/css">
            .loadmask {
              left: 0;
              top: 0;
              position: absolute;
              height: 100%;
              width: 100%;
              overflow: hidden;
              border: none;
              background-color: #f4f4f4;
              z-index: 1001;
            }
            .loader-page {
              width: 100%;
              height: 170px;
              bottom: 42%;
              position: absolute;
              text-align: center;
              line-height: 10px;
              margin-bottom: 20px;
            }
            .loader-page-romb {
              width: 40px;
              display: inline-block;
            }
            .romb {
              width: 40px;
              height: 40px;
              position: absolute;
              background: red;
              border-radius: 6px;
              -webkit-transform: rotate(135deg) skew(20deg, 20deg);
              -moz-transform: rotate(135deg) skew(20deg, 20deg);
              -ms-transform: rotate(135deg) skew(20deg, 20deg);
              -o-transform: rotate(135deg) skew(20deg, 20deg);
              -webkit-animation: movedown 3s infinite ease;
              -moz-animation: movedown 3s infinite ease;
              -ms-animation: movedown 3s infinite ease;
              -o-animation: movedown 3s infinite ease;
              animation: movedown 3s infinite ease;
            }
            #blue {
              z-index: 3;
              background: #55bce6;
              -webkit-animation-name: blue;
              -moz-animation-name: blue;
              -ms-animation-name: blue;
              -o-animation-name: blue;
              animation-name: blue;
            }
            #red {
              z-index: 1;
              background: #de7a59;
              -webkit-animation-name: red;
              -moz-animation-name: red;
              -ms-animation-name: red;
              -o-animation-name: red;
              animation-name: red;
            }
            #green {
              z-index: 2;
              background: #a1cb5c;
              -webkit-animation-name: green;
              -moz-animation-name: green;
              -ms-animation-name: green;
              -o-animation-name: green;
              animation-name: green;
            }
            @-webkit-keyframes red {
              0% {
                top: 120px;
                background: #de7a59;
              }
              10% {
                top: 120px;
                background: #f2cbbf;
              }
              14% {
                background: #f4f4f4;
                top: 120px;
              }
              15% {
                background: #f4f4f4;
                top: 0;
              }
              20% {
                background: #e6e4e4;
              }
              30% {
                background: #d2d2d2;
              }
              40% {
                top: 120px;
              }
              100% {
                top: 120px;
                background: #de7a59;
              }
            }
            @keyframesred {
              0% {
                top: 120px;
                background: #de7a59;
              }
              10% {
                top: 120px;
                background: #f2cbbf;
              }
              14% {
                background: #f4f4f4;
                top: 120px;
              }
              15% {
                background: #f4f4f4;
                top: 0;
              }
              20% {
                background: #e6e4e4;
              }
              30% {
                background: #d2d2d2;
              }
              40% {
                top: 120px;
              }
              100% {
                top: 120px;
                background: #de7a59;
              }
            }
            @-webkit-keyframes green {
              0% {
                top: 110px;
                background: #a1cb5c;
                opacity: 1;
              }
              10% {
                top: 110px;
                background: #cbe0ac;
                opacity: 1;
              }
              14% {
                background: #f4f4f4;
                top: 110px;
                opacity: 1;
              }
              15% {
                background: #f4f4f4;
                top: 0;
                opacity: 1;
              }
              20% {
                background: #f4f4f4;
                top: 0;
                opacity: 0;
              }
              25% {
                background: #efefef;
                top: 0;
                opacity: 1;
              }
              30% {
                background: #e6e4e4;
              }
              70% {
                top: 110px;
              }
              100% {
                top: 110px;
                background: #a1cb5c;
              }
            }
            @keyframes green {
              0% {
                top: 110px;
                background: #a1cb5c;
                opacity: 1;
              }
              10% {
                top: 110px;
                background: #cbe0ac;
                opacity: 1;
              }
              14% {
                background: #f4f4f4;
                top: 110px;
                opacity: 1;
              }
              15% {
                background: #f4f4f4;
                top: 0;
                opacity: 1;
              }
              20% {
                background: #f4f4f4;
                top: 0;
                opacity: 0;
              }
              25% {
                background: #efefef;
                top: 0;
                opacity: 1;
              }
              30% {
                background: #e6e4e4;
              }
              70% {
                top: 110px;
              }
              100% {
                top: 110px;
                background: #a1cb5c;
              }
            }
            @-webkit-keyframes blue {
              0% {
                top: 100px;
                background: #55bce6;
                opacity: 1;
              }
              10% {
                top: 100px;
                background: #bfe8f8;
                opacity: 1;
              }
              14% {
                background: #f4f4f4;
                top: 100px;
                opacity: 1;
              }
              15% {
                background: #f4f4f4;
                top: 0;
                opacity: 1;
              }
              20% {
                background: #f4f4f4;
                top: 0;
                opacity: 0;
              }
              25% {
                background: #f4f4f4;
                top: 0;
                opacity: 0;
              }
              45% {
                background: #efefef;
                top: 0;
                opacity: 0.2;
              }
              100% {
                top: 100px;
                background: #55bce6;
              }
            }
            @keyframes blue {
              0% {
                top: 100px;
                background: #55bce6;
                opacity: 1;
              }
              10% {
                top: 100px;
                background: #bfe8f8;
                opacity: 1;
              }
              14% {
                background: #f4f4f4;
                top: 100px;
                opacity: 1;
              }
              15% {
                background: #f4f4f4;
                top: 0;
                opacity: 1;
              }
              20% {
                background: #f4f4f4;
                top: 0;
                opacity: 0;
              }
              25% {
                background: #fff;
                top: 0;
                opacity: 0;
              }
              45% {
                background: #efefef;
                top: 0;
                opacity: 0.2;
              }
              100% {
                top: 100px;
                background: #55bce6;
              }
            }
          </style>
        </head>
        <body>
          <div id="loader" class="loadmask">
            <div class="loader-page">
              <div class="loader-page-romb">
                <div class="romb" id="blue"></div>
                <div class="romb" id="green"></div>
                <div class="romb" id="red"></div>
              </div>
            </div>
          </div>
          <div id="root">${appComponent}</div>
          <noscript> You need to enable JavaScript to run this app. </noscript>
          ${scripts}
        </body>
      </html>
  `;

  return page;
}
