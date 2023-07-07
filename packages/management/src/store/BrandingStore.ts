import axios from "axios";
import { makeAutoObservable, runInAction } from "mobx";
import isEqual from "lodash/isEqual";

import { TLogoUrl } from "SRC_DIR/types/branding";

class BrandingStore {
  defaultWhiteLabelLogos: TLogoUrl[] | null = null;
  whiteLabelLogos: TLogoUrl[] | null = null;
  whiteLabelLogoText: string | null = null;
  defaultWhiteLabelLogoText: string | null = null;

  constructor() {
    makeAutoObservable(this);
  }

  initStore = async () => {
    const logos = await axios.get("http://localhost:3001/whitelabel");
    const text = await axios.get("http://localhost:3001/logotext");

    runInAction(() => {
      this.defaultWhiteLabelLogos = logos.data;
      this.whiteLabelLogos = logos.data;
      this.defaultWhiteLabelLogoText = text.data;
      this.whiteLabelLogoText = text.data;
    });
  };

  setWhiteLabelLogos = (whiteLabelLogos: TLogoUrl[]) => {
    this.whiteLabelLogos = whiteLabelLogos;
  };

  setWhiteLabelLogoText = (whiteLabelLogoText: string) => {
    this.whiteLabelLogoText = whiteLabelLogoText;
  };

  restoreDefault = async () => {
    const res = await axios.get("http://localhost:3001/defaultwhitelabel");

    runInAction(() => {
      this.defaultWhiteLabelLogos = res.data;
      this.whiteLabelLogos = res.data;
    });
  };

  get isEqualLogo() {
    return isEqual(this.whiteLabelLogos, this.defaultWhiteLabelLogos);
  }

  get isEqualText() {
    return this.whiteLabelLogoText === this.defaultWhiteLabelLogoText;
  }
}

export default BrandingStore;
