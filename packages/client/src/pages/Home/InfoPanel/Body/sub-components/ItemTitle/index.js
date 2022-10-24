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
  getIcon,
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
      getIcon={getIcon}
    />
  );
};

export default inject(({ auth, settingsStore }) => {
  const { selectionParentRoom, roomsView } = auth.infoPanelStore;
  const { getIcon } = settingsStore;

  return {
    selectionParentRoom,
    roomsView,
    getIcon,
  };
})(observer(ItemTitle));
