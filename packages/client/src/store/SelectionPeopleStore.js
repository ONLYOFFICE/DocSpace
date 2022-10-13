import { action, computed, makeObservable, observable } from "mobx";
import {
  EmployeeStatus,
  EmployeeActivationStatus,
} from "@docspace/common/constants";
import { getUserStatus } from "../helpers/people-helpers";

class SelectionStore {
  peopleStore = null;
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
      getUsersToMakeEmployees: computed,
      hasUsersToActivate: computed,
      getUsersToActivateIds: computed,
      hasUsersToDisable: computed,
      getUsersToDisableIds: computed,
      hasUsersToInvite: computed,
      getUsersToInviteIds: computed,
      hasUsersToRemove: computed,
      getUsersToRemoveIds: computed,
      hasFreeUsers: computed,
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
    const { id, isOwner } = this.peopleStore.authStore.userStore.user;

    if (isOwner) {
      const users = this.selection.filter(
        (x) => x.status !== EmployeeStatus.Disabled && x.id !== id
      );

      return users.length > 0;
    }

    const users = this.selection.filter(
      (x) =>
        x.status !== EmployeeStatus.Disabled &&
        x.id !== id &&
        !x.isAdmin &&
        !x.isOwner &&
        x.isVisitor
    );

    return users.length > 0;
  }

  get getUsersToMakeEmployees() {
    const { id, isOwner } = this.peopleStore.authStore.userStore.user;

    if (isOwner) {
      const users = this.selection.filter(
        (x) => x.status !== EmployeeStatus.Disabled && x.id !== id
      );

      return users.map((u) => u);
    }

    const users = this.selection.filter(
      (x) =>
        x.status !== EmployeeStatus.Disabled &&
        x.id !== id &&
        !x.isAdmin &&
        !x.isOwner &&
        x.isVisitor
    );

    return users.map((u) => u);
  }

  get hasFreeUsers() {
    const users = this.selection.filter(
      (x) => x.status !== EmployeeStatus.Disabled && x.isVisitor
    );

    return users.length > 0;
  }

  get hasUsersToActivate() {
    const { id, isOwner, isAdmin } = this.peopleStore.authStore.userStore.user;

    if (isOwner) {
      const users = this.selection.filter(
        (x) => x.status !== EmployeeStatus.Active && x.id !== id
      );

      return users.length > 0;
    }

    if (isAdmin && !isOwner) {
      const users = this.selection.filter(
        (x) =>
          x.status !== EmployeeStatus.Active &&
          x.id !== id &&
          !x.isAdmin &&
          !x.isOwner
      );

      return users.length > 0;
    }

    const users = this.selection.filter(
      (x) =>
        x.status !== EmployeeStatus.Active &&
        x.id !== id &&
        !x.isAdmin &&
        !x.isOwner &&
        x.isVisitor
    );

    return users.length > 0;
  }

  get getUsersToActivateIds() {
    const { id, isOwner, isAdmin } = this.peopleStore.authStore.userStore.user;

    if (isOwner) {
      const users = this.selection.filter(
        (x) => x.status !== EmployeeStatus.Active && x.id !== id
      );

      return users.map((u) => u.id);
    }

    if (isAdmin && !isOwner) {
      const users = this.selection.filter(
        (x) =>
          x.status !== EmployeeStatus.Active &&
          x.id !== id &&
          !x.isAdmin &&
          !x.isOwner
      );

      return users.map((u) => u.id);
    }

    const users = this.selection.filter(
      (x) =>
        x.status !== EmployeeStatus.Active &&
        x.id !== id &&
        !x.isAdmin &&
        !x.isOwner &&
        x.isVisitor
    );

    return users.map((u) => u.id);
  }

  get hasUsersToDisable() {
    const { id, isOwner, isAdmin } = this.peopleStore.authStore.userStore.user;

    if (isOwner) {
      const users = this.selection.filter(
        (x) => x.status !== EmployeeStatus.Disabled && x.id !== id
      );

      return users.length > 0;
    }

    if (isAdmin && !isOwner) {
      const users = this.selection.filter(
        (x) =>
          x.status !== EmployeeStatus.Disabled &&
          x.id !== id &&
          !x.isAdmin &&
          !x.isOwner
      );

      return users.length > 0;
    }

    const users = this.selection.filter(
      (x) =>
        x.status !== EmployeeStatus.Disabled &&
        x.id !== id &&
        !x.isAdmin &&
        !x.isOwner &&
        x.isVisitor
    );

    return users.length > 0;
  }

  get getUsersToDisableIds() {
    const { id, isOwner, isAdmin } = this.peopleStore.authStore.userStore.user;

    if (isOwner) {
      const users = this.selection.filter(
        (x) => x.status !== EmployeeStatus.Active && x.id !== id
      );

      return users.map((u) => u.id);
    }

    if (isAdmin && !isOwner) {
      const users = this.selection.filter(
        (x) =>
          x.status !== EmployeeStatus.Active &&
          x.id !== id &&
          !x.isAdmin &&
          !x.isOwner
      );

      return users.map((u) => u.id);
    }

    const users = this.selection.filter(
      (x) =>
        x.status !== EmployeeStatus.Active &&
        x.id !== id &&
        !x.isAdmin &&
        !x.isOwner &&
        x.isVisitor
    );

    return users.map((u) => u.id);
  }

  get hasUsersToInvite() {
    const { id, isOwner, isAdmin } = this.peopleStore.authStore.userStore.user;

    if (isOwner) {
      const users = this.selection.filter(
        (x) =>
          x.activationStatus === EmployeeActivationStatus.Pending &&
          x.status === EmployeeStatus.Active &&
          x.id !== id
      );

      return users.length > 0;
    }

    const users = this.selection.filter(
      (x) =>
        x.activationStatus === EmployeeActivationStatus.Pending &&
        x.status === EmployeeStatus.Active &&
        x.id !== id &&
        !x.isAdmin &&
        !x.isOwner
    );

    return users.length > 0;
  }

  get getUsersToInviteIds() {
    const { id, isOwner, isAdmin } = this.peopleStore.authStore.userStore.user;

    if (isOwner) {
      const users = this.selection.filter(
        (x) =>
          x.activationStatus === EmployeeActivationStatus.Pending &&
          x.status === EmployeeStatus.Active &&
          x.id !== id
      );

      return users.map((u) => u.id);
    }

    const users = this.selection.filter(
      (x) =>
        x.activationStatus === EmployeeActivationStatus.Pending &&
        x.status === EmployeeStatus.Active &&
        x.id !== id &&
        !x.isAdmin &&
        !x.isOwner
    );

    return users.map((u) => u.id);
  }

  get hasUsersToRemove() {
    const { id, isOwner, isAdmin } = this.peopleStore.authStore.userStore.user;

    if (isOwner) {
      const users = this.selection.filter(
        (x) => x.status === EmployeeStatus.Disabled && x.id !== id
      );

      return users.length > 0;
    }

    if (isAdmin && !isOwner) {
      const users = this.selection.filter(
        (x) =>
          x.status === EmployeeStatus.Disabled &&
          x.id !== id &&
          !x.isAdmin &&
          !x.isOwner
      );

      return users.length > 0;
    }

    const users = this.selection.filter(
      (x) =>
        x.status === EmployeeStatus.Disabled &&
        x.id !== id &&
        !x.isAdmin &&
        !x.isOwner &&
        x.isVisitor
    );

    return users.length > 0;
  }

  get getUsersToRemoveIds() {
    const { id, isOwner, isAdmin } = this.peopleStore.authStore.userStore.user;

    if (isOwner) {
      const users = this.selection.filter(
        (x) => x.status === EmployeeStatus.Disabled && x.id !== id
      );

      return users.map((u) => u.id);
    }

    if (isAdmin && !isOwner) {
      const users = this.selection.filter(
        (x) =>
          x.status === EmployeeStatus.Disabled &&
          x.id !== id &&
          !x.isAdmin &&
          !x.isOwner
      );

      return users.map((u) => u.id);
    }

    const users = this.selection.filter(
      (x) =>
        x.status === EmployeeStatus.Disabled &&
        x.id !== id &&
        !x.isAdmin &&
        !x.isOwner &&
        x.isVisitor
    );

    return users.map((u) => u.id);
  }
}

export default SelectionStore;
