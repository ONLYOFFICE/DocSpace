import React from "react";

import AccountsItemTitle from "./AccountsItemTitle";
import FilesItemTitle from "./FilesItemTitle";
import GalleryItemTitle from "./GalleryItemTitle";

const ItemTitle = ({
  selection,
  selectionParentRoom,
  roomsView,

  isRooms,
  isAccounts,
  isGallery,
  isSeveralItems,

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
      selection={filesItemSelection}
      isSeveralItems={isSeveralItems}
      setBufferSelection={setBufferSelection}
      getIcon={getIcon}
      getContextOptions={getContextOptions}
      getContextOptionActions={getContextOptionActions}
    />
  );
};

export default ItemTitle;
