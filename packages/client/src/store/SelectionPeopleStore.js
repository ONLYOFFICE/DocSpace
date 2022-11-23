import { makeAutoObservable } from "mobx";
import { EmployeeStatus } from "@docspace/common/constants";
import { getUserStatus } from "../helpers/people-helpers";

class SelectionStore {
  peopleStore = null;
  selection = [];
  bufferSelection = null;
  selected = "none";

  constructor(peopleStore) {
    this.peopleStore = peopleStore;

    makeAutoObservable(this);
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
    const { canMakeEmployeeUser } = this.peopleStore.accessRightsStore;

    const users = this.selection.filter((x) => canMakeEmployeeUser(x));

    return users.length > 0;
  }

  get getUsersToMakeEmployees() {
    const { canMakeEmployeeUser } = this.peopleStore.accessRightsStore;

    const users = this.selection.filter((x) => canMakeEmployeeUser(x));

    return users.map((u) => u);
  }

  get hasFreeUsers() {
    const users = this.selection.filter(
      (x) => x.status !== EmployeeStatus.Disabled && x.isVisitor
    );

    return users.length > 0;
  }

  get hasUsersToActivate() {
    const { canActivateUser } = this.peopleStore.accessRightsStore;

    const users = this.selection.filter((x) => canActivateUser(x));

    return users.length > 0;
  }

  get getUsersToActivate() {
    const { canActivateUser } = this.peopleStore.accessRightsStore;

    const users = this.selection.filter((x) => canActivateUser(x));

    return users.map((u) => u);
  }

  get hasUsersToDisable() {
    const { canDisableUser } = this.peopleStore.accessRightsStore;

    const users = this.selection.filter((x) => canDisableUser(x));

    return users.length > 0;
  }

  get getUsersToDisable() {
    const { canDisableUser } = this.peopleStore.accessRightsStore;

    const users = this.selection.filter((x) => canDisableUser(x));

    return users.map((u) => u);
  }

  get hasUsersToInvite() {
    const { canInviteUser } = this.peopleStore.accessRightsStore;

    const users = this.selection.filter((x) => canInviteUser(x));

    return users.length > 0;
  }

  get getUsersToInviteIds() {
    const { canInviteUser } = this.peopleStore.accessRightsStore;

    const users = this.selection.filter((x) => canInviteUser(x));

    return users.length > 0 ? users.map((u) => u.id) : [];
  }

  get hasUsersToRemove() {
    const { canRemoveUser } = this.peopleStore.accessRightsStore;

    const users = this.selection.filter((x) => canRemoveUser(x));

    return users.length > 0;
  }

  get getUsersToRemoveIds() {
    const { canRemoveUser } = this.peopleStore.accessRightsStore;

    const users = this.selection.filter((x) => canRemoveUser(x));

    return users.length > 0 ? users : false;
  }
}

export default SelectionStore;
