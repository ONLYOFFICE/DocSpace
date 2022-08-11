import { makeObservable, action, observable, computed } from "mobx";

class SecondaryProgressDataStore {
  percent = 0;
  label = "";
  visible = false;
  icon = "trash";
  alert = false;
  filesCount = 0;
  itemsSelectionLength = 0;
  itemsSelectionTitle = null;

  constructor() {
    makeObservable(this, {
      percent: observable,
      label: observable,
      visible: observable,
      icon: observable,
      alert: observable,
      filesCount: observable,
      itemsSelectionLength: observable,
      itemsSelectionTitle: observable,

      isSecondaryProgressFinished: computed,

      setSecondaryProgressBarData: action,
      clearSecondaryProgressData: action,
      setItemsSelectionLength: action,
      setItemsSelectionTitle: action,
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

  setItemsSelectionTitle = (itemsSelectionTitle) => {
    this.itemsSelectionTitle = itemsSelectionTitle;
  };

  setItemsSelectionLength = (itemsSelectionLength) => {
    this.itemsSelectionLength = itemsSelectionLength;
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
