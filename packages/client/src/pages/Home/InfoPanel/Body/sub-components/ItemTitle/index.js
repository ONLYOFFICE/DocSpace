import React from "react";
import { inject, observer } from "mobx-react";

import AccountsItemTitle from "./AccountsItemTitle";
import FilesItemTitle from "./FilesItemTitle";
import GalleryItemTitle from "./GalleryItemTitle";

const ItemTitle = ({
  selection,
  gallerySelected,
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
    return (
      <GalleryItemTitle gallerySelected={gallerySelected} getIcon={getIcon} />
    );

  const filesItemSelection =
    isRooms &&
    !isSeveralItems &&
    roomsView === "info_members" &&
    !selection.isRoom &&
    !!selectionParentRoom
      ? selectionParentRoom
      : selection;

  return (
    <FilesItemTitle
      selectionLength={selectionLength}
      selection={filesItemSelection}
      isSeveralItems={isSeveralItems}
    />
  );
};

export default inject(({ auth, settingsStore, peopleStore, oformsStore }) => {
  const { selectionParentRoom, roomsView } = auth.infoPanelStore;
  const { getIcon } = settingsStore;
  const { getUserContextOptions } = peopleStore.contextOptionsStore;
  const { gallerySelected } = oformsStore;

  return {
    gallerySelected,
    getUserContextOptions,
    selectionParentRoom,
    roomsView,
    getIcon,
  };
})(observer(ItemTitle));
