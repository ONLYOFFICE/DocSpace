import { api, store } from "asc-web-common";
import { action, makeObservable, observable } from "mobx";

const { authStore } = store;

class TargetUserStore {
  targetUser = null;

  constructor() {
    makeObservable(this, {
      targetUser: observable,
      getTargetUser: action,
      resetTargetUser: action,
    });
  }

  getTargetUser = async (userName) => {
    if (authStore.userStore.user.userName === userName) {
      return (this.targetUser = authStore.userStore.user);
    } else {
      const user = await api.people.getUser(userName);
      return (this.targetUser = user);
    }
  };

  resetTargetUser = () => {
    this.targetUser = null;
  };
}

export default TargetUserStore;
