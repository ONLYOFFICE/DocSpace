import { makeAutoObservable } from "mobx";

import {
  EmployeeActivationStatus,
  EmployeeStatus,
  FolderType,
  ShareAccessRights,
} from "@docspace/common/constants";
import {
  getFileRoleActions,
  getRoomRoleActions,
} from "@docspace/common/utils/actions";

class AccessRightsStore {
  authStore = null;
  selectedFolderStore = null;

  constructor(authStore, selectedFolderStore) {
    this.authStore = authStore;
    this.selectedFolderStore = selectedFolderStore;

    makeAutoObservable(this);
  }

  canInviteUserInRoom = (room) => {
    const { rootFolderType } = this.selectedFolderStore;

    if (rootFolderType === FolderType.Archive) return false;

    const { inviteUsers } = getRoomRoleActions(room.access);

    return inviteUsers;
  };

  canArchiveRoom = (room) => {
    const { archive } = getRoomRoleActions(room.access);

    return archive;
  };

  canRemoveRoom = (room) => {
    const { delete: remove } = getRoomRoleActions(room.access);

    return remove;
  };

  get canCreateFiles() {
    const { access, rootFolderType } = this.selectedFolderStore;

    if (rootFolderType === FolderType.Archive) return false;

    const { create } = getFileRoleActions(access);

    return create;
  }

  canMoveFile = (room) => {
    const { rootFolderType } = room;

    if (rootFolderType === FolderType.Archive) return false;

    const { moveSelf, moveAlien } = getFileRoleActions(room.access);

    return moveSelf || moveAlien;
  };

  canDeleteFile = (room) => {
    const { rootFolderType } = room;

    if (rootFolderType === FolderType.Archive) return false;

    const { deleteSelf, deleteAlien } = getFileRoleActions(room.access);

    return deleteSelf || deleteAlien;
  };

  canCopyFile = (room) => {
    const { rootFolderType } = room;

    if (rootFolderType === FolderType.Archive) return false;

    const { copyFromPersonal } = getFileRoleActions(room.access);

    return copyFromPersonal;
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
