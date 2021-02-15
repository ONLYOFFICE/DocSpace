import { action, makeObservable, observable } from "mobx";

class EditingFormStore {
  isEdit = false;
  isVisibleDataLossDialog = false;

  constructor() {
    makeObservable(this, {
      isEdit: observable,
      isVisibleDataLossDialog: observable,
      setIsEditingForm: action,
      setIsVisibleDataLossDialog: action,
    });
  }

  setIsEditingForm = (isEdit) => {
    return (this.isEdit = isEdit);
  };

  setIsVisibleDataLossDialog = (isVisible, callback = null) => {
    if (typeof callback === "function") {
      this.isVisibleDataLossDialog = isVisible;
      return callback();
    }
    return (this.isVisibleDataLossDialog = isVisible);
  };
}

export default EditingFormStore;
