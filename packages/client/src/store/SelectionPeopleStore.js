import { action, computed, makeObservable, observable } from "mobx";
import {
  EmployeeStatus,
  EmployeeActivationStatus,
} from "@docspace/common/constants";
import { getUserStatus } from "../helpers/people-helpers";
import store from "client/store";
const { auth: authStore } = store;

class SelectionStore {
  selection = [];
  bufferSelection = null;
  selected = "none";

  constructor(peopleStore) {
    this.peopleStore = peopleStore;
    makeObservable(this, {
      selection: observable,
      bufferSelection: observable,
      selected: observable,
      selectUser: action,
      setBufferSelection: action,
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
    if (selection?.length && !this.selection.length) {
      this.bufferSelection = null;
    }
  };

  setBufferSelection = (bufferSelection) => {
    this.bufferSelection = bufferSelection;
    this.setSelection([]);
  };

  selectUser = (user) => {
    if (!this.selection.length) {
      this.bufferSelection = null;
    }
    this.selection.push(user);
  };

  deselectUser = (user) => {
    const newData = this.selection.filter((el) => el.id !== user.id);
    return (this.selection = newData);
  };

  selectAll = () => {
    this.bufferSelection = null;
    const list = this.peopleStore.usersStore.peopleList;
    this.setSelection(list);
  };

  clearSelection = () => {
    return this.setSelection([]);
  };

  selectByStatus = (status) => {
    this.bufferSelection = null;
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
        return status === "active";
      case "pending":
        return status === "pending";
      case "disabled":
        return status === "disabled";
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
    this.bufferSelection = null;
    this.selected = selected;
    const list = this.peopleStore.usersStore.peopleList;
    this.setSelection(this.getUsersBySelected(list, selected));

    return selected;
  };

  get hasAnybodySelected() {
    return this.selection.length > 0;
  }

  get hasUsersToMakeEmployees() {
    const isOwner = authStore.userStore.user.isOwner;
    const users = this.selection.filter((x) => {
      return (
        (!x.isOwner &&
          !x.isAdmin &&
          x.status !== EmployeeStatus.Disabled &&
          x.id !== authStore.userStore.user.id) ||
        (x.isAdmin &&
          isOwner &&
          x.status !== EmployeeStatus.Disabled &&
          x.id !== authStore.userStore.user.id)
      );
    });

    return users.length > 0;
  }

  get getUsersToMakeEmployeesIds() {
    const users = this.selection.filter((x) => {
      return (
        !x.isOwner &&
        x.status !== EmployeeStatus.Disabled &&
        x.id !== authStore.userStore.user.id
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
        x.id !== authStore.userStore.user.id
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
        x.id !== authStore.userStore.user.id
      );
    });
    return users.map((u) => u.id);
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

  get getUsersToActivateIds() {
    const users = this.selection.filter(
      (x) =>
        !x.isOwner &&
        x.status !== EmployeeStatus.Active &&
        x.id !== authStore.userStore.user.id
    );
    return users.map((u) => u.id);
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

  get getUsersToDisableIds() {
    const users = this.selection.filter(
      (x) =>
        !x.isOwner &&
        x.status !== EmployeeStatus.Disabled &&
        x.id !== authStore.userStore.user.id
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
