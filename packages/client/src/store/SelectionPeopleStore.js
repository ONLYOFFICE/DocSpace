import { action, computed, makeObservable, observable } from "mobx";
import {
  EmployeeStatus,
  EmployeeActivationStatus,
} from "@docspace/common/constants";
import { getUserStatus } from "../helpers/people-helpers";

class SelectionStore {
  peopleStore = null;
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
      setSelection: action,
      clearSelection: action,
      selectByStatus: action,
      setSelected: action,
      hasAnybodySelected: computed,
      hasUsersToMakeEmployees: computed,
      getUsersToMakeEmployeesIds: computed,
      hasUsersToMakeGuests: computed,
      getUsersToMakeGuestsIds: computed,
      hasUsersToActivate: computed,
      getUsersToActivateIds: computed,
      hasUsersToDisable: computed,
      getUsersToDisableIds: computed,
      hasUsersToInvite: computed,
      getUsersToInviteIds: computed,
      hasUsersToRemove: computed,
      getUsersToRemoveIds: computed,
    });
  }

  setSelection = (selection) => {
    this.selection = selection;
  };

  selectUser = (user) => {
    return this.selection.push(user);
  };

  deselectUser = (user) => {
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

  getUserChecked = (user, selected) => {
    const status = getUserStatus(user);
    switch (selected) {
      case "all":
        return true;
      case "active":
        return status === "normal";
      case "disabled":
        return status === "disabled";
      case "invited":
        return status === "pending";
      default:
        return false;
    }
  };

  getUsersBySelected = (users, selected) => {
    let newSelection = [];
    users.forEach((user) => {
      const checked = this.getUserChecked(user, selected);

      if (checked) newSelection.push(user);
    });

    return newSelection;
  };

  setSelected = (selected) => {
    this.selected = selected;
    const list = this.peopleStore.usersStore.peopleList;
    this.setSelection(this.getUsersBySelected(list, selected));

    return selected;
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
        x.id !== this.peopleStore.authStore.userStore.user.id
      );
    });
    return !!users.length;
  }

  get getUsersToMakeEmployeesIds() {
    const users = this.selection.filter((x) => {
      return (
        !x.isAdmin &&
        !x.isOwner &&
        x.isVisitor &&
        x.status !== EmployeeStatus.Disabled &&
        x.id !== this.peopleStore.authStore.userStore.user.id
      );
    });
    return users.map((u) => u.id);
  }

  get hasUsersToMakeGuests() {
    const users = this.selection.filter((x) => {
      return (
        !x.isAdmin &&
        !x.isOwner &&
        !x.isVisitor &&
        x.status !== EmployeeStatus.Disabled &&
        x.id !== this.peopleStore.authStore.userStore.user.id
      );
    });
    return !!users.length;
  }

  get getUsersToMakeGuestsIds() {
    const users = this.selection.filter((x) => {
      return (
        !x.isAdmin &&
        !x.isOwner &&
        !x.isVisitor &&
        x.status !== EmployeeStatus.Disabled &&
        x.id !== this.peopleStore.authStore.userStore.user.id
      );
    });
    return users.map((u) => u.id);
  }

  get hasUsersToActivate() {
    const users = this.selection.filter(
      (x) =>
        !x.isOwner &&
        x.status !== EmployeeStatus.Active &&
        x.id !== this.peopleStore.authStore.userStore.user.id
    );
    return !!users.length;
  }

  get getUsersToActivateIds() {
    const users = this.selection.filter(
      (x) =>
        !x.isOwner &&
        x.status !== EmployeeStatus.Active &&
        x.id !== this.peopleStore.authStore.userStore.user.id
    );
    return users.map((u) => u.id);
  }

  get hasUsersToDisable() {
    const users = this.selection.filter(
      (x) =>
        !x.isOwner &&
        x.status !== EmployeeStatus.Disabled &&
        x.id !== this.peopleStore.authStore.userStore.user.id
    );
    return !!users.length;
  }

  get getUsersToDisableIds() {
    const users = this.selection.filter(
      (x) =>
        !x.isOwner &&
        x.status !== EmployeeStatus.Disabled &&
        x.id !== this.peopleStore.authStore.userStore.user.id
    );
    return users.map((u) => u.id);
  }

  get hasUsersToInvite() {
    const users = this.selection.filter(
      (x) =>
        x.activationStatus === EmployeeActivationStatus.Pending &&
        x.status === EmployeeStatus.Active
    );
    return !!users.length;
  }

  get getUsersToInviteIds() {
    const users = this.selection.filter(
      (x) =>
        x.activationStatus === EmployeeActivationStatus.Pending &&
        x.status === EmployeeStatus.Active
    );
    return users.map((u) => u.id);
  }

  get hasUsersToRemove() {
    const users = this.selection.filter(
      (x) => x.status === EmployeeStatus.Disabled
    );
    return !!users.length;
  }

  get getUsersToRemoveIds() {
    const users = this.selection.filter(
      (x) => x.status === EmployeeStatus.Disabled
    );
    return users.map((u) => u.id);
  }
}

export default SelectionStore;
