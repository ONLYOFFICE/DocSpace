import { action, makeObservable, observable } from "mobx";
import GroupsStore from "./GroupsStore";
import UsersStore from "./UsersStore";
import { getFilterByLocation } from "../helpers/converters";
import config from "../../package.json";
import TargetUserStore from "./TargetUserStore";
import SelectedGroupStore from "./SelectedGroupStore";
import EditingFormStore from "./EditingFormStore";

class PeopleStore {
  groupsStore = null;
  usersStore = null;
  targetUserStore = null;
  selectedGroupStore = null;
  editingFormStore = null;

  isLoading = false;
  //isAuthenticated = false;

  constructor() {
    this.setGroupsStore(new GroupsStore());
    this.setUsersStore(new UsersStore());
    this.setTargetUserStore(new TargetUserStore());
    this.setSelectedGroupStore(new SelectedGroupStore());
    this.setEditingFormStore(new EditingFormStore());

    makeObservable(this, {
      isLoading: observable,
      setGroupsStore: action,
      setUsersStore: action,
      setTargetUserStore: action,
      setSelectedGroupStore: action,
      setEditingFormStore: action,
      init: action,
    });
  }

  init = async () => {
    const re = new RegExp(`${config.homepage}((/?)$|/filter)`, "gm");
    const match = window.location.pathname.match(re);

    if (match && match.length > 0) {
      const newFilter = getFilterByLocation(window.location);
      await this.usersStore.getUsersList(newFilter);
    }
    await this.groupsStore.getGroupList();
    //await this.usersStore.getUsersList();
  };

  setGroupsStore = (store) => {
    this.groupsStore = store;
  };
  setUsersStore = (store) => {
    this.usersStore = store;
  };
  setTargetUserStore = (store) => {
    this.targetUserStore = store;
  };
  setSelectedGroupStore = (store) => {
    this.selectedGroupStore = store;
  };
  setEditingFormStore = (store) => {
    this.editingFormStore = store;
  };
}

export default new PeopleStore();
