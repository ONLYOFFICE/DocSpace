import { makeAutoObservable } from "mobx";

import {
  EmployeeActivationStatus,
  EmployeeStatus,
  ShareAccessRights,
} from "@docspace/common/constants";

class AccessRightsStore {
  authStore = null;
  selectedFolderStore = null;

  constructor(authStore, selectedFolderStore) {
    this.authStore = authStore;
    this.selectedFolderStore = selectedFolderStore;

    makeAutoObservable(this);
  }

  canEditRoom = (room) => {
    const { isAdmin, isOwner } = this.authStore.userStore.user;

    if (
      isAdmin ||
      isOwner ||
      ShareAccessRights.None === room.access ||
      ShareAccessRights.RoomManager === room.access ||
      ShareAccessRights.FullAccess === room.access
    )
      return true;

    return false;
  };

  canInviteUserInRoom = (room) => {
    const { isAdmin, isOwner } = this.authStore.userStore.user;

    if (
      isAdmin ||
      isOwner ||
      ShareAccessRights.None === room.access ||
      ShareAccessRights.RoomManager === room.access ||
      ShareAccessRights.FullAccess === room.access
    )
      return true;

    return false;
  };

  get canChangeUserRoleInRoom() {}

  canRemoveUserFromRoom = (room) => {
    const { isAdmin, isOwner } = this.authStore.userStore.user;

    if (
      isAdmin ||
      isOwner ||
      ShareAccessRights.None === room.access ||
      ShareAccessRights.RoomManager === room.access ||
      ShareAccessRights.FullAccess === room.access
    )
      return true;

    return false;
  };

  canArchiveRoom = (room) => {
    const { isAdmin, isOwner } = this.authStore.userStore.user;

    if (
      isAdmin ||
      isOwner ||
      ShareAccessRights.None === room.access ||
      ShareAccessRights.None === room.access ||
      ShareAccessRights.FullAccess === room.access
    )
      return true;

    return false;
  };

  canRemoveRoom = (room) => {
    const { isAdmin, isOwner } = this.authStore.userStore.user;

    if (
      isAdmin ||
      isOwner ||
      ShareAccessRights.None === room.access ||
      ShareAccessRights.None === room.access ||
      ShareAccessRights.FullAccess === room.access
    )
      return true;

    return false;
  };

  get canCreateFiles() {
    const { isAdmin, isOwner } = this.authStore.userStore.user;

    const { access } = this.selectedFolderStore;

    if (
      access === ShareAccessRights.None ||
      access === ShareAccessRights.FullAccess ||
      access === ShareAccessRights.RoomManager ||
      isAdmin ||
      isOwner
    )
      return true;

    return false;
  }

  canDownloadFiles = (room) => {
    const { isAdmin, isOwner } = this.authStore.userStore.user;

    if (
      isAdmin ||
      isOwner ||
      ShareAccessRights.None === room.access ||
      ShareAccessRights.FullAccess === room.access ||
      ShareAccessRights.RoomManager === room.access
    )
      return true;

    return false;
  };

  canEditFile = (room) => {
    const { isAdmin, isOwner } = this.authStore.userStore.user;

    if (
      isAdmin ||
      isOwner ||
      ShareAccessRights.None === room.access ||
      ShareAccessRights.FullAccess === room.access ||
      ShareAccessRights.RoomManager === room.access ||
      ShareAccessRights.Editing === room.access
    )
      return true;

    return false;
  };

  canFillForm = (room) => {
    const { isAdmin, isOwner } = this.authStore.userStore.user;

    if (
      isAdmin ||
      isOwner ||
      ShareAccessRights.None === room.access ||
      ShareAccessRights.FullAccess === room.access ||
      ShareAccessRights.RoomManager === room.access ||
      ShareAccessRights.Editing === room.access ||
      ShareAccessRights.FormFilling === room.access
    )
      return true;

    return false;
  };

  canPeerReview = (room) => {
    const { isAdmin, isOwner } = this.authStore.userStore.user;

    if (
      isAdmin ||
      isOwner ||
      ShareAccessRights.None === room.access ||
      ShareAccessRights.FullAccess === room.access ||
      ShareAccessRights.RoomManager === room.access ||
      ShareAccessRights.Editing === room.access ||
      ShareAccessRights.FormFilling === room.access ||
      ShareAccessRights.Review === room.access
    )
      return true;

    return false;
  };

  canCommentFile = (room) => {
    const { isAdmin, isOwner } = this.authStore.userStore.user;

    if (isAdmin || isOwner || ShareAccessRights.ReadOnly !== room.access)
      return true;

    return false;
  };

  canBlockFile = (room) => {
    const { isAdmin, isOwner } = this.authStore.userStore.user;

    if (
      isAdmin ||
      isOwner ||
      ShareAccessRights.None === room.access ||
      ShareAccessRights.FullAccess === room.access ||
      ShareAccessRights.RoomManager === room.access
    )
      return true;

    return false;
  };

  canShowVersionHistory = (room) => {
    const { isAdmin, isOwner } = this.authStore.userStore.user;

    if (
      isAdmin ||
      isOwner ||
      ShareAccessRights.None === room.access ||
      ShareAccessRights.FullAccess === room.access ||
      ShareAccessRights.RoomManager === room.access ||
      ShareAccessRights.Editing === room.access
    )
      return true;

    return false;
  };

  canManageVersionHistory = (room) => {
    const { isAdmin, isOwner } = this.authStore.userStore.user;

    if (
      isAdmin ||
      isOwner ||
      ShareAccessRights.None === room.access ||
      ShareAccessRights.FullAccess === room.access ||
      ShareAccessRights.RoomManager === room.access
    )
      return true;

    return false;
  };

  canMoveFile = (room) => {
    const { isAdmin, isOwner } = this.authStore.userStore.user;

    if (
      isAdmin ||
      isOwner ||
      ShareAccessRights.None === room.access ||
      ShareAccessRights.FullAccess === room.access ||
      ShareAccessRights.RoomManager === room.access
    )
      return true;

    return false;
  };

  canDeleteFile = (room) => {
    const { isAdmin, isOwner } = this.authStore.userStore.user;

    if (
      isAdmin ||
      isOwner ||
      ShareAccessRights.None === room.access ||
      ShareAccessRights.FullAccess === room.access ||
      ShareAccessRights.RoomManager === room.access
    )
      return true;

    return false;
  };

  canRenameFile = (room) => {
    const { isAdmin, isOwner } = this.authStore.userStore.user;

    if (
      isAdmin ||
      isOwner ||
      ShareAccessRights.None === room.access ||
      ShareAccessRights.FullAccess === room.access ||
      ShareAccessRights.RoomManager === room.access
    )
      return true;

    return false;
  };

  canCopyFile = (room) => {
    const { isAdmin, isOwner } = this.authStore.userStore.user;

    if (
      isAdmin ||
      isOwner ||
      ShareAccessRights.None === room.access ||
      ShareAccessRights.FullAccess === room.access ||
      ShareAccessRights.RoomManager === room.access
    )
      return true;

    return false;
  };

  canChangeUserType = (user) => {
    const { id, isOwner } = this.authStore.userStore.user;

    const { id: userId, statusType, role } = user;

    if (userId === id || statusType === EmployeeStatus.Disabled) return false;

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
    const { id, isOwner } = this.authStore.userStore.user;

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
    const { id, isOwner, isAdmin } = this.authStore.userStore.user;

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
    const { id, isOwner, isAdmin } = this.authStore.userStore.user;

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
    const { id, isOwner } = this.authStore.userStore.user;

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
    const { id, isOwner, isAdmin } = this.authStore.userStore.user;

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

export default AccessRightsStore;
