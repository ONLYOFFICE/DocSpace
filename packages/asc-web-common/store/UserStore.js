import { action, makeObservable, observable } from "mobx";
import api from "../api";

class UserStore {
  user = null;
  isLoading = false;
  isLoaded = false;
  userIsUpdate = false;

  constructor() {
    makeObservable(this, {
      user: observable,
      isLoading: observable,
      isLoaded: observable,
      userIsUpdate: observable,
      getCurrentUser: action,
      setIsLoading: action,
      setIsLoaded: action,
      setUser: action,
      setUserIsUpdate: action,
    });
  }

  getCurrentUser = async () => {
    const user = await api.people.getUser();

    this.setUser(user);
  };

  init = async () => {
    this.setIsLoading(true);

    await this.getCurrentUser();

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

  setUserIsUpdate = (isUpdate) => {
    //console.log("setUserIsUpdate");
    this.userIsUpdate = isUpdate;
  };
}

export default UserStore;
