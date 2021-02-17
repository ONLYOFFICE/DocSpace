import { api, store } from "asc-web-common";
import { action, computed, makeObservable, observable } from "mobx";

const { authStore } = store;

class TargetUserStore {
  targetUser = null;

  constructor(peopleStore) {
    this.peopleStore = peopleStore;
    makeObservable(this, {
      targetUser: observable,
      getTargetUser: action,
      setTargetUser: action,
      resetTargetUser: action,
      updateProfile: action,
      updateCreatedAvatar: action,
      updateProfileCulture: action,
      getUserPhoto: action,
      getDisableProfileType: computed,
    });
  }

  get getDisableProfileType() {
    const res =
      authStore.userStore.user.id === this.targetUser.id ||
      !authStore.isAdmin ||
      this.peopleStore.isPeoplesAdmin
        ? false
        : true;

    return res;
  }

  getTargetUser = async (userName) => {
    if (authStore.userStore.user.userName === userName) {
      return (this.targetUser = authStore.userStore.user);
    } else {
      const user = await api.people.getUser(userName);
      return (this.targetUser = user);
    }
  };

  setTargetUser = (user) => {
    return (this.targetUser = user);
  };

  resetTargetUser = () => {
    this.targetUser = null;
  };

  updateProfile = async (profile) => {
    const member = this.peopleStore.usersStore.employeeWrapperToMemberModel(
      profile
    );
    let result;
    const res = await api.people.updateUser(member);
    result = res;

    this.setTargetUser(res);
    return Promise.resolve(result);
  };

  updateCreatedAvatar = (avatar) => {
    const { big, max, medium, small } = avatar;
    this.targetUser.avatarMax = max;
    this.targetUser.avatarMedium = medium;
    this.targetUser.avatar = big;
    this.targetUser.avatarSmall = small;
  };

  updateProfileCulture = async (id, culture) => {
    const res = await api.people.updateUserCulture(id, culture);
    authStore.userStore.setUser(res);
    this.setTargetUser(res);
    caches.delete("api-cache");
    await authStore.settingsStore.init();
  };

  getUserPhoto = async (id) => {
    const res = await api.people.getUserPhoto(id);
    return res;
  };
}

export default TargetUserStore;
