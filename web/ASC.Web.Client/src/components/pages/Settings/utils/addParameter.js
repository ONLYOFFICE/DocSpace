export const addParameter = (fn, newParameter) => {
  return (originalParameter) => fn(originalParameter, newParameter);
};
