import { makeObservable, action, observable } from "mobx";

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
}

export default PrimaryProgressDataStore;
