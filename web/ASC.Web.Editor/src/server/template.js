export default function template(
  title,
  initialState = {},
  content = "",
  styleTags,
  scriptTags,
  initialI18nStore,
  initialLanguage
) {
  const { docApiUrl } = initialState.props;

  const scripts = `   
    <script id="__ASC_INITIAL_STATE__">
      window.__ASC_INITIAL_STATE__ = ${JSON.stringify(initialState)}
      window.initialI18nStore = JSON.parse('${JSON.stringify(
        initialI18nStore
      )}')
      window.initialLanguage = '${initialLanguage}'
    </script>
    <script type='text/javascript' id='scripDocServiceAddress' src="${docApiUrl}" async></script>
    ${scriptTags}
`;

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
          <link id="favicon" rel="shortcut icon" href="/favicon.ico" />
          <link rel="manifest" href="/manifest.json" />
          <meta name="mobile-web-app-capable" content="yes" />
          <meta name="apple-mobile-web-app-capable" content="yes" />
          <link rel="apple-touch-icon" href="/appIcon.png" />
          ${styleTags}
        </head>
        <body>
          <div id="root">${content}</div>
            ${scripts}
          </body>
      </html>
  `;

  return page;
}
