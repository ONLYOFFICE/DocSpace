export const getFromSessionStorage = (key) => {
  return JSON.parse(sessionStorage.getItem(key));
};
