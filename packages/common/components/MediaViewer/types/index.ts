declare global {
  interface Window {
    Tiff: new (arg: object) => any;
  }
}

export type TranslationType = (key: string, opt?: object) => string;

export type NumberOrString = number | string;

export type NullOrUndefined = null | undefined;

export type PlaylistType = {
  id: number;
  canShare: boolean;
  fileExst: string;
  fileId: number;
  fileStatus: number;
  src: string;
  title: string;
};

export type CreatedType = {
  id: string;
  avatarSmall: string;
  displayName: string;
  hasAvatar: boolean;
  profileUrl: string;
};

export type SecurityType = {
  Comment: boolean;
  Copy: boolean;
  CustomFilter: boolean;
  Delete: boolean;
  Duplicate: boolean;
  Edit: boolean;
  EditHistory: boolean;
  FillForms: boolean;
  Lock: boolean;
  Move: boolean;
  Read: boolean;
  ReadHistory: boolean;
  Rename: boolean;
  Review: boolean;
};

export type ViewAccessabilityType = {
  CoAuhtoring: boolean;
  Convert: boolean;
  ImageView: boolean;
  MediaView: boolean;
  WebComment: boolean;
  WebCustomFilterEditing: boolean;
  WebEdit: boolean;
  WebRestrictedEditing: boolean;
  WebReview: boolean;
  WebView: boolean;
};

export interface IFile {
  id: number;
  access: number;
  canShare: boolean;
  comment: string;
  contentLength: string;
  created: string;
  createdBy: CreatedType;
  denyDownload: boolean;
  denySharing: boolean;
  fileExst: string;
  fileStatus: number;
  fileType: number;
  folderId: number;
  pureContentLength: number;
  rootFolderId: number;
  rootFolderType: number;
  security: SecurityType;
  shared: boolean;
  thumbnailStatus: number;
  title: string;
  updated: string;
  updatedBy: CreatedType;
  version: number;
  versionGroup: number;
  viewAccessability: ViewAccessabilityType;
  viewUrl: string;
  webUrl: string;
}

export type ContextMenuType = {
  id?: string;
  key: string;
  label: string;
  icon: string;
  disabled: boolean;
  onClick: VoidFunction;
  isSeparator?: undefined;
};

export type SeparatorType = {
  key: string;
  isSeparator: boolean;
  disabled: boolean;
};

export type ContextMenuModel = ContextMenuType | SeparatorType;
