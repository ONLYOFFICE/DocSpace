module.exports = (files) => {
  const router = require("express").Router();

  router.post("/start-edit", (req, res) => {
    files.startEdit(req.body);
    res.end();
  });

  router.post("/stop-edit", (req, res) => {
    files.stopEdit(req.body);
    res.end();
  });

  router.post("/create-file", (req, res) => {
    files.createFile(req.body);
    res.end();
  });

  router.post("/create-folder", (req, res) => {
    files.createFolder(req.body);
    res.end();
  });

  router.post("/update-file", (req, res) => {
    files.updateFile(req.body);
    res.end();
  });

  router.post("/update-folder", (req, res) => {
    files.updateFolder(req.body);
    res.end();
  });

  router.post("/delete-file", (req, res) => {
    files.deleteFile(req.body);
    res.end();
  });

  router.post("/delete-folder", (req, res) => {
    files.deleteFolder(req.body);
    res.end();
  });

  router.post("/markasnew-file", (req, res) => {
    files.markAsNewFiles(req.body);
    res.end();
  });

  router.post("/markasnew-folder", (req, res) => {
    files.markAsNewFolders(req.body);
    res.end();
  });

  router.post("/change-quota-used-value", (req, res) => {
    files.changeQuotaUsedValue(req.body);
    res.end();
  });

  router.post("/change-quota-feature-value", (req, res) => {
    files.changeQuotaFeatureValue(req.body);
    res.end();
  });

  return router;
};