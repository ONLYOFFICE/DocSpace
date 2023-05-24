export const getErrorsStats = (userId, version) => {
  const errors = {
    id: userId,
    version: version,
    platform: navigator?.platform,
    userAgent: navigator?.userAgent,
  };
  console.log("errors", errors);
};
