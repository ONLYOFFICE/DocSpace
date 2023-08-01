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
    //console.log("setSelection", { selection });
    this.selection = selection;
  };

  setSelections = (added, removed, clear = false) => {
    if (clear) {
      this.selection = [];
    }

    let newSelections = JSON.parse(JSON.stringify(this.selection));

    for (let item of added) {
      if (!item) return;

      const value = item.getElementsByClassName("user-item")
        ? item.getElementsByClassName("user-item")[0]?.getAttribute("value")
        : null;

      if (!value) return;
      const splitValue = value && value.split("_");
      const id = splitValue.slice(1, -3).join("_");

      const isFound = this.selection.findIndex((f) => f.id == id) === -1;

      isFound &&
        newSelections.push(
          this.peopleStore.usersStore.peopleList.find((f) => f.id == id)
        );
    }

    for (let item of removed) {
      if (!item) return;

      const value = item.getElementsByClassName("user-item")
        ? item.getElementsByClassName("user-item")[0]?.getAttribute("value")
        : null;

      const splitValue = value && value.split("_");
      const id = splitValue.slice(1, -3).join("_");

      newSelections = newSelections.filter((f) => !(f.id == id));
    }

    this.setSelection(newSelections);
  };

  setBufferSelection = (bufferSelection, addToSelection = true) => {
    this.bufferSelection = bufferSelection;
    //console.log("setBufferSelection", { bufferSelection });
    bufferSelection
      ? addToSelection && this.setSelection([bufferSelection])
      : this.clearSelection();
  };

  selectUser = (user) => {
    const index = this.selection.findIndex((el) => el.id === user.id);

    const exists = index > -1;

    //console.log("selectUser", { user, selection: this.selection, exists });

    if (exists) return;

    this.setSelection([...this.selection, user]);
  };

  deselectUser = (user) => {
    const index = this.selection.findIndex((el) => el.id === user.id);

    const exists = index > -1;

    //console.log("deselectUser", { user, selection: this.selection, exists });

    if (!exists) return;

    const newData = [...this.selection];

    newData.splice(index, 1);

    this.setSelection(newData);
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

    this.setSelection(list);
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

    return users.map((u) => u.id);
  }
}

export default SelectionStore;
