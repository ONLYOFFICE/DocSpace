const fs = require('fs');
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

const tests = isTranslation
  ? './tests/translation_tests.js'
  : ['./tests/action_tests.js', './tests/render_tests.js'];

const reportDir = isTranslation
  ? `../../../TestsResults/people`
  : `./tests/reports/${browser}/${deviceType}`;

const reportFileName = isTranslation ? 'people-translation' : 'report';

const diffFolder = isTranslation ? '../../../TestsResults/people/diff' : './tests/output/diff/';

if (isTranslation) {
  fs.rmdir(diffFolder, { recursive: true }, (err) => {
    if (err) throw err;
  });

  fs.mkdir(diffFolder, { recursive: true }, (err) => {
    if (err) throw err;
  });
}

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
      diffFolder: diffFolder,
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
          reportDir: reportDir,
          reportFilename: reportFileName,
        },
      },
      'mocha-junit-reporter': {
        stdout: '-',
        options: {
          mochaFile: `${reportDir}/${reportFileName}.xml`,
          attachments: false, //add screenshot for a failed test
        },
      },
    },
  },
  name: 'ASC.Web.People',
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
