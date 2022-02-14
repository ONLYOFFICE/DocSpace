export const addArgument = (fn, newArgument) => {
  return (originalArgument) => fn(originalArgument, newArgument);
};
