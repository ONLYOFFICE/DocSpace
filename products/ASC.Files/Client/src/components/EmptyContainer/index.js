import React from "react";
import { observer, inject } from "mobx-react";
import RootFolderContainer from "./RootFolderContainer";
import EmptyFilterContainer from "./EmptyFilterContainer";
import EmptyFolderContainer from "./EmptyFolderContainer";
import { FileAction } from "@appserver/common/constants";
import { isMobile } from "react-device-detect";
import { Events } from "../../helpers/constants";

const linkStyles = {
  isHovered: true,
  type: "action",
  fontWeight: "600",
  className: "empty-folder_link",
  display: "flex",
};

const EmptyContainer = ({
  isFiltered,
  parentId,
  theme,
  setCreateRoomDialogVisible,
}) => {
  linkStyles.color = theme.filesEmptyContainer.linkColor;

  const onCreate = (e) => {
    const format = e.currentTarget.dataset.format || null;

    const event = new Event(Events.CREATE);

    const payload = {
      extension: format,
      id: -1,
    };
    event.payload = payload;

    window.dispatchEvent(event);
  };

  const onCreateRoom = (e) => {
    setCreateRoomDialogVisible(true);
    console.log("create room");
  };

  return isFiltered ? (
    <EmptyFilterContainer linkStyles={linkStyles} />
  ) : parentId === 0 ? (
    <RootFolderContainer
      onCreate={onCreate}
      linkStyles={linkStyles}
      onCreateRoom={onCreateRoom}
    />
  ) : (
    <EmptyFolderContainer onCreate={onCreate} linkStyles={linkStyles} />
  );
};

export default inject(
  ({
    auth,
    filesStore,
    dialogsStore,
    treeFoldersStore,
    selectedFolderStore,
  }) => {
    const {
      authorType,
      search,
      withSubfolders,
      filterType,
    } = filesStore.filter;
    const { isPrivacyFolder } = treeFoldersStore;

    const { setCreateRoomDialogVisible } = dialogsStore;

    const isFiltered =
      (authorType || search || !withSubfolders || filterType) &&
      !(isPrivacyFolder && isMobile);

    return {
      theme: auth.settingsStore.theme,
      isFiltered,
      setCreateRoomDialogVisible,

      parentId: selectedFolderStore.parentId,
    };
  }
)(observer(EmptyContainer));
