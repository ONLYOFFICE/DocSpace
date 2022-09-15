import { makeAutoObservable } from "mobx";
import { Events } from "@docspace/common/constants";

class InfoPanelStore {
  isVisible = false;

  constructor() {
    makeAutoObservable(this);
  }

  toggleIsVisible = () => {
    this.isVisible = !this.isVisible;
    const event = new Event(Events.CHANGE_COLUMN);

    window.dispatchEvent(event);
  };

  setVisible = () => {
    this.isVisible = true;
    const event = new Event(Events.CHANGE_COLUMN);

    window.dispatchEvent(event);
  };

  setIsVisible = (bool) => {
    this.isVisible = bool;

    const event = new Event(Events.CHANGE_COLUMN);

    window.dispatchEvent(event);
  };
}

export default InfoPanelStore;
