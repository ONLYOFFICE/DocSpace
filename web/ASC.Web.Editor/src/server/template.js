export default function template(
  title,
  initialState = {},
  content = "",
  styleTags,
  scriptTags
) {
  const scripts = `   
    <script id="__STATE__">
      window.__STATE__ = ${JSON.stringify(initialState)}
    </script>

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
  `;

  return page;
}
