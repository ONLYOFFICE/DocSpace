export const ArchiveFilesActions = Object.freeze({
  create: false,
  load: false,
  edit: false,
  fillForm: false,
  peerReview: false,
  commenting: false,
  block: false,
  viewVersionHistory: false,
  changeVersionHistory: false,
  viewContent: false,
  viewComments: false,
  copyAtBuffer: false,
  printing: false,
  download: false,
  deleteSelf: false,
  moveSelf: false,
  deleteAlien: false,
  moveAlien: false,
  rename: false,
  copyToPersonal: false,
  saveAsForm: false,
  canCopy: false,
});

export const OwnerArchiveFilesActions = Object.freeze({
  ...FilesActions,
  viewVersionHistory: true,
  viewContent: true,
  viewComments: true,
  copyAtBuffer: true,
  printing: true,
  download: true,
  copyToPersonal: true,
  canCopy: true,
});

export const RoomAdminArchiveFilesActions = Object.freeze({
  ...FilesActions,
  viewVersionHistory: true,
  viewContent: true,
  viewComments: true,
  copyAtBuffer: true,
  printing: true,
  download: true,
  copyToPersonal: true,
  canCopy: true,
});

export const EditorArchiveFilesActions = Object.freeze({
  ...FilesActions,
  viewVersionHistory: true,
  viewContent: true,
  viewComments: true,
  copyAtBuffer: true,
  printing: true,
  download: true,
});

export const FormFillerArchiveFilesActions = Object.freeze({
  ...FilesActions,
  viewContent: true,
  viewComments: true,
  copyAtBuffer: true,
  printing: true,
  download: true,
});

export const ReviewerArchiveFilesActions = Object.freeze({
  ...FilesActions,
  viewContent: true,
  viewComments: true,
  copyAtBuffer: true,
  printing: true,
  download: true,
});

export const CommentatorArchiveFilesActions = Object.freeze({
  ...FilesActions,
  viewContent: true,
  viewComments: true,
  copyAtBuffer: true,
  printing: true,
  download: true,
});

export const ViewerArchiveFilesActions = Object.freeze({
  ...FilesActions,
  viewContent: true,
  viewComments: true,
  copyAtBuffer: true,
  printing: true,
  download: true,
});
