export default function template(title, initialState = {}, content = "") {
  let scripts = ""; // Dynamically ship scripts based on render type
  if (content) {
    scripts = `   <script>
                        window.__STATE__ = ${JSON.stringify(initialState)}
                    </script>
                    <script src="/products/files/doceditor/static/scripts/doceditor.client.js"></script>
                    `;
  } else {
    scripts = ` <script src="bundle.js"> </script> `;
  }
  let page = `<!DOCTYPE html>
                  <html lang="en">
                  <head>
                    <meta charset="utf-8">
                    <title> ${title} </title>
                    <link rel="stylesheet" href="style.css">
                  </head>
                  <body>
                    <div id="root">${content}</div>
                    ${scripts}
                  </body>
                  `;

  return page;
}
