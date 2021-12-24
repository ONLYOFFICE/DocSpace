module.exports = (files) => {
  const router = require("express").Router(),
    bodyParser = require("body-parser"),
    authService = require("../middleware/authService.js")();

  router.use(bodyParser.json());
  router.use(bodyParser.urlencoded({ extended: false }));
  router.use(require("cookie-parser")());
  router.use((req, res, next) => {
    if (!authService(req)) {
      res.sendStatus(403);
      return;
    }

    next();
  });

  router.use("/files", require(`./files.js`)(files));

  return router;
};
