import { makeAutoObservable } from "mobx";

import {
  EmployeeActivationStatus,
  EmployeeStatus,
} from "@docspace/common/constants";

class AccessRights {
  authStore = null;

  constructor(authStore) {
    this.authStore = authStore;

    makeAutoObservable(this);
  }

  canChangeUserType = (user) => {
    const { id, isOwner } = this.authStore;

    const { id: userId, statusType, role } = user;

    if (userId === id || statusType === EmployeeStatus.Disabled) return text;

    switch (role) {
      case "owner":
        return false;

      case "admin":
      case "manager":
        if (isOwner) {
          return true;
        } else {
          return false;
        }

      case "user":
        return true;

      default:
        return false;
    }
  };

  canMakeEmployeeUser = (user) => {
    const { id, isOwner } = this.authStore;

    const {
      status,
      id: userId,
      isAdmin: userIsAdmin,
      isOwner: userIsOwner,
      isVisitor: userIsVisitor,
    } = user;

    const needMakeEmployee =
      status !== EmployeeStatus.Disabled && userId !== id;

    if (isOwner) return needMakeEmployee;

    return needMakeEmployee && !userIsAdmin && !userIsOwner && userIsVisitor;
  };

  canActivateUser = (user) => {
    const { id, isOwner, isAdmin } = this.authStore;

    const {
      status,
      id: userId,
      isAdmin: userIsAdmin,
      isOwner: userIsOwner,
    } = user;

    const needActivate = status !== EmployeeStatus.Active && userId !== id;

    if (isOwner) return needActivate;

    if (isAdmin) return needActivate && !userIsAdmin && !userIsOwner;

    return false;
  };

  canDisableUser = (user) => {
    const { id, isOwner, isAdmin } = this.authStore;

    const {
      status,
      id: userId,
      isAdmin: userIsAdmin,
      isOwner: userIsOwner,
    } = user;

    const needDisable = status !== EmployeeStatus.Disabled && userId !== id;

    if (isOwner) return needDisable;

    if (isAdmin) return needDisable && !userIsAdmin && !userIsOwner;

    return false;
  };

  canInviteUser = (user) => {
    const { id, isOwner } = this.authStore;

    const {
      activationStatus,
      status,
      id: userId,
      isAdmin: userIsAdmin,
      isOwner: userIsOwner,
    } = user;

    const needInvite =
      activationStatus === EmployeeActivationStatus.Pending &&
      status === EmployeeStatus.Active &&
      userId !== id;

    if (isOwner) return needInvite;

    return needInvite && !userIsAdmin && !userIsOwner;
  };

  canRemoveUser = (user) => {
    const { id, isOwner, isAdmin } = this.authStore;

    const {
      status,
      id: userId,
      isAdmin: userIsAdmin,
      isOwner: userIsOwner,
    } = user;

    const needRemove = status === EmployeeStatus.Disabled && userId !== id;

    if (isOwner) return needRemove;

    if (isAdmin) return needRemove && !userIsAdmin && !userIsOwner;

    return false;
  };
}

export default AccessRights;
