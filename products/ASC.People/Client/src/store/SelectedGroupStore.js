import { action, computed, makeObservable, observable } from "mobx";
import { api } from "asc-web-common";

class SelectedGroupStore {
  selectedGroup = null;
  targetedGroup = null;

  constructor(peopleStore) {
    this.peopleStore = peopleStore;
    makeObservable(this, {
      selectedGroup: observable,
      targetedGroup: observable,
      setSelectedGroup: action,
      setTargetedGroup: action,
      resetGroup: action,
      selectGroup: action,
      group: computed,
      isEmptyGroup: computed,
    });
  }

  selectGroup = (groupId) => {
    const { filter } = this.peopleStore.filterStore;
    const { clearSelection } = this.peopleStore.selectionStore;
    const { getUsersList } = this.peopleStore.usersStore;
    let newFilter = filter.clone();

    newFilter.group = groupId;
    clearSelection();
    getUsersList(newFilter);
  };

  setSelectedGroup = (groupId) => {
    this.selectedGroup = groupId;
  };

  setTargetedGroup = async (groupId) => {
    const res = await api.groups.getGroup(groupId);
    this.targetedGroup = res;
  };

  resetGroup = () => {
    return (this.targetedGroup = null);
  };

  get group() {
    const { groups } = this.peopleStore.groupsStore;
    return groups.find((g) => g.id === this.selectedGroup);
  }

  get isEmptyGroup() {
    const { groups } = this.peopleStore.groupsStore;
    const { filter } = this.peopleStore.filterStore;

    const { group, search, role, activationStatus, employeeStatus } = filter;

    let countMembers;
    groups.filter((el) => {
      if (el.id === group) countMembers = el.members.length;
    });

    const filterIsClear =
      !search && !role && !activationStatus && !employeeStatus;

    if (countMembers === 0 && filterIsClear && group) return true;
    return false;
  }
}

export default SelectedGroupStore;
