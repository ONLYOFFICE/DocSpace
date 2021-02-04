import { action, makeObservable, observable } from "mobx";
import { api } from "asc-web-common";

class GroupsStore {
  groups = null;

  constructor() {
    makeObservable(this, {
      groups: observable,
      getGroupList: action,
    });
  }

  getGroupList = async () => {
    const res = await api.groups.getGroupList();
    this.groups = res;
  };
}

export default GroupsStore;
