import { FolderType } from "@docspace/common/constants";

export const exceptSortedByTagsFolders = [
  FolderType.Recent,
  FolderType.TRASH,
  FolderType.Favorites,
  FolderType.Privacy,
];

export const exceptPrivacyTrashFolders = [FolderType.Privacy, FolderType.TRASH];
