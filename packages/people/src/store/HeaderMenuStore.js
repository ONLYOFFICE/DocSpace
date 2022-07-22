import { computed, makeObservable } from "mobx";

class HeaderMenuStore {
  constructor(peopleStore) {
    this.peopleStore = peopleStore;
    makeObservable(this, {
      isHeaderVisible: computed,
      isHeaderIndeterminate: computed,
      isHeaderChecked: computed,
    });
  }

  get isHeaderVisible() {
    const { selection, selected } = this.peopleStore.selectionStore;
    return !!selection.length || selected !== "none";
  }

  get isHeaderIndeterminate() {
    const { selection } = this.peopleStore.selectionStore;
    const { users } = this.peopleStore.usersStore;

    return (
      this.isHeaderVisible &&
      !!selection.length &&
      selection.length < users.length
    );
  }

  get isHeaderChecked() {
    const { selection } = this.peopleStore.selectionStore;
    const { users } = this.peopleStore.usersStore;

    return this.isHeaderVisible && selection.length === users.length;
  }
}

export default HeaderMenuStore;
