import express from "express";
import path from "path";
import template from "./server/template";
import render from "./server/render";

const app = express();

app.use(
  "/products/files/doceditor/static/",
  express.static(path.resolve(__dirname, "../clientBuild/static"))
);
app.use(express.static(path.resolve(__dirname, "../clientBuild")));

app.disable("x-powered-by");

const port = process.env.PORT || 5013;

app.get("/products/files/doceditor", async (req, res) => {
  const { props, content, styleTags, scriptTags } = await render(req);

  const response = template(
    "Server Rendered Page",
    props,
    content,
    styleTags,
    scriptTags
  );
  res.setHeader("Cache-Control", "assets, max-age=604800");
  res.send(response);
});
//app.get("/", (req, res) => console.log("root", req));
const isDevelopment = process.env.NODE_ENV === "development";

app.listen(port, () => console.log(`server listen port: ${port}`));
