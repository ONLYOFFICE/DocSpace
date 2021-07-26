const getUTCString = function () {
  const utc = new Date();
  return utc.toISOString();
};

module.exports = getUTCString;
