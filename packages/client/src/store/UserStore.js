import { makeAutoObservable } from "mobx";
import api from "@docspace/common/api";

class UserStore {
  user = null;
  isLoading = false;
  isLoaded = false;
  userIsUpdate = false;

  constructor() {
    makeAutoObservable(this);
  }

  loadCurrentUser = async () => {
    let user = null;
    if (window?.__ASC_INITIAL_EDITOR_STATE__?.user)
      user = window.__ASC_INITIAL_EDITOR_STATE__.user;
    else user = await api.people.getUser();

    this.setUser(user);
  };

  init = async () => {
    if (this.isLoaded) return;

    this.setIsLoading(true);

    await this.loadCurrentUser();

    this.setIsLoading(false);
    this.setIsLoaded(true);
  };

  setIsLoading = (isLoading) => {
    this.isLoading = isLoading;
  };

  setIsLoaded = (isLoaded) => {
    this.isLoaded = isLoaded;
  };

  setUser = (user) => {
    this.user = user;
  };

  changeEmail = async (userId, email, key) => {
    this.setIsLoading(true);

    const user = await api.people.changeEmail(userId, email, key);

    this.setUser(user);
    this.setIsLoading(false);
  };

  changeTheme = async (key) => {
    this.setIsLoading(true);

    const { theme } = await api.people.changeTheme(key);

    this.user.theme = theme;

    this.setIsLoading(false);

    return theme;
  };

  setUserIsUpdate = (isUpdate) => {
    //console.log("setUserIsUpdate");
    this.userIsUpdate = isUpdate;
  };

  get isAuthenticated() {
    return !!this.user;
  }
}

export default UserStore;
