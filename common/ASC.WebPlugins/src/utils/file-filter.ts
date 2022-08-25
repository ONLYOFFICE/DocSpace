const fileFilter = (req, file, cb) => {
  if (file.mimetype === "text/javascript") return cb(null, true);

  return cb(null, false);
};

export default fileFilter;
