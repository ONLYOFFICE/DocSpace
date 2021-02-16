import { action, computed, makeObservable, observable } from "mobx";
import { constants, store } from "asc-web-common";

const { EmployeeStatus, EmployeeActivationStatus } = constants;
const { authStore } = store;

class SelectionStore {
  selection = [];
  selected = "none";

  constructor(peopleStore) {
    this.peopleStore = peopleStore;
    makeObservable(this, {
      selection: observable,
      selected: observable,
      selectUser: action,
      deselectUser: action,
      selectAll: action,
      clearSelection: action,
      selectByStatus: action,
      setSelected: action,
      hasAnybodySelected: computed,
      hasUsersToMakeEmployees: computed,
      hasUsersToMakeGuests: computed,
      hasUsersToActivate: computed,
      hasUsersToDisable: computed,
      hasUsersToInvite: computed,
      hasUsersToRemove: computed,
    });
  }

  selectUser = (user) => {
    const u = user;
    return this.selection.push(u);
  };

  deselectUser = (user) => {
    const newData = this.selection.filter((el) => el.id !== user.id);
    return (this.selection = newData);
  };

  selectAll = () => {
    const list = this.peopleStore.usersStore.composePeopleList();
    this.selection = list;
  };

  clearSelection = () => {
    return (this.selection = []);
  };

  selectByStatus = (status) => {
    const list = this.peopleStore.usersStore
      .composePeopleList()
      .filter((u) => u.status === status);

    return (this.selection = list);
  };

  setSelected = (selected) => {
    return (this.selected = selected);
  };

  get hasAnybodySelected() {
    return this.selection.length > 0;
  }

  get hasUsersToMakeEmployees() {
    const users = this.selection.filter((x) => {
      return (
        !x.isAdmin &&
        !x.isOwner &&
        x.isVisitor &&
        x.status !== EmployeeStatus.Disabled &&
        x.id !== authStore.userStore.user.id
      );
    });
    return !!users.length;
  }

  get hasUsersToMakeGuests() {
    const users = this.selection.filter((x) => {
      return (
        !x.isAdmin &&
        !x.isOwner &&
        !x.isVisitor &&
        x.status !== EmployeeStatus.Disabled &&
        x.id !== authStore.userStore.user.id
      );
    });
    return !!users.length;
  }

  get hasUsersToActivate() {
    const users = this.selection.filter(
      (x) =>
        !x.isOwner &&
        x.status !== EmployeeStatus.Active &&
        x.id !== authStore.userStore.user.id
    );
    return !!users.length;
  }

  get hasUsersToDisable() {
    const users = this.selection.filter(
      (x) =>
        !x.isOwner &&
        x.status !== EmployeeStatus.Disabled &&
        x.id !== authStore.userStore.user.id
    );
    return !!users.length;
  }

  get hasUsersToInvite() {
    const users = this.selection.filter(
      (x) =>
        x.activationStatus === EmployeeActivationStatus.Pending &&
        x.status === EmployeeStatus.Active
    );
    return !!users.length;
  }

  get hasUsersToRemove() {
    const users = this.selection.filter(
      (x) => x.status === EmployeeStatus.Disabled
    );
    return !!users.length;
  }
}

export default SelectionStore;
