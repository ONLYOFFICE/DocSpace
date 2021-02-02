import {
  action,
  /*makeAutoObservable,*/ makeObservable,
  observable,
} from "mobx";
import api from "../api";
import { LANGUAGE } from "../constants";

class UserStore {
  user = null;
  isAuthenticated = false;
  isAdmin = false;

  constructor() {
    makeObservable(this, {
      user: observable,
      isAuthenticated: observable,
      isAdmin: observable,
      getCurrentUser: action,
    });
  }

  getCurrentUser = async () => {
    const user = await api.people.getUser();
    localStorage.getItem(LANGUAGE) !== user.cultureName &&
      localStorage.setItem(LANGUAGE, user.cultureName);
    this.user = user;
    this.isAuthenticated = true;
    this.isAdmin = user.isAdmin;
  };
}

export default UserStore;
