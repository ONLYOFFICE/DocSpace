import { makeObservable, action, observable } from "mobx";

class PrimaryProgressDataStore {
  percent = 0;
  label = "";
  visible = false;
  icon = "upload";
  alert = false;
  loadingFile = null;
  errors = 0;

  constructor() {
    makeObservable(this, {
      percent: observable,
      label: observable,
      visible: observable,
      icon: observable,
      alert: observable,
      loadingFile: observable,
      errors: observable,

      setPrimaryProgressBarData: action,
      clearPrimaryProgressData: action,
      setPrimaryProgressBarErrors: action,
    });
  }

  setPrimaryProgressBarData = (primaryProgressData) => {
    const progressDataItems = Object.keys(primaryProgressData);
    for (let key of progressDataItems) {
      this[key] = primaryProgressData[key];
    }
  };

  clearPrimaryProgressData = () => {
    this.setPrimaryProgressBarData({
      visible: false,
      percent: 0,
      label: "",
      icon: "",
      alert: false,
      errors: 0,
    });
  };

  setPrimaryProgressBarShowError = (error) => {
    this.alert = error;
  };

  setPrimaryProgressBarErrors = (errors) => {
    this.errors = errors;
  };
}

export default PrimaryProgressDataStore;
