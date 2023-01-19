const path = require("path");

const docSpacePath = "../../../../";

const imagesPattern = /\.(gif|jpe|jpeg|tiff?|png|webp|bmp|svg)$/i;
const filesPattern = /\.(js|jsx|ts|tsx|html|css|scss|saas|json)$/i;
const excludeFilesPattern = /\.(stories|test)$/i;

const excludePath = [
  "C:",
  "GitHub",
  "DocSpace",
  "public",
  "packages",
  "client",
  "common",
  "components",
];

const modules = ["public", "client", "components", "common", "login", "editor"];
// const modules = ["public"];

const publicPath = path.join(__dirname, docSpacePath, "/public");
const clientPath = path.join(__dirname, docSpacePath, "/packages/client");
const componentsPath = path.join(
  __dirname,
  docSpacePath,
  "/packages/components"
);
const commonPath = path.join(__dirname, docSpacePath, "/packages/common");
const loginPath = path.join(__dirname, docSpacePath, "/packages/login");
const editorPath = path.join(__dirname, docSpacePath, "/packages/editor");

const paths = {
  public: publicPath,
  client: clientPath,
  components: componentsPath,
  common: commonPath,
  login: loginPath,
  editor: editorPath,
};

const imageHelperPath = path.join(commonPath, "/utils/image-helpers.js");

module.exports = {
  imagesPattern,
  filesPattern,
  excludeFilesPattern,
  modules,
  paths,
  imageHelperPath,
  excludePath,
};
