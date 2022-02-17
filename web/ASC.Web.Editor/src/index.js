import express from "express";
import path from "path";
import template from "./server/template";
import render from "./server/render";

const app = express();

app.use(
  "/products/files/doceditor/static/scripts/",
  express.static(path.resolve(__dirname, "static/scripts"))
);
app.use("/media", express.static(path.resolve(__dirname, "media")));
app.use(express.static("dist"));
app.disable("x-powered-by");
const port = process.env.PORT || 5013;
app.listen(port, () => console.log(`server listen port: ${port}`));

app.get("/products/files/doceditor/", async (req, res) => {
  const { props, content, styleTags } = await render(req);
  const response = template("Server Rendered Page", props, content, styleTags);
  res.setHeader("Cache-Control", "assets, max-age=604800");
  res.send(response);
});
