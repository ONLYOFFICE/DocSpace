import { action, makeObservable, observable } from "mobx";

class EditingFormStore {
  isEdit = false;
  isVisibleDataLossDialog = false;
  callback = null;

  constructor() {
    makeObservable(this, {
      isEdit: observable,
      isVisibleDataLossDialog: observable,
      callback: observable,
      setIsEditingForm: action,
      setIsVisibleDataLossDialog: action,
    });
  }

  setIsEditingForm = (isEdit) => {
    return (this.isEdit = isEdit);
  };

  setIsVisibleDataLossDialog = (isVisible, callback = null) => {
    this.isVisibleDataLossDialog = isVisible;
    this.callback = callback;
  };
}

export default EditingFormStore;
