module.exports = {
  setupFiles: ["<rootDir>/test/setup-tests.js"],
  setupFilesAfterEnv: ["<rootDir>/scripts/setup-test-framework.js"],
  transform: {
    "^.+\\.js$": "<rootDir>/test/transform-babel-jest.js",
  },
  /* It solves css/less/scss import issues.
    You might have similar issues with different file extensions (e.g. md).
    Just search for "<file type> jest loader"
  */
  moduleNameMapper: {
    "\\.(jpg|jpeg|png|gif|eot|otf|webp|svg|ttf|woff|woff2|mp4|webm|wav|mp3|m4a|aac|oga)$":
      "<rootDir>/test/transform-file.js",
    "\\.css$": "<rootDir>/test/style-mock.js",
  },
  coverageReporters: ["json", "lcov", "text", "clover", "cobertura"],
};
