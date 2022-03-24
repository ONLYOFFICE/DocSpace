import { isDesktop } from "@appserver/components/utils/device";
import { makeAutoObservable } from "mobx";

class InfoPanelStore {
  isVisible = false;

  constructor() {
    makeAutoObservable(this);
  }

  onItemClick = (checked = true) => {
    if (localStorage.getItem("disableOpenOnFileClick")) return;
    if (!this.isVisible && isDesktop()) {
      if (checked) this.isVisible = true;
    }
  };

  onHeaderCrossClick = () => {
    localStorage.setItem("disableOpenOnFileClick", "true");
    this.isVisible = false;
  };

  revertHeaderCrossClick = () => {
    localStorage.removeItem("disableOpenOnFileClick");
  };

  toggleIsVisible = () => {
    this.isVisible = !this.isVisible;
  };

  setVisible = () => {
    this.isVisible = true;
  };

  setIsVisible = (bool) => {
    this.isVisible = bool;
  };

  get isVisible() {
    return this.isVisible;
  }
}

export default InfoPanelStore;
