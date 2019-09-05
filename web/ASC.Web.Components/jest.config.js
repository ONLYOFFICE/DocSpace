module.exports = {
  setupFilesAfterEnv: [
    /* Enables jest-enzyme assertions
      https://github.com/FormidableLabs/enzyme-matchers/tree/master/packages/jest-enzyme#readme
    */
    '<rootDir>/node_modules/jest-enzyme/lib/index.js',
    '<rootDir>/config/setupTest.js',
  ],
  transform: {
    '^.+\\.js$': 'babel-jest',
  },
  /* It solves css/less/scss import issues.
    You might have similar issues with different file extensions (e.g. md).
    Just search for "<file type> jest loader"
  */
  moduleNameMapper: {
    '^.+\\.(css|less|scss)$': 'babel-jest',
  },
};
