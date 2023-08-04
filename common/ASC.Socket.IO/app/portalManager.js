module.exports = (req) => {
  const proto = req.headers['x-forwarded-proto']?.split(',').shift();
  const host = req.headers['x-forwarded-host']?.split(',').shift();

  return `${proto}://${host}`;
};
