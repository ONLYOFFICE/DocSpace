export const ArchiveRoomsActions = Object.freeze({
  edit: false,
  inviteUsers: false,
  changeUserRole: false,
  viewUsers: false,
  viewHistory: false,
  viewInfo: false,
  deleteUsers: false,
  restore: false,
  delete: false,
});

export const OwnerArchiveRoomsActions = Object.freeze({
  ...ArchiveRoomsActions,
  viewUsers: true,
  viewInfo: true,
  deleteUsers: true,
  restore: true,
  delete: true,
});

export const RoomAdminArchiveRoomsActions = Object.freeze({
  ...ArchiveRoomsActions,
  viewUsers: true,
  viewInfo: true,
  deleteUsers: true,
});

export const EditorArchiveRoomsActions = Object.freeze({
  ...ArchiveRoomsActions,
  viewInfo: true,
});

export const FormFillerArchiveRoomsActions = Object.freeze({
  ...ArchiveRoomsActions,
  viewInfo: true,
});

export const ReviewerArchiveRoomsActions = Object.freeze({
  ...ArchiveRoomsActions,
  viewInfo: true,
});

export const CommentatorArchiveRoomsActions = Object.freeze({
  ...ArchiveRoomsActions,
  viewInfo: true,
});

export const ViewerArchiveRoomsActions = Object.freeze({
  ...ArchiveRoomsActions,
  viewInfo: true,
});
