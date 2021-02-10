import { action, makeObservable, observable } from "mobx";

class SelectionStore {
  selection = [];

  constructor(peopleStore) {
    this.peopleStore = peopleStore;
    makeObservable(this, {
      selection: observable,
      selectUser: action,
      deselectUser: action,
    });
  }

  selectUser = (user) => {
    const u = user;
    this.selection.push(u);
    console.log(this.selection);
  };

  deselectUser = (user) => {
    const newData = this.selection.filter((el) => el.id !== user.id);
    this.selection = newData;
    console.log(this.selection);
  };
}

export default SelectionStore;
