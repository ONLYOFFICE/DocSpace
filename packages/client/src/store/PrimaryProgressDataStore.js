import { makeObservable, action, observable } from "mobx";

class PrimaryProgressDataStore {
  percent = 0;
  label = "";
  visible = false;
  icon = "upload";
  alert = false;
  loadingFile = null;

  constructor() {
    makeObservable(this, {
      percent: observable,
      label: observable,
      visible: observable,
      icon: observable,
      alert: observable,
      loadingFile: observable,

      setPrimaryProgressBarData: action,
      clearPrimaryProgressData: action,
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
    });
  };

  setPrimaryProgressBarShowError = (error) => {
    this.alert = error;
  };
}

export default PrimaryProgressDataStore;
