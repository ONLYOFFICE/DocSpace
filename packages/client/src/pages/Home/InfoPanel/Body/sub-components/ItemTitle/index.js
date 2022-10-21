import React from "react";
import { inject, observer } from "mobx-react";

import AccountsItemTitle from "./AccountsItemTitle";
import FilesItemTitle from "./FilesItemTitle";
import GalleryItemTitle from "./GalleryItemTitle";

const ItemTitle = ({
  selection,
  isRooms,
  isAccounts,
  isGallery,
  isSeveralItems,
  selectionLength,

  selectionParentRoom,
  roomsView,

  setBufferSelection,
  getIcon,

  getContextOptions,
  getContextOptionActions,
  getUserContextOptions,
}) => {
  if (!selection) return null;

  if (isAccounts)
    return (
      <AccountsItemTitle
        selection={selection}
        isSeveralItems={isSeveralItems}
        getUserContextOptions={getUserContextOptions}
        selectionLength={selectionLength}
      />
    );

  if (isGallery)
    return <GalleryItemTitle selection={selection} getIcon={getIcon} />;

  const filesItemSelection =
    isRooms &&
    !isSeveralItems &&
    roomsView === "members" &&
    !selection.isRoom &&
    !!selectionParentRoom
      ? selectionParentRoom
      : selection;

  return (
    <FilesItemTitle
      selectionLength={selectionLength}
      selection={filesItemSelection}
      isSeveralItems={isSeveralItems}
      setBufferSelection={setBufferSelection}
      getIcon={getIcon}
      getContextOptions={getContextOptions}
      getContextOptionActions={getContextOptionActions}
    />
  );
};

export default inject(
  ({ auth, settingsStore, filesStore, peopleStore, contextOptionsStore }) => {
    const { selectionParentRoom, roomsView } = auth.infoPanelStore;
    const { getIcon } = settingsStore;
    const { getUserContextOptions } = peopleStore.contextOptionsStore;

    const {
      setBufferSelection,
      getFilesContextOptions: getContextOptions,
    } = filesStore;

    const {
      getFilesContextOptions: getContextOptionActions,
    } = contextOptionsStore;

    return {
      selectionParentRoom,
      roomsView,

      setBufferSelection,
      getIcon,

      getContextOptions,
      getContextOptionActions,
      getUserContextOptions,
    };
  }
)(observer(ItemTitle));
