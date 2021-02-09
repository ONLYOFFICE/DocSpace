import { action, makeObservable, observable } from "mobx";

class SelectedGroupStore {
  selectedGroup = null;

  constructor(peopleStore) {
    this.peopleStore = peopleStore;
    makeObservable(this, {
      selectedGroup: observable,
      setSelectedGroup: action,
    });
  }

  selectGroup = (groupId) => {
    const { filter } = this.peopleStore.filterStore;

    let newFilter = filter.clone();
    newFilter.group = groupId;

    this.peopleStore.usersStore.getUsersList(newFilter);
  };

  setSelectedGroup = (group) => {
    console.log("prev data: ", this.selectedGroup);
    this.selectedGroup = group;
    console.log("new data: ", this.selectedGroup);
  };
}

export default SelectedGroupStore;
