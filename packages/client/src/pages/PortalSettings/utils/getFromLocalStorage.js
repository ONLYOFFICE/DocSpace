export const getFromLocalStorage = (key) => {
  return JSON.parse(localStorage.getItem(key));
};
