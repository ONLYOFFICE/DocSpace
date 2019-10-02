export function getObjectByLocation(location) {
  if (!location.search || !location.search.length) return null;

  const searchUrl = location.search.substring(1);
  const object = JSON.parse(
    '{"' +
    decodeURIComponent(searchUrl)
      .replace(/"/g, '\\"')
      .replace(/&/g, '","')
      .replace(/=/g, '":"') +
    '"}'
  );

  return object;
}

export function decomposeConfirmLink(location) {

  const queryParams = getObjectByLocation(location);
  const url = location.pathname;
  const posSeparator = url.lastIndexOf('/');
  const type = url.slice(posSeparator + 1);
  const data = Object.assign({ type }, queryParams);
  return data;
}
