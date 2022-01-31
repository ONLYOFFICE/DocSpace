import { makeObservable, action, observable, computed } from "mobx";

class SecondaryProgressDataStore {
  percent = 0;
  label = "";
  visible = false;
  icon = "trash";
  alert = false;
  filesCount = 0;

  constructor() {
    makeObservable(this, {
      percent: observable,
      label: observable,
      visible: observable,
      icon: observable,
      alert: observable,
      filesCount: observable,

      isSecondaryProgressFinished: computed,

      setSecondaryProgressBarData: action,
      clearSecondaryProgressData: action,
    });
  }

  setSecondaryProgressBarData = (secondaryProgressData) => {
    const progressDataItems = Object.keys(secondaryProgressData);
    for (let key of progressDataItems) {
      if (key in this) {
        this[key] = secondaryProgressData[key];
      }
    }
  };

  clearSecondaryProgressData = () => {
    this.percent = 0;
    this.label = "";
    this.visible = false;
    this.icon = "";
    this.alert = false;
    this.filesCount = 0;
  };

  get isSecondaryProgressFinished() {
    return this.percent === 100;
  }
}

export default SecondaryProgressDataStore;
