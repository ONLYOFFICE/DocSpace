const minifyJson = (content, path) => {
  try {
    var isBuffer = Buffer.isBuffer(content);
    //console.log("is buffer", isBuffer);
    if (isBuffer) {
      content = content.toString().trim();
      //console.log("content string", content);
    }
    const json = JSON.parse(content);
    return JSON.stringify(json);
  } catch (e) {
    console.error("Unable to minimize ", path, e);
    return content;
  }
};

module.exports = minifyJson;
