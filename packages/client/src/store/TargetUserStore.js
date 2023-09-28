import api from "@docspace/common/api";
import {
  LANGUAGE,
  COOKIE_EXPIRATION_YEAR,
  NotificationsType,
} from "@docspace/common/constants";
import { makeAutoObservable } from "mobx";
import { setCookie } from "@docspace/common/utils";
import {
  changeNotificationSubscription,
  getNotificationSubscription,
} from "@docspace/common/api/settings";
import toastr from "@docspace/components/toast/toastr";
const { Badges, RoomsActivity, DailyFeed, UsefulTips } = NotificationsType;
class TargetUserStore {
  peopleStore = null;
  targetUser = null;
  isEditTargetUser = false;

  changeEmailVisible = false;
  changePasswordVisible = false;
  changeNameVisible = false;
  changeAvatarVisible = false;

  badgesSubscription = false;
  roomsActivitySubscription = false;
  dailyFeedSubscriptions = false;
  usefulTipsSubscription = false;

  isFirstSubscriptionsLoad = true;

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

  setHasAvatar = (value) => {
    this.targetUser.hasAvatar = value;
    this.peopleStore.authStore.userStore.user.hasAvatar = value;
  };

  getTargetUser = async (userName) => {
    /*if (this.peopleStore.authStore.userStore.user.userName === userName) {
      return this.setTargetUser(this.peopleStore.authStore.userStore.user);
    } else {*/
    const user = await api.people.getUser(userName);

    this.setTargetUser(user);
    return user;
    //}
  };

  setTargetUser = (user) => {
    this.targetUser = user;
    this.peopleStore.authStore.userStore.setUser(user); //TODO
  };

  updateProfile = async (profile) => {
    const member =
      this.peopleStore.usersStore.employeeWrapperToMemberModel(profile);

    const res = await api.people.updateUser(member);
    if (!res.theme) res.theme = this.peopleStore.authStore.userStore.user.theme;

    this.setTargetUser(res);
    return Promise.resolve(res);
  };

  updateCreatedAvatar = (avatar) => {
    const { big, small, medium, max } = avatar;

    this.targetUser.avatar = big;
    this.targetUser.avatarSmall = small;
    this.targetUser.avatarMedium = medium;
    this.targetUser.avatarMax = max;

    this.peopleStore.authStore.userStore.updateAvatarInfo(
      big,
      small,
      medium,
      max
    );

    console.log("updateCreatedAvatar", {
      targetUser: this.targetUser,
      user: this.peopleStore.authStore.userStore.user,
    });
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

  setChangeEmailVisible = (visible) => {
    //console.log("setChangeEmailVisible", { visible });
    this.changeEmailVisible = visible;
  };

  setChangePasswordVisible = (visible) =>
    (this.changePasswordVisible = visible);

  setChangeNameVisible = (visible) => {
    //console.log("setChangeNameVisible", { visible });
    this.changeNameVisible = visible;
  };

  setChangeAvatarVisible = (visible) => {
    //console.log("setChangeAvatarVisible", { visible });
    this.changeAvatarVisible = visible;
  };

  setSubscriptions = (
    isEnableBadges,
    isEnableRoomsActivity,
    isEnableDailyFeed,
    isEnableTips
  ) => {
    this.badgesSubscription = isEnableBadges;
    this.roomsActivitySubscription = isEnableRoomsActivity;
    this.dailyFeedSubscriptions = isEnableDailyFeed;
    this.usefulTipsSubscription = isEnableTips;
    this.isFirstSubscriptionsLoad = false;
  };

  changeSubscription = async (notificationType, isEnabled) => {
    const setNotificationValue = (notificationType, isEnabled) => {
      switch (notificationType) {
        case Badges:
          this.badgesSubscription = isEnabled;
          break;
        case DailyFeed:
          this.dailyFeedSubscriptions = isEnabled;
          break;
        case RoomsActivity:
          this.roomsActivitySubscription = isEnabled;
          break;
        case UsefulTips:
          this.usefulTipsSubscription = isEnabled;
          break;
      }
    };

    setNotificationValue(notificationType, isEnabled);

    try {
      await changeNotificationSubscription(notificationType, isEnabled);
    } catch (e) {
      toastr.error(e);
      const notification = await getNotificationSubscription(notificationType);

      setNotificationValue(notificationType, notification.isEnabled);
    }
  };
}

export default TargetUserStore;
