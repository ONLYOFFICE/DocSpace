import React from "react";
import { observer, inject } from "mobx-react";
import RootFolderContainer from "./RootFolderContainer";
import EmptyFilterContainer from "./EmptyFilterContainer";
import EmptyFolderContainer from "./EmptyFolderContainer";
import { FileAction } from "@appserver/common/constants";
import { isMobile } from "react-device-detect";

const linkStyles = {
  isHovered: true,
  type: "action",
  fontWeight: "600",
  color: "#555f65",
  className: "empty-folder_link",
  display: "flex",
};

const EmptyContainer = ({
  isFiltered,
  setAction,
  isPrivacyFolder,
  parentId,
  isEncryptionSupport,
}) => {
  const onCreate = (e) => {
    const format = e.currentTarget.dataset.format || null;
    setAction({
      type: FileAction.Create,
      extension: format,
      id: -1,
    });
  };

  return isFiltered ? (
    <EmptyFilterContainer linkStyles={linkStyles} />
  ) : parentId === 0 ? (
    <RootFolderContainer onCreate={onCreate} linkStyles={linkStyles} />
  ) : (
    <EmptyFolderContainer onCreate={onCreate} linkStyles={linkStyles} />
  );
};

export default inject(
  ({ auth, filesStore, treeFoldersStore, selectedFolderStore }) => {
    const {
      authorType,
      search,
      withSubfolders,
      filterType,
    } = filesStore.filter;
    const isPrivacyFolder = treeFoldersStore.isPrivacyFolder;
    const isFiltered =
      (authorType || search || !withSubfolders || filterType) &&
      !(isPrivacyFolder && isMobile);

    return {
      isEncryptionSupport: auth.settingsStore.isEncryptionSupport,
      isFiltered,
      setAction: filesStore.fileActionStore.setAction,
      isPrivacyFolder,
      parentId: selectedFolderStore.parentId,
    };
  }
)(observer(EmptyContainer));
