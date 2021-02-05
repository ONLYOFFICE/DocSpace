import { api } from "asc-web-common";
import { action, makeObservable, observable } from "mobx";
// import { getFilterByLocation } from "../helpers/converters";
// import { api } from "asc-web-common";

// const { Filter } = api;

class TargetUserStore {
  targetUser = null;

  constructor() {
    makeObservable(this, {
      targetUser: observable,
      getTargetUser: action,
    });
  }

  getTargetUser = async (userName) => {
    const user = await api.people.getUser(userName);
    this.targetUser = user;
  };
}

export default TargetUserStore;
