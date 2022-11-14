export const AccountsActions = Object.freeze({
  inviteDocspaceAdmin: false,
  inviteRoomAdmin: false,
  inviteUser: false,
  raiseToDocspaceAdmin: false,
  raiseToRoomAdmin: false,
  downgradeToRoomAdmin: false,
  downgradeToUser: false,
  blockDocspaceAdmin: false,
  blockRoomAdmin: false,
  blockUser: false,
  changeDocspaceAdminData: false,
  changeRoomAdminData: false,
  changeUserData: false,
  deleteDocspaceAdmin: false,
  deleteRoomAdmin: false,
  deleteUser: false,
});

export const OwnerAccountsActions = Object.freeze({
  ...AccountsActions,
  inviteDocspaceAdmin: true,
  inviteRoomAdmin: true,
  inviteUser: true,
  raiseToDocspaceAdmin: true,
  raiseToRoomAdmin: true,
  downgradeToRoomAdmin: true,
  blockDocspaceAdmin: true,
  blockRoomAdmin: true,
  blockUser: true,
  changeDocspaceAdminData: true,
  changeRoomAdminData: true,
  changeUserData: true,
  deleteDocspaceAdmin: true,
  deleteRoomAdmin: true,
  deleteUser: true,
});

export const DocSpaceAdminAccountsActions = Object.freeze({
  ...AccountsActions,
  inviteRoomAdmin: true,
  inviteUser: true,
  raiseToRoomAdmin: true,
  blockRoomAdmin: true,
  blockUser: true,
  changeRoomAdminData: true,
  changeUserData: true,
  deleteRoomAdmin: true,
  deleteUser: true,
});

export const RoomAdminAccountsActions = Object.freeze({
  ...AccountsActions,
  inviteRoomAdmin: true,
  inviteUser: true,
  raiseToRoomAdmin: true,
});
