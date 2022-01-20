const fs = require('fs');
const path = require('path');

const settingsFile = require(path.resolve(
  __dirname,
  '../mocking/mock-data/settings/settingsTranslation.json',
));

const selfFile = require(path.resolve(
  __dirname,
  '../mocking/mock-data/people/selfTranslation.json',
));

function changeCulture(culture) {
  settingsFile.response.culture = culture;
  selfFile.response.cultureName = culture;

  fs.writeFile(
    path.resolve(__dirname, '../mocking/mock-data/settings/settingsTranslation.json'),
    JSON.stringify(settingsFile, null, 2),
    function writeJSON(err) {
      if (err) return console.log(err);
    },
  );

  fs.writeFile(
    path.resolve(__dirname, '../mocking/mock-data/people/selfTranslation.json'),
    JSON.stringify(selfFile, null, 2),
    function writeJSON(err) {
      if (err) return console.log(err);
    },
  );
}

module.exports = changeCulture;
