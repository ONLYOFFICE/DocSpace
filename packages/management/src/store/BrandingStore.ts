import axios from "axios";
import { makeAutoObservable, runInAction } from "mobx";
import isEqual from "lodash/isEqual";

import api from "@docspace/common/api";

import { TLogoUrl } from "SRC_DIR/types/branding";

type BrandingData = {
  logoText: string | null;
  logo: TLogoUrl[];
};

class BrandingStore {
  isInit = false;

  defaultWhiteLabelLogos: TLogoUrl[] | null = null;
  whiteLabelLogos: TLogoUrl[] | null = null;
  whiteLabelLogoText: string | null = null;
  defaultWhiteLabelLogoText: string | null = null;

  constructor() {
    makeAutoObservable(this);
  }

  initStore = async () => {
    if (this.isInit) return;
    this.isInit = true;

    const requests = [];
    requests.push(this.getWhiteLabelLogoUrls(), this.getWhiteLabelLogoText());

    return Promise.all(requests);
  };

  getWhiteLabelLogoUrls = async () => {
    const res = await api.settings.getLogoUrls();
    this.setWhiteLabelLogos(Object.values(res));
    this.setDefaultWhiteLabelLogos(Object.values(res));
  };

  getWhiteLabelLogoText = async () => {
    const res = await api.settings.getLogoText();
    this.setWhiteLabelLogoText(res);
    this.setDefaultWhiteLabelLogoText(res);
  };

  setWhiteLabelLogos = (whiteLabelLogos: TLogoUrl[]) => {
    this.whiteLabelLogos = whiteLabelLogos;
  };

  setDefaultWhiteLabelLogos = (defaultWhiteLabelLogos: TLogoUrl[]) => {
    this.defaultWhiteLabelLogos = defaultWhiteLabelLogos;
  };

  setWhiteLabelLogoText = (whiteLabelLogoText: string) => {
    this.whiteLabelLogoText = whiteLabelLogoText;
  };

  setDefaultWhiteLabelLogoText = (defaultWhiteLabelLogoText: string) => {
    this.defaultWhiteLabelLogoText = defaultWhiteLabelLogoText;
  };

  saveWhiteLabelSettings = async (data: BrandingData) => {
    const response = await api.settings.setWhiteLabelSettings(data);
    return Promise.resolve(response);
  };

  restoreDefault = async (isDefault: boolean) => {
    const res = await api.settings.restoreWhiteLabelSettings(isDefault);
    this.getWhiteLabelLogoUrls();
    this.getWhiteLabelLogoText();
  };

  get isEqualLogo() {
    return isEqual(this.whiteLabelLogos, this.defaultWhiteLabelLogos);
  }

  get isEqualText() {
    return this.whiteLabelLogoText === this.defaultWhiteLabelLogoText;
  }
}

export default BrandingStore;
