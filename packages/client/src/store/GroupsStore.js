import { action, makeObservable, observable } from "mobx";
import api from "@docspace/common/api";

class GroupsStore {
  groups = [];

  constructor(peopleStore) {
    this.peopleStore = peopleStore;
    makeObservable(this, {
      groups: observable,
      getGroupList: action,
      deleteGroup: action,
      createGroup: action,
      updateGroup: action,
    });
  }

  getGroupList = async () => {
    const res = await api.groups.getGroupList(); //TODO: replace with getGroupListFull() after fixing problems on the server
    this.groups = res;
  };

  deleteGroup = async (id) => {
    const { filter } = this.peopleStore.filterStore;
    await api.groups.deleteGroup(id);
    const newData = this.groups.filter((g) => g.id !== id);
    this.groups = newData;
    await this.peopleStore.usersStore.getUsersList(filter);
  };

  createGroup = async (groupName, groupManager, members) => {
    const res = await api.groups.createGroup(groupName, groupManager, members);
    this.peopleStore.selectedGroupStore.resetGroup();
    this.groups.push(res);
    this.groups.sort((a, b) => a.name.localeCompare(b.name));
    return Promise.resolve(res);
  };

  updateGroup = async (id, groupName, groupManager, members) => {
    const res = await api.groups.updateGroup(
      id,
      groupName,
      groupManager,
      members
    );
    this.peopleStore.selectedGroupStore.resetGroup();
    await this.getGroupList();
    return Promise.resolve(res);
  };
}

export default GroupsStore;
