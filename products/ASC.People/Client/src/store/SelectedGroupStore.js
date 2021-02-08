import { action, makeObservable, observable } from "mobx";

class SelectedGroupStore {
  selectedGroup = null;

  constructor() {
    makeObservable(this, {
      selectedGroup: observable,
      setSelectedGroup: action,
    });
  }

  setSelectedGroup = (group) => {
    console.log("prev data: ", this.selectedGroup);
    this.selectedGroup = group;
    console.log("new data: ", this.selectedGroup);
  };
}

export default SelectedGroupStore;
