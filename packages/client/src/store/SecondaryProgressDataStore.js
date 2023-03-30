import { makeAutoObservable } from "mobx";

class SecondaryProgressDataStore {
  percent = 0;
  label = "";
  visible = false;
  icon = "trash";
  alert = false;
  filesCount = 0;
  itemsSelectionLength = 0;
  itemsSelectionTitle = null;

  secondaryOperationsArray = [];

  constructor() {
    makeAutoObservable(this);
  }

  setSecondaryProgressBarData = (secondaryProgressData) => {
    const progressIndex = this.secondaryOperationsArray.findIndex(
      (p) => p.operationId === secondaryProgressData.operationId
    );

    if (progressIndex !== -1) {
      this.secondaryOperationsArray[progressIndex] = secondaryProgressData;

      if (progressIndex === 0) {
        const progressDataItems = Object.keys(secondaryProgressData);
        for (let key of progressDataItems) {
          if (key in this) {
            this[key] = secondaryProgressData[key];
          }
        }
      }
    } else {
      if (this.secondaryOperationsArray.length === 0) {
        const progressDataItems = Object.keys(secondaryProgressData);
        for (let key of progressDataItems) {
          if (key in this) {
            this[key] = secondaryProgressData[key];
          }
        }
      }

      this.secondaryOperationsArray.push(secondaryProgressData);
    }
  };

  setItemsSelectionTitle = (itemsSelectionTitle) => {
    this.itemsSelectionTitle = itemsSelectionTitle;
  };

  setItemsSelectionLength = (itemsSelectionLength) => {
    this.itemsSelectionLength = itemsSelectionLength;
  };

  clearSecondaryProgressData = (operationId) => {
    const progressIndex = this.secondaryOperationsArray.findIndex(
      (p) => p.operationId === operationId
    );

    if (progressIndex !== -1) {
      this.secondaryOperationsArray = this.secondaryOperationsArray.filter(
        (p) => p.operationId !== operationId
      );

      if (this.secondaryOperationsArray.length > 0) {
        const nextOperation = this.secondaryOperationsArray[0];

        this.percent = nextOperation.percent;
        this.label = nextOperation.label;
        this.visible = nextOperation.visible;
        this.icon = nextOperation.icon;
        this.alert = nextOperation.alert;
        this.filesCount = nextOperation.filesCount;

        return;
      }
    }

    if (this.secondaryOperationsArray.length <= 1) {
      this.percent = 0;
      this.label = "";
      this.visible = false;
      this.icon = "";
      this.alert = false;
      this.filesCount = 0;
    }
  };

  get isSecondaryProgressFinished() {
    return this.percent === 100;
  }
}

export default SecondaryProgressDataStore;
