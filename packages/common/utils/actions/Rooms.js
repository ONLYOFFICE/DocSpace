export const RoomsActions = Object.freeze({
  edit: false,
  inviteUsers: false,
  changeUserRole: false,
  viewUsers: false,
  viewHistory: false,
  viewInfo: false,
  deleteUsers: false,
  archive: false,
  delete: false,
});

export const OwnerRoomsActions = Object.freeze({
  ...RoomsActions,
  edit: true,
  inviteUsers: true,
  changeUserRole: true,
  viewUsers: true,
  viewHistory: true,
  viewInfo: true,
  deleteUsers: true,
  archive: true,
  delete: true,
});

export const RoomAdminRoomsActions = Object.freeze({
  ...RoomsActions,
  edit: true,
  inviteUsers: true,
  changeUserRole: true,
  viewUsers: true,
  viewHistory: true,
  viewInfo: true,
  deleteUsers: true,
});

export const EditorRoomsActions = Object.freeze({
  ...RoomsActions,
  viewUsers: true,
  viewHistory: true,
  viewInfo: true,
});

export const FormFillerRoomsActions = Object.freeze({
  ...RoomsActions,
  viewUsers: true,
  viewHistory: true,
  viewInfo: true,
});

export const ReviewerRoomsActions = Object.freeze({
  ...RoomsActions,
  viewUsers: true,
  viewHistory: true,
  viewInfo: true,
});

export const CommentatorRoomsActions = Object.freeze({
  ...RoomsActions,
  viewUsers: true,
  viewHistory: true,
  viewInfo: true,
});

export const ViewerRoomsActions = Object.freeze({
  ...RoomsActions,
  viewUsers: true,
  viewHistory: true,
  viewInfo: true,
});
