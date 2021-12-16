const { setHeadlessWhen, setWindowSize } = require('@codeceptjs/configure');

// turn on headless mode when running with HEADLESS=true environment variable
// export HEADLESS=true && npx codeceptjs run
setHeadlessWhen(process.env.HEADLESS);

const sizes = {
  mobile: { width: 375, height: 667 },
  smallTablet: { width: 600, height: 667 },
  tablet: { width: 1023, height: 667 },
  desktop: { width: 1920, height: 1080 },
};

const deviceType = process.env.DEVICE_TYPE || 'desktop';

const isTranslation = !!process.env.TRANSLATION;

const device = sizes[deviceType];

setWindowSize(device.width, device.height);

const browser = process.env.profile || 'chromium';

const isModel = !!process.env.MODEL;

const screenshotOutput = isTranslation
  ? isModel
    ? `./tests/screenshots/translation/${browser}/${deviceType}`
    : `./tests/output/translation/${browser}/${deviceType}`
  : isModel
  ? `./tests/screenshots/${browser}/${deviceType}`
  : `./tests/output/${browser}/${deviceType}`;

const baseFolder = isTranslation
  ? `./tests/screenshots/translation/${browser}/${deviceType}`
  : `./tests/screenshots/${browser}/${deviceType}`;

const tests = isTranslation ? './tests/translation_tests.js' : './tests/*_tests.js';

exports.config = {
  tests: tests,
  output: screenshotOutput,
  helpers: {
    Playwright: {
      url: 'http://localhost:8092',
      // show browser window
      show: false,
      browser: browser,
      // restart browser between tests
      restart: true,
      waitForNavigation: 'networkidle0',
      // don't save screenshot on failure
      disableScreenshots: false,
    },
    ResembleHelper: {
      require: 'codeceptjs-resemblehelper',
      screenshotFolder: './tests/output/',
      baseFolder: baseFolder,
      diffFolder: './tests/output/diff/',
    },
    PlaywrightHelper: {
      require: './tests/helpers/playwright.helper.js',
    },
  },
  include: {
    I: './steps_file.js',
  },
  bootstrap: null,
  mocha: {
    reporterOptions: {
      mochawesome: {
        stdout: '-',
        options: {
          reportDir: `./tests/reports/${browser}/${deviceType}`,
          reportFilename: 'report',
        },
      },
      'mocha-junit-reporter': {
        stdout: '-',
        options: {
          mochaFile: `./tests/reports/${browser}/${deviceType}/report.xml`,
          attachments: false, //add screenshot for a failed test
        },
      },
    },
  },
  name: 'ASC.Web.Files',
  plugins: {
    pauseOnFail: {},
    retryFailedStep: {
      enabled: true,
    },
    tryTo: {
      enabled: true,
    },
    screenshotOnFail: {
      enabled: true,
    },
  },
};
