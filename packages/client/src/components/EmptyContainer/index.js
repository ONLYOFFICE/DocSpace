import React from "react";
import { observer, inject } from "mobx-react";
import RootFolderContainer from "./RootFolderContainer";
import EmptyFilterContainer from "./EmptyFilterContainer";
import EmptyFolderContainer from "./EmptyFolderContainer";
import { FileAction } from "@docspace/common/constants";
import { isMobile } from "react-device-detect";
import { Events } from "@docspace/common/constants";
import RoomNoAccessContainer from "./RoomNoAccessContainer";

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
  sectionWidth,
  isRoomNotFoundOrMoved,
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
    const event = new Event(Events.ROOM_CREATE);
    window.dispatchEvent(event);
  };

  if (isRoomNotFoundOrMoved) {
    return (
      <RoomNoAccessContainer
        linkStyles={linkStyles}
        sectionWidth={sectionWidth}
      />
    );
  }

  return isFiltered ? (
    <EmptyFilterContainer linkStyles={linkStyles} />
  ) : parentId === 0 ? (
    <RootFolderContainer
      onCreate={onCreate}
      linkStyles={linkStyles}
      onCreateRoom={onCreateRoom}
      sectionWidth={sectionWidth}
    />
  ) : (
    <EmptyFolderContainer
      sectionWidth={sectionWidth}
      onCreate={onCreate}
      linkStyles={linkStyles}
    />
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
    const { filter, roomsFilter, isErrorRoomNotAvailable } = filesStore;

    const {
      authorType,
      search,
      withSubfolders,
      filterType,
      searchInContent,
    } = filter;
    const {
      subjectId,
      filterValue,
      type,
      withSubfolders: withRoomsSubfolders,
      searchInContent: searchInContentRooms,
      tags,
      withoutTags,
    } = roomsFilter;

    const {
      isPrivacyFolder,
      isRoomsFolder,
      isArchiveFolder,
    } = treeFoldersStore;

    const isRooms = isRoomsFolder || isArchiveFolder;

    const { setCreateRoomDialogVisible } = dialogsStore;

    const isFiltered = isRooms
      ? filterValue ||
        type ||
        withRoomsSubfolders ||
        searchInContentRooms ||
        subjectId ||
        tags ||
        withoutTags
      : (authorType ||
          search ||
          !withSubfolders ||
          filterType ||
          searchInContent) &&
        !(isPrivacyFolder && isMobile);

    const isRoomNotFoundOrMoved =
      isFiltered === null &&
      selectedFolderStore.parentId === null &&
      isErrorRoomNotAvailable;

    return {
      theme: auth.settingsStore.theme,
      isFiltered,
      setCreateRoomDialogVisible,

      parentId: selectedFolderStore.parentId,
      isRoomNotFoundOrMoved,
    };
  }
)(observer(EmptyContainer));
