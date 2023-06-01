import React from "react";
import { toastr } from "@docspace/components";
import { makeAutoObservable, runInAction } from "mobx";
import { Trans } from "react-i18next";
import api from "../api";
import { EmployeeActivationStatus } from "../constants";

class UserStore {
  user = null;
  isLoading = false;
  isLoaded = false;
  userIsUpdate = false;

  withSendAgain = true;

  constructor() {
    makeAutoObservable(this);
  }

  loadCurrentUser = async () => {
    let user = null;
    if (window?.__ASC_INITIAL_EDITOR_STATE__?.user)
      user = window.__ASC_INITIAL_EDITOR_STATE__.user;
    else user = await api.people.getUser();

    this.setUser(user);

    return user;
  };

  init = async () => {
    if (this.isLoaded) return;

    this.setIsLoading(true);

    try {
      await this.loadCurrentUser();
    } catch (e) {
      console.error(e);
    }

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

  updateEmailActivationStatus = async (activationStatus, userId, key) => {
    this.setIsLoading(true);

    const user = await api.people.updateActivationStatus(
      activationStatus,
      userId,
      key
    );

    this.setUser(user);
    this.setIsLoading(false);
  };

  changeTheme = async (key) => {
    this.setIsLoading(true);

    const { theme } = await api.people.changeTheme(key);

    runInAction(() => {
      this.user.theme = theme;
    })

    this.setIsLoading(false);

    return theme;
  };

  setUserIsUpdate = (isUpdate) => {
    //console.log("setUserIsUpdate");
    this.userIsUpdate = isUpdate;
  };

  setWithSendAgain = (withSendAgain) => {
    this.withSendAgain = withSendAgain;
  };

  sendActivationLink = async () => {
    const { email, id } = this.user;
    await api.people.resendUserInvites([id]);
    return email;
  };

  updateAvatarInfo = (avatar, avatarSmall, avatarMedium, avatarMax) => {
    this.user.avatar = avatar;
    this.user.avatarSmall = avatarSmall;
    this.user.avatarMedium = avatarMedium;
    this.user.avatarMax = avatarMax;
  };

  get withActivationBar() {
    return (
      this.user &&
      (this.user.activationStatus === EmployeeActivationStatus.Pending ||
        this.user.activationStatus === EmployeeActivationStatus.NotActivated) &&
      this.withSendAgain
    );
  }

  get isAuthenticated() {
    return !!this.user;
  }
}

export default UserStore;
