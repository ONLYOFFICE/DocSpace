import { action, makeObservable, observable } from "mobx";
import { api } from "asc-web-common";

class GroupsStore {
  groups = null;

  constructor(peopleStore) {
    this.peopleStore = peopleStore;
    makeObservable(this, {
      groups: observable,
      getGroupList: action,
    });
  }

  getGroupList = async () => {
    const res = await api.groups.getGroupList();
    this.groups = res;
  };

  deleteGroup = async (id) => {
    const { filter } = this.peopleStore.filterStore;
    await api.groups.deleteGroup(id);
    const newData = this.groups.filter((g) => g.id !== id);
    this.groups = newData;
    await this.peopleStore.usersStore.getUsersList(filter);
  };
}

export default GroupsStore;
