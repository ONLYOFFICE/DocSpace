import React from "react";
import EmptyScreen from "./EmptyScreen";
import SeveralItems from "./SeveralItems";
import SingleItem from "./SingleItem";

const Info = ({
  t,

  selection,

  getFolderInfo,
  getIcon,
  getFolderIcon,
  getShareUsers,
  onSelectItem,
  setSharingPanelVisible,
  createThumbnail,

  personal,
  culture,

  isRoom,

  isFileCategory,
  isRootFolder,
  isRecycleBinFolder,
  isRecentFolder,
  isFavoritesFolder,
}) => {
  const singleItem = (item) => {
    const dontShowLocation = isRootFolder;
    const dontShowSize = item.isFolder && (isFavoritesFolder || isRecentFolder);
    const dontShowAccess =
      isRecycleBinFolder ||
      isRootFolder ||
      item.rootFolderId === 7 ||
      (item.isFolder && item.pathParts && item.pathParts[0] === 7);
    const dontShowOwner = isRootFolder && (isFavoritesFolder || isRecentFolder);

    return (
      <SingleItem
        t={t}
        isRoom={isRoom}
        selectedItem={item}
        onSelectItem={onSelectItem}
        setSharingPanelVisible={setSharingPanelVisible}
        getFolderInfo={getFolderInfo}
        getIcon={getIcon}
        getFolderIcon={getFolderIcon}
        getShareUsers={getShareUsers}
        dontShowLocation={dontShowLocation}
        dontShowSize={dontShowSize}
        dontShowAccess={dontShowAccess}
        dontShowOwner={dontShowOwner}
        personal={personal}
        culture={culture}
        createThumbnail={createThumbnail}
      />
    );
  };

  return isFileCategory && selection.isSelectedFolder ? (
    <EmptyScreen />
  ) : Array.isArray(selection) ? (
    <SeveralItems selectedItems={selection} getIcon={getIcon} />
  ) : (
    singleItem(selection)
  );
};

export default Info;
