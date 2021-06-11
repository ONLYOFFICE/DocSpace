import api from "@appserver/common/api";
import { LANGUAGE } from "@appserver/common/constants";
import { makeAutoObservable } from "mobx";
import store from "studio/store";

const { auth: authStore } = store;

class TargetUserStore {
  targetUser = null;
  isEditTargetUser = false;

  constructor(peopleStore) {
    this.peopleStore = peopleStore;
    makeAutoObservable(this);
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

  get isMe() {
    return (
      this.targetUser &&
      this.targetUser.userName === authStore.userStore.user.userName
    );
  }

  getTargetUser = async (userName) => {
    /*if (authStore.userStore.user.userName === userName) {
      return this.setTargetUser(authStore.userStore.user);
    } else {*/
    const user = await api.people.getUser(userName);
    this.setTargetUser(user);
    return user;
    //}
  };

  setTargetUser = (user) => {
    this.targetUser = user;
  };

  resetTargetUser = () => {
    if (!this.isEditTargetUser) {
      this.targetUser = null;
    }
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
    //caches.delete("api-cache");
    //await authStore.settingsStore.init();
    localStorage.setItem(LANGUAGE, culture);
  };

  getUserPhoto = async (id) => {
    const res = await api.people.getUserPhoto(id);
    return res;
  };

  setIsEditTargetUser = (isEditTargetUser) => {
    this.isEditTargetUser = isEditTargetUser;
  };
}

export default TargetUserStore;
