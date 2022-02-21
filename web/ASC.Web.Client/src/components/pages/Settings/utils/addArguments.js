export const addArguments = (fn, ...newArguments) => {
  return (originalArgument) => fn(originalArgument, ...newArguments);
};
