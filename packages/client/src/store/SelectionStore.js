import { makeAutoObservable } from "mobx";

class SelectionStore {
  selection = [];
  selected = "none";

  constructor(settingsSetupStore) {
    this.settingsSetupStore = settingsSetupStore;
    makeAutoObservable(this);
  }

  setSelection = (selection) => {
    this.selection = selection;
  };

  selectUser = (user) => {
    return this.selection.push(user);
  };

  deselectUser = (user) => {
    if (!user) {
      this.selected = "none";
      this.selection = [];
      return;
    }

    const newData = this.selection.filter((el) => el.id !== user.id);
    return (this.selection = newData);
  };

  selectAll = () => {
    const list = this.peopleStore.usersStore.peopleList;
    this.setSelection(list);
  };

  clearSelection = () => {
    return this.setSelection([]);
  };

  selectByStatus = (status) => {
    const list = this.peopleStore.usersStore.peopleList.filter(
      (u) => u.status === status
    );

    return (this.selection = list);
  };

  getUserChecked = () => {
    switch (this.selected) {
      case "all":
        return true;
      default:
        return false;
    }
  };

  getUsersBySelected = (users) => {
    let newSelection = [];
    users.forEach((user) => {
      const checked = this.getUserChecked();

      if (checked) newSelection.push(user);
    });

    return newSelection;
  };

  isUserSelected = (userId) => {
    return this.selection.some((el) => el.id === userId);
  };

  setSelected = (selected) => {
    const { admins } = this.settingsSetupStore.security.accessRight;
    this.selected = selected;
    this.setSelection(this.getUsersBySelected(admins));

    return selected;
  };

  get isHeaderVisible() {
    return !!this.selection.length;
  }

  get isHeaderIndeterminate() {
    //console.log("RUN isHeaderIndeterminate");
    const { admins } = this.settingsSetupStore.security.accessRight;
    return (
      this.isHeaderVisible &&
      !!this.selection.length &&
      this.selection.length < admins.length
    );
  }

  get isHeaderChecked() {
    const { admins } = this.settingsSetupStore.security.accessRight;
    return this.isHeaderVisible && this.selection.length === admins.length;
  }
}

export default SelectionStore;
