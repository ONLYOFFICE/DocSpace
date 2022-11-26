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
  getArchiveRoomRoleActions,
  getArchiveFileRoleActions,
} from "@docspace/common/utils/actions";

class AccessRightsStore {
  authStore = null;
  selectedFolderStore = null;
  treeFoldersStore = null;

  constructor(authStore, selectedFolderStore) {
    this.authStore = authStore;
    this.selectedFolderStore = selectedFolderStore;

    makeAutoObservable(this);
  }

  canInviteUserInRoom(room) {
    const { access, rootFolderType } = room;

    if (rootFolderType === FolderType.Archive)
      return getArchiveRoomRoleActions(access).inviteUsers;

    return getRoomRoleActions(access).inviteUsers;
  }

  canChangeUserRoleInRoom = (room) => {
    const { access, rootFolderType, currentUserInList } = room;
    const { userStore } = this.authStore;
    const { user } = userStore;

    if (rootFolderType === FolderType.Archive)
      return getArchiveRoomRoleActions(access).changeUserRole;

    const isMyProfile = user.id === currentUserInList.id;
    const isOwnerRoleRoom =
      currentUserInList.access === ShareAccessRights.FullAccess;

    if (isMyProfile || isOwnerRoleRoom) return false;

    return getRoomRoleActions(access).changeUserRole;
  };

  canLockFile = (file) => {
    const { rootFolderType, access } = file;

    if (rootFolderType === FolderType.Archive)
      return getArchiveFileRoleActions(access).block;

    if (rootFolderType === FolderType.TRASH) return false;

    return getFileRoleActions(access).block;
  };

  canChangeVersionFileHistory = (file) => {
    const { rootFolderType, editing, providerKey, access } = file;

    if (rootFolderType === FolderType.Archive)
      return getArchiveFileRoleActions(access).changeVersionHistory;

    if (
      rootFolderType === FolderType.TRASH ||
      // rootFolderType === FolderType.Privacy ||
      editing ||
      providerKey
    )
      return false;

    return getFileRoleActions(access).changeVersionHistory;
  };
  canViewVersionFileHistory = (file) => {
    const { rootFolderType, access, providerKey } = file;

    if (rootFolderType === FolderType.Archive)
      return getArchiveFileRoleActions(access).viewVersionHistory;

    if (
      rootFolderType === FolderType.TRASH ||
      // rootFolderType === FolderType.Privacy ||
      providerKey
    )
      return false;

    return getFileRoleActions(access).viewVersionHistory;
  };

  canEditFile = (file) => {
    const { rootFolderType, access } = file;

    if (rootFolderType === FolderType.Archive)
      return getArchiveFileRoleActions(access).edit;

    if (
      rootFolderType === FolderType.TRASH
      // || rootFolderType === FolderType.Privacy
    )
      return false;

    return getFileRoleActions(access).edit;
  };

  canRenameItem = (item = {}) => {
    const { rootFolderType, access, isFile } = item;
    const { isDesktopClient } = this.authStore.settingsStore;

    if (rootFolderType === FolderType.Archive)
      return getArchiveFileRoleActions(access).rename;

    if (
      rootFolderType === FolderType.TRASH ||
      (!isFile &&
        // rootFolderType === FolderType.Privacy &&
        !isDesktopClient)
    )
      return false;

    return getFileRoleActions(access).rename;
  };
  canFillForm = (file) => {
    const { rootFolderType, access } = file;

    if (rootFolderType === FolderType.Archive)
      return getArchiveFileRoleActions(access).fillForm;

    if (rootFolderType === FolderType.TRASH) return false;

    return getFileRoleActions(access).fillForm;
  };

  canMakeForm = (item) => {
    const { rootFolderType, access } = item;

    if (rootFolderType === FolderType.Archive)
      return getArchiveFileRoleActions(access).saveAsForm;

    if (
      rootFolderType === FolderType.TRASH ||
      // rootFolderType === FolderType.Privacy ||
      rootFolderType === FolderType.Favorites ||
      rootFolderType === FolderType.Recent
    )
      return false;

    return getFileRoleActions(access).saveAsForm;
  };

  canArchiveRoom = (room) => {
    const { archive } = getRoomRoleActions(room.access);

    return archive;
  };

  canRemoveRoom = (room) => {
    const { access, rootFolderType } = room;

    if (rootFolderType !== FolderType.Archive)
      return getRoomRoleActions(access).delete;

    return getArchiveRoomRoleActions(access).delete;
  };

  canViewRoomInfo = (room) => {
    const { access, rootFolderType } = room;

    if (rootFolderType === FolderType.Archive)
      return getArchiveRoomRoleActions(access).viewInfo;

    return getRoomRoleActions(access).viewInfo;
  };

  canPinRoom = (room) => {
    const { access, rootFolderType } = room;

    if (rootFolderType === FolderType.Archive)
      return getArchiveRoomRoleActions(access).canPin;

    return getRoomRoleActions(access).canPin;
  };

  canEditRoom = (room) => {
    const { access, rootFolderType } = room;

    if (rootFolderType === FolderType.Archive)
      return getArchiveRoomRoleActions(access).edit;

    return getRoomRoleActions(access).edit;
  };

  get canCreateFiles() {
    const {
      access,
      rootFolderType,
      private: isPrivateFolder,
    } = this.selectedFolderStore;

    const {
      isDesktopClient,
      isEncryptionSupport,
    } = this.authStore.settingsStore;

    if (isPrivateFolder && (!isDesktopClient || !isEncryptionSupport))
      return false;

    if (rootFolderType === FolderType.Archive)
      return getArchiveFileRoleActions(access).create;

    const { create } = getFileRoleActions(access);

    return create;
  }

  canMoveItems = (item) => {
    const { rootFolderType, access, editing: fileEditing, providerKey } = item;

    if (rootFolderType === FolderType.Archive) {
      const { moveSelf, moveAlien } = getArchiveFileRoleActions(access);

      return moveSelf || moveAlien;
    }

    if (
      rootFolderType === FolderType.TRASH ||
      rootFolderType === FolderType.Favorites ||
      rootFolderType === FolderType.Recent ||
      // rootFolderType === FolderType.Privacy ||
      providerKey ||
      fileEditing
    )
      return false;

    const { moveSelf, moveAlien } = getFileRoleActions(access);

    return moveSelf || moveAlien;
  };

  canDeleteItems = (item) => {
    const { rootFolderType, access, editing: fileEditing } = item;

    if (rootFolderType === FolderType.Archive) {
      const { deleteSelf, deleteAlien } = getArchiveFileRoleActions(access);

      return deleteSelf || deleteAlien;
    }

    if (
      rootFolderType === FolderType.TRASH ||
      rootFolderType === FolderType.Favorites ||
      rootFolderType === FolderType.Recent ||
      // rootFolderType === FolderType.Privacy ||
      fileEditing
    )
      return false;

    const { deleteSelf, deleteAlien } = getFileRoleActions(access);

    return deleteSelf || deleteAlien;
  };

  canCopyItems = (item) => {
    const { rootFolderType, access } = item;

    if (
      rootFolderType === FolderType.TRASH ||
      rootFolderType === FolderType.Favorites ||
      rootFolderType === FolderType.Recent
      // || rootFolderType === FolderType.Privacy
    )
      return false;

    const { canCopy } = getFileRoleActions(access);

    return canCopy;
  };
  canDuplicateFile = (item) => {
    const { rootFolderType, access } = item;

    if (rootFolderType === FolderType.Archive)
      return getArchiveFileRoleActions(access).canDuplicate;

    if (
      rootFolderType === FolderType.TRASH ||
      rootFolderType === FolderType.Favorites ||
      rootFolderType === FolderType.Recent
      // || rootFolderType === FolderType.Privacy
    )
      return false;

    return getFileRoleActions(access).canDuplicate;
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

  canViewUsers = (room) => {
    const { rootFolderType } = this.selectedFolderStore;

    if (!room) return false;

    const options =
      rootFolderType === FolderType.Archive
        ? getArchiveRoomRoleActions(room.access)
        : getRoomRoleActions(room.access);

    return options.viewUsers;
  };
}

export default AccessRightsStore;
