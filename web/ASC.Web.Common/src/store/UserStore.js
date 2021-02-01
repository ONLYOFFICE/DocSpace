import { makeAutoObservable } from "mobx";
import api from "../api";
import { LANGUAGE } from "../constants";

class UserStore {
  user = null;
  isAuthenticated = false;

  constructor() {
    makeAutoObservable(this);
  }

  async setCurrentUser() {
    const user = await api.people.getUser();
    localStorage.getItem(LANGUAGE) !== user.cultureName &&
      localStorage.setItem(LANGUAGE, user.cultureName);
    this.user = user;
    this.isAuthenticated = true;
  }
}

export default UserStore;
