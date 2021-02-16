import { api } from "asc-web-common";
import { makeAutoObservable } from "mobx";

class WizardStore {
  isWizardLoaded = false;
  isLicenseRequired = false;
  machineName = "unknown";
  licenseUpload = null;

  constructor() {
    makeAutoObservable(this);
  }

  setIsWizardLoaded = (isWizardLoaded) => {
    this.isWizardLoaded = isWizardLoaded;
  };

  setMachineName = (machineName) => {
    this.machineName = machineName;
  };

  setIsRequiredLicense = (isRequired) => {
    this.isLicenseRequired = isRequired;
  };

  setLicenseUpload = (message) => {
    this.licenseUpload = message;
  };

  resetLicenseUploaded = () => {
    this.setLicenseUpload(null);
  };

  getMachineName = async (token) => {
    const machineName = await api.settings.getMachineName(token);
    this.machineName = machineName;
  };

  setPortalOwner = async (
    email,
    hash,
    lng,
    timeZone,
    confirmKey,
    analytics
  ) => {
    const response = await api.settings.setPortalOwner(
      email,
      hash,
      lng,
      timeZone,
      confirmKey,
      analytics
    );

    console.log("setPortalOwner", response);

    return Promise.resolve(response);
  };

  getIsRequiredLicense = async () => {
    const isRequired = await api.settings.getIsLicenseRequired();

    this.setIsRequiredLicense(isRequired);
  };

  setLicense = async (confirmKey, data) => {
    const message = await api.settings.setLicense(confirmKey, data);

    this.setLicenseUpload(message);
  };
}

export default WizardStore;
