module.exports = {
  isValidLicense: (license) => {
    const valid = new RegExp("\\b(mit|apache\\b.*2|bsd|isc|unlicense)\\b", "i");
    return valid.test(license);
  },
};
