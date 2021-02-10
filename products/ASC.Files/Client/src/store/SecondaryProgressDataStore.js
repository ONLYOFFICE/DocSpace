import { makeObservable, action, observable } from "mobx";

class SecondaryProgressDataStore {
  percent = 0;
  label = "";
  visible = false;
  icon = "trash";
  alert = false;

  constructor() {
    makeObservable(this, {
      percent: observable,
      label: observable,
      visible: observable,
      icon: observable,
      alert: observable,

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
  };
}

export default SecondaryProgressDataStore;
