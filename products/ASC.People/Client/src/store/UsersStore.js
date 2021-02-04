import { action, makeObservable, observable } from "mobx";
import { getFilterByLocation } from "../helpers/converters";
import { api } from "asc-web-common";

const { Filter } = api;

class UsersStore {
  users = null;

  constructor() {
    makeObservable(this, {
      users: observable,
      getUsersList: action,
    });
  }

  getUsersList = async (filter) => {
    let filterData = filter && filter.clone();
    if (!filterData) {
      filterData = Filter.getDefault();
    }
    const res = await api.people.getListAdmins(filterData);
    this.users = res;
  };
}

export default UsersStore;
