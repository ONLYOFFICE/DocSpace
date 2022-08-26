import React from "react";
import EmptyScreen from "./EmptyScreen";
import SeveralItems from "./SeveralItems";
import SingleItem from "./SingleItem";

const Info = ({
  t,

  selectedItems,
  selectedFolder,

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

  return (
    <>
      {selectedItems.length === 0 ? (
        // Can    its info
        isRootFolder &&
        (isRecycleBinFolder || isRecentFolder || isFavoritesFolder) ? (
          <EmptyScreen />
        ) : (
          singleItem({
            ...selectedFolder,
            isFolder: true,
          })
        )
      ) : selectedItems.length === 1 ? (
        singleItem(selectedItems[0])
      ) : (
        <SeveralItems selectedItems={selectedItems} getIcon={getIcon} />
      )}
    </>
  );
};

export default Info;
