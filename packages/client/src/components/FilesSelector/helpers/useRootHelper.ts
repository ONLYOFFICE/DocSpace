import React from "react";

import { FolderType } from "@docspace/common/constants";

import CatalogFolderReactSvgUrl from "PUBLIC_DIR/images/catalog.folder.react.svg?url";
import CatalogUserReactSvgUrl from "PUBLIC_DIR/images/catalog.user.react.svg?url";

import { useRootHelperProps, Item } from "../FilesSelector.types";

import { defaultBreadCrumb } from "../utils";

const useRootHelper = ({
  setBreadCrumbs,
  setIsBreadCrumbsLoading,
  setItems,
  treeFolders,
}: useRootHelperProps) => {
  const [isRoot, setIsRoot] = React.useState<boolean>(false);

  const getRootData = React.useCallback(() => {
    setBreadCrumbs([defaultBreadCrumb]);
    setIsRoot(true);
    setIsBreadCrumbsLoading(false);
    const newItems: Item[] = [];

    treeFolders?.forEach((folder) => {
      if (
        folder.rootFolderType === FolderType.Rooms ||
        folder.rootFolderType === FolderType.USER
      ) {
        newItems.push({
          label: folder.title,
          title: folder.title,
          id: folder.id,
          parentId: folder.parentId,
          rootFolderType: folder.rootFolderType,
          filesCount: folder.filesCount,
          foldersCount: folder.foldersCount,
          security: folder.security,
          isFolder: true,

          avatar:
            folder.rootFolderType === FolderType.Rooms
              ? CatalogFolderReactSvgUrl
              : CatalogUserReactSvgUrl,
        });
      }
    });

    setItems(newItems);
  }, [treeFolders]);

  return { isRoot, setIsRoot, getRootData };
};

export default useRootHelper;
