import { makeObservable, action, observable, computed } from "mobx";

class PrimaryProgressDataStore {
  percent = 0;
  label = "";
  visible = false;
  icon = "upload";
  alert = false;

  constructor() {
    makeObservable(this, {
      percent: observable,
      label: observable,
      visible: observable,
      icon: observable,
      alert: observable,

      loadingFile: computed,

      setPrimaryProgressBarData: action,
    });
  }

  setPrimaryProgressBarData = (primaryProgressData) => {
    const progressDataItems = Object.keys(primaryProgressData);
    for (let key of progressDataItems) {
      if (key in this) {
        this[key] = primaryProgressData[key];
      }
    }
  };
  //TODO: loadingFile
  get loadingFile() {
    if (!this.loadingFile || !this.loadingFile.uniqueId) return null;
    return this.loadingFile;
  }
}

export default PrimaryProgressDataStore;
