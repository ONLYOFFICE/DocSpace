import api from "@docspace/common/api";
import { LANGUAGE, COOKIE_EXPIRATION_YEAR } from "@docspace/common/constants";
import { makeAutoObservable } from "mobx";
import { setCookie } from "@docspace/common/utils";

class TargetUserStore {
  peopleStore = null;
  targetUser = null;
  isEditTargetUser = false;
  tipsSubscription = null;
  changeEmailVisible = false;
  changePasswordVisible = false;
  changeNameVisible = false;
  changeAvatarVisible = false;

  constructor(peopleStore) {
    this.peopleStore = peopleStore;
    makeAutoObservable(this);
  }

  get getDisableProfileType() {
    const res =
      this.peopleStore.authStore.userStore.user.id === this.targetUser.id ||
      !this.peopleStore.authStore.isAdmin ||
      this.peopleStore.isPeoplesAdmin
        ? false
        : true;

    return res;
  }

  get isMe() {
    return (
      this.targetUser &&
      this.targetUser.userName ===
        this.peopleStore.authStore.userStore.user.userName
    );
  }

  getTargetUser = async (userName) => {
    /*if (this.peopleStore.authStore.userStore.user.userName === userName) {
      return this.setTargetUser(this.peopleStore.authStore.userStore.user);
    } else {*/
    const user = await api.people.getUser(userName);
    if (user?.userName === this.peopleStore.authStore.userStore.user.userName) {
      const tipsSubscription = await api.settings.getTipsSubscription();
      this.tipsSubscription = tipsSubscription;
    }
    this.setTargetUser(user);
    return user;
    //}
  };

  setTargetUser = (user) => {
    this.targetUser = user;
    this.peopleStore.authStore.userStore.setUser(user); //TODO
  };

  updateProfile = async (profile) => {
    const member = this.peopleStore.usersStore.employeeWrapperToMemberModel(
      profile
    );

    const res = await api.people.updateUser(member);
    if (!res.theme) res.theme = this.peopleStore.authStore.userStore.user.theme;

    this.setTargetUser(res);
    return Promise.resolve(res);
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

    this.peopleStore.authStore.userStore.setUser(res);

    this.setTargetUser(res);
    //caches.delete("api-cache");
    //await this.peopleStore.authStore.settingsStore.init();
    setCookie(LANGUAGE, culture, {
      "max-age": COOKIE_EXPIRATION_YEAR,
    });
  };

  getUserPhoto = async (id) => {
    const res = await api.people.getUserPhoto(id);
    return res;
  };

  setIsEditTargetUser = (isEditTargetUser) => {
    this.isEditTargetUser = isEditTargetUser;
  };

  changeEmailSubscription = async (enabled) => {
    this.tipsSubscription = enabled;
    this.tipsSubscription = await api.settings.toggleTipsSubscription();
  };

  setChangeEmailVisible = (visible) => (this.changeEmailVisible = visible);

  setChangePasswordVisible = (visible) =>
    (this.changePasswordVisible = visible);

  setChangeNameVisible = (visible) => (this.changeNameVisible = visible);

  setChangeAvatarVisible = (visible) => (this.changeAvatarVisible = visible);
}

export default TargetUserStore;
