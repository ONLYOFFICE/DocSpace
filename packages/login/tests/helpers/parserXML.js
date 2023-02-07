let fs = require('fs');
let path = require('path');

const deviceType = ['mobile', 'smallTablet', 'tablet', 'desktop'];
const browser = ['chromium', 'firefox', 'webkit'];

function parse() {
  const currentStrings = [];
  const final = [];

  for (let i = 0; i < browser.length; i++) {
    for (let j = 0; j < deviceType.length; j++) {
      const filePath = path.join(__dirname, `../reports/${browser[i]}/${deviceType[j]}/report.xml`);
      let currentString = '';
      const testName = `${browser[i]} ${deviceType[j]}`;
      const fn = new Promise((resolve) => {
        fs.stat(filePath, (err) => {
          if (err) {
            resolve();
          } else {
            fs.readFile(filePath, 'utf-8', (err, data) => {
              currentString = data.match(/<testsuites .*>\n(.*\n)*<\/testsuites>/g);
              currentString = currentString.toString().replace('Mocha Tests', testName);
              currentStrings.splice(i * 4 + j, 0, currentString);
              resolve();
            });
          }
        });
      });
      final.push(fn);
    }
  }

  Promise.all(final).then(() => {
    let tests = 0;
    let failures = 0;
    let time = 0;
    let xmlString = '';

    currentStrings.forEach((currentString) => {
      const currentTestData = currentString.match(/<testsuites .*>\n/g)[0].match(/"\d*.?\d*"/g);
      const currentTestDataNumber = currentTestData.map((testData) =>
        testData.replace(/[^0-9,.]/g, ''),
      );
      time += Math.round(Number(currentTestDataNumber[0]) * 100) / 100;
      tests += Number(currentTestDataNumber[1]);
      failures += Number(currentTestDataNumber[2]);
      xmlString += currentString + '\n';
    });

    const moduleInfo = `<moduleinfo name="Login" time="${time}" tests="${tests}" failures="${failures}">\n</moduleinfo>\n`;
    const xmlData = `<?xml version="1.0" encoding="UTF-8"?>\n`;

    xmlString = xmlData + moduleInfo + xmlString;

    const filePath = path.join(__dirname, `../reports/report.xml`);

    fs.writeFile(filePath, xmlString, 'utf-8', (err) => {
      if (err) throw err;
      console.log('Data has been replaced!');
    });
  });
}

parse();
