import { isDesktop } from "@appserver/components/utils/device";
import { makeAutoObservable } from "mobx";

class InfoPanelStore {
  isVisible = false;
  showCurrentFolder = false;

  constructor() {
    makeAutoObservable(this);
  }

  setShowCurrentFolder = (bool) => {
    this.showCurrentFolder = bool;
  };

  onItemClick = (checked = true) => {
    if (localStorage.getItem("disableOpenOnFileClick")) return;
    if (!this.isVisible && isDesktop()) {
      this.showCurrentFolder = false;
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

  get showCurrentFolder() {
    return this.showCurrenFolder;
  }

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
