const decomposeLink = (url, querySearch) => {
  const queryString = querySearch.slice(1).split('&');
  const arrayOfQueryString = queryString.map(queryParam => queryParam.split('='));
  const queryParams = Object.fromEntries(arrayOfQueryString);
  const posSeparator = url.lastIndexOf('/');
  const type = url.slice(posSeparator + 1);
  const data = Object.assign({ type }, queryParams);
  return data;
}

export default decomposeLink;