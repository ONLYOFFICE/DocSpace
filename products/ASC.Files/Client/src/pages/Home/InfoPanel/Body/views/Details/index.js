import React from "react";
import EmptyScreen from "./EmptyScreen";
import SeveralItems from "./SeveralItems";
import SingleItem from "./SingleItem";

const Details = ({
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

  isRootFolder,
  isRecycleBinFolder,
  isRecentFolder,
  isFavoritesFolder,
  isShareFolder,
  isCommonFolder,
  isPrivacyFolder,
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
        // Can get future changes, currently only "My documents" displays its info
        isRootFolder &&
        (isRecycleBinFolder ||
          isRecentFolder ||
          isFavoritesFolder ||
          isShareFolder ||
          isCommonFolder) ? (
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

export default Details;
