export const getCrashReport = (userId, version, language, error) => {
  const reportTime = new Date();

  const report = {
    url: window.origin,
    id: userId,
    version: version,
    platform: navigator?.platform,
    userAgent: navigator?.userAgent,
    language: language,
    errorMessage: error?.message,
    errorStack: error?.stack,
    localStorage: window.localStorage,
    reportTime: reportTime,
  };
  console.log("report", report);
};
