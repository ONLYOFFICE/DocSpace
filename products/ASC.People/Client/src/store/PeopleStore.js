import { action, computed, makeObservable, observable } from "mobx";
// import api from "../api";
// import history from "../history";
// import ModuleStore from "./ModuleStore";
// import SettingsStore from "./SettingsStore";
import GroupsStore from "./GroupsStore";
import UsersStore from "./UsersStore";
import { getFilterByLocation } from "../helpers/converters";
import config from "../../package.json";
import TargetUserStore from "./TargetUserStore";

class PeopleStore {
  groupsStore = null;
  usersStore = null;
  targetUserStore = null;

  isLoading = false;
  //isAuthenticated = false;

  constructor() {
    this.setGroupsStore(new GroupsStore());
    this.setUsersStore(new UsersStore());
    this.setTargetUserStore(new TargetUserStore());

    makeObservable(this, {
      isLoading: observable,
      setGroupsStore: action,
      setUsersStore: action,
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
}

export default new PeopleStore();
