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

  router.post("/update-file", (req, res) => {
    files.updateFile(req.body);
    res.end();
  });

  router.post("/delete-file", (req, res) => {
    files.deleteFile(req.body);
    res.end();
  });

  return router;
};
