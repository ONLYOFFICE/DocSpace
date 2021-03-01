module.exports = {
  setupFiles: ["./test/setup-tests.js"],
  setupFilesAfterEnv: ["./scripts/setup-test-framework.js"],
  transform: {
    "^.+\\.js$": "./test/transform-babel-jest.js",
  },
  /* It solves css/less/scss import issues.
    You might have similar issues with different file extensions (e.g. md).
    Just search for "<file type> jest loader"
  */
  moduleNameMapper: {
    "\\.(jpg|jpeg|png|gif|eot|otf|webp|svg|ttf|woff|woff2|mp4|webm|wav|mp3|m4a|aac|oga)$":
      "./test/transform-file.js",
  },
  coverageReporters: ["json", "lcov", "text", "clover", "cobertura"],
};
