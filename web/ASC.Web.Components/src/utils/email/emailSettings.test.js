import { EmailSettings, isEqualEmailSettings, checkAndConvertEmailSettings } from './index';

const defaultEmailSettingsObj = {
 allowDomainPunycode: false,
 allowLocalPartPunycode: false,
 allowDomainIp: false,
 allowStrictLocalPart: true,
 allowSpaces: false,
 allowName: false,
 allowLocalDomainName: false
};
describe('emailSettings', () => {

 it('get default settings from instance', () => {

  const email = new EmailSettings();
  const settings = email.getSettings();
  expect(settings).toStrictEqual(defaultEmailSettingsObj);
 });

 it('change and get settings from instance', () => {

  const emailSettingsObj = {
   allowDomainPunycode: false,
   allowLocalPartPunycode: false,
   allowDomainIp: false,
   allowStrictLocalPart: true,
   allowSpaces: false,
   allowName: false,
   allowLocalDomainName: true
  };

  const emailSettings = new EmailSettings();
  emailSettings.allowLocalDomainName = true;
  const settings = emailSettings.getSettings();

  expect(settings).toStrictEqual(emailSettingsObj);
 });

 it('set and get allowStrictLocalPart setting', () => {
  const emailSettings = new EmailSettings();
  emailSettings.allowStrictLocalPart = false;

  expect(emailSettings.allowStrictLocalPart).toBe(false);
 });

 it('disable settings', () => {

  const disabledSettings = {
   allowDomainPunycode: true,
   allowLocalPartPunycode: true,
   allowDomainIp: true,
   allowStrictLocalPart: false,
   allowSpaces: true,
   allowName: true,
   allowLocalDomainName: true
  };
  const emailSettings = new EmailSettings();
  emailSettings.disableAllSettings();
  const newSettings = emailSettings.getSettings();

  expect(newSettings).toStrictEqual(disabledSettings);
 });

 // test isEqualEmailSettings function

 it('is not equal email settings', () => {
  const emailSettings = new EmailSettings();
  const emailSettings2 = new EmailSettings();

  emailSettings.allowStrictLocalPart = false;
  const isEqual = isEqualEmailSettings(emailSettings, emailSettings2);

  expect(isEqual).toBe(false);
 });

 it('is equal email settings', () => {
  const emailSettings = new EmailSettings();
  const emailSettings2 = new EmailSettings();
  const isEqual = isEqualEmailSettings(emailSettings, emailSettings2);

  expect(isEqual).toBe(true);
 });

 // test checkAndConvertEmailSettings function

 it('passed instance of default EmailSettings, return same instance', () => {

  const emailSettings = new EmailSettings();
  const convertedSettings = checkAndConvertEmailSettings(emailSettings);

  expect(convertedSettings).toStrictEqual(emailSettings);
 });

 it('passed object with default settings, return instance of default EmailSettings', () => {

  const convertedSettings = checkAndConvertEmailSettings(defaultEmailSettingsObj);
  const emailSettings = new EmailSettings();

  expect(convertedSettings).toStrictEqual(emailSettings);
 });

 it('passed instance of EmailSettings, return same instance', () => {

  const emailSettings = new EmailSettings();
  emailSettings.allowLocalDomainName = true;
  const convertedSettings = checkAndConvertEmailSettings(emailSettings);

  expect(convertedSettings).toStrictEqual(emailSettings);
 });

 it('passed object with settings, return instance of EmailSettings', () => {

  const emailSettingsObj = {
   allowDomainPunycode: true,
   allowLocalPartPunycode: true,
   allowDomainIp: false,
   allowStrictLocalPart: true,
   allowSpaces: false,
   allowName: false,
   allowLocalDomainName: false
  };

  const convertedSettings = checkAndConvertEmailSettings(emailSettingsObj);
  const emailSettings = new EmailSettings();
  emailSettings.allowDomainPunycode = true;
  emailSettings.allowLocalPartPunycode = true;

  expect(convertedSettings).toStrictEqual(emailSettings);
 });

});
