import { makeObservable, action, observable } from "mobx";

class DialogsStore {
  sharingPanelVisible = false;
  ownerPanelVisible = false;

  constructor() {
    makeObservable(this, {
      sharingPanelVisible: observable,
      ownerPanelVisible: observable,

      setSharingPanelVisible: action,

      setChangeOwnerPanelVisible: action,
    });
  }

  setSharingPanelVisible = (sharingPanelVisible) => {
    this.sharingPanelVisible = sharingPanelVisible;
  };

  setChangeOwnerPanelVisible = (ownerPanelVisible) => {
    this.ownerPanelVisible = ownerPanelVisible;
  };
}

export default new DialogsStore();
