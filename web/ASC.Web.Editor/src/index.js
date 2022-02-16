import express from "express";
import path from "path";
import template from "./src/server/template";
import render from "./src/server/render";

const app = express();

app.use(
  "/products/files/doceditor/static/scripts/",
  express.static(path.resolve(__dirname, "static/scripts"))
);
app.use("/media", express.static(path.resolve(__dirname, "media")));

app.disable("x-powered-by");

app.listen(process.env.PORT || 5013);

app.get("/products/files/doceditor/", async (req, res) => {
  const { props, content } = await render(req);
  const response = template("Server Rendered Page", props, content);
  res.setHeader("Cache-Control", "assets, max-age=604800");
  res.send(response);
});
