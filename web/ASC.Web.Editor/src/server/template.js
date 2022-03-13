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
          <link rel="stylesheet" href="style.css">
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
