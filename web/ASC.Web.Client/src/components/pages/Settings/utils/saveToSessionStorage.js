export const saveToSessionStorage = (key, value) => {
  sessionStorage.setItem(key, JSON.stringify(value));
};
