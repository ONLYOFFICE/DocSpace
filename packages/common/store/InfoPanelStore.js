import { makeAutoObservable } from "mobx";

class InfoPanelStore {
  selection = null;
  isVisible = false;
  roomState = "members";

  constructor() {
    makeAutoObservable(this);
  }

  setSelection = (selection) => {
    this.selection = selection;
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

  setRoomState = (str) => {
    this.roomState = str;
  };
}

export default InfoPanelStore;
