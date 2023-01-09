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
