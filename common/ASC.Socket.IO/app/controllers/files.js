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

  return router;
};
