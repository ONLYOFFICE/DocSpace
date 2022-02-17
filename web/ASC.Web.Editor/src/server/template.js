export default function template(
  title,
  initialState = {},
  content = "",
  styleTags
) {
  const scripts = `   
    <script>
      window.__STATE__ = ${JSON.stringify(initialState)}
    </script>
    <script src="/products/files/doceditor/static/scripts/doceditor.client.js"></script>
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
