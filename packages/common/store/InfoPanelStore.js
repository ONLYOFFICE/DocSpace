import { makeAutoObservable } from "mobx";

class InfoPanelStore {
  selection = null;
  isVisible = false;
  roomView = "members";
  itemView = "history";

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

  setView = (view) => {
    this.roomView = view;
    this.itemView = view === "members" ? "history" : view;
  };
}

export default InfoPanelStore;
