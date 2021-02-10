import { makeObservable, action, observable } from "mobx";

class DialogsStore {
  convertDialogVisible = false;
  sharingPanelVisible = false;
  uploadPanelVisible = false;
  ownerPanelVisible = false;

  constructor() {
    makeObservable(this, {
      convertDialogVisible: observable,
      sharingPanelVisible: observable,
      uploadPanelVisible: observable,
      ownerPanelVisible: observable,

      setConvertDialogVisible: action,
      setSharingPanelVisible: action,
      setUploadPanelVisible: action,
      setChangeOwnerPanelVisible: action,
    });
  }

  setConvertDialogVisible = (convertDialogVisible) => {
    this.convertDialogVisible = convertDialogVisible;
  };

  setSharingPanelVisible = (sharingPanelVisible) => {
    this.sharingPanelVisible = sharingPanelVisible;
  };

  setUploadPanelVisible = (uploadPanelVisible) => {
    this.uploadPanelVisible = uploadPanelVisible;
  };

  setChangeOwnerPanelVisible = (ownerPanelVisible) => {
    this.ownerPanelVisible = ownerPanelVisible;
  };
}

export default DialogsStore;
