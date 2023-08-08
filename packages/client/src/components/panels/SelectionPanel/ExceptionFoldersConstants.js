import { FolderType } from "@docspace/common/constants";

export const exceptSortedByTagsFolders = [
  FolderType.Recent,
  FolderType.TRASH,
  FolderType.Favorites,
  FolderType.Privacy,
  FolderType.Archive,
];

export const exceptPrivacyTrashArchiveFolders = [
  FolderType.Privacy,
  FolderType.TRASH,
  FolderType.Archive,
];

export const roomsOnly = [
  FolderType.USER,
  FolderType.Recent,
  FolderType.TRASH,
  FolderType.Favorites,
  FolderType.Privacy,
  FolderType.Archive,
];

export const userFolderOnly = [
  FolderType.Recent,
  FolderType.Rooms,
  FolderType.TRASH,
  FolderType.Favorites,
  FolderType.Privacy,
  FolderType.Archive,
];
