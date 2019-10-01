const decomposeConfirmLink = (url, querySearch) => {
  const decodedString = decodeURIComponent(querySearch);
  const queryString = decodedString.slice(1).split('&');
  const arrayOfQueryString = queryString.map(queryParam => queryParam.split('='));
  const queryParams = Object.fromEntries(arrayOfQueryString);
  const posSeparator = url.lastIndexOf('/');
  const type = url.slice(posSeparator + 1);
  const data = Object.assign({ type }, queryParams);
  return data;
}

export default decomposeConfirmLink;