export const getCrashReport = (userId, version, language, error) => {
  const currentTime = new Date();
  const reportTime = currentTime.toTimeString();
  const lsObject = JSON.stringify(window.localStorage) || "";

  const report = {
    url: window.origin,
    id: userId,
    version: version,
    platform: navigator?.platform,
    userAgent: navigator?.userAgent,
    language: language || "en",
    errorMessage: error?.message,
    errorStack: error?.stack,
    localStorage: lsObject,
    reportTime: reportTime,
  };

  return report;
};
