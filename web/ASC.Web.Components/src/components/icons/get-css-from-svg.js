function addNameSpace(data) {
  if (data.indexOf("http://www.w3.org/2000/svg") < 0) {
    data = data.replace(/<svg/g, `<svg xmlns='http://www.w3.org/2000/svg'`);
  }

  return data;
}
function encodeSVG(data) {
  // eslint-disable-next-line no-useless-escape
  var symbols = /[\r\n%#()<>?\[\\\]^`{|}]/g;

  data = data.replace(/"/g, "'");
  data = data.replace(/>\s{1,}</g, "><");
  data = data.replace(/\s{2,}/g, " ");

  return data.replace(symbols, encodeURIComponent);
}

export function getCssFromSvg(svg) {
  var namespaced = addNameSpace(svg);
  var escaped = encodeSVG(namespaced);

  return escaped;
}
