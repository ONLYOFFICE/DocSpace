import { inject, observer } from "mobx-react";
import React from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import SeveralItems from "./SeveralItems";
import SingleItem from "./SingleItem";
import GalleryItem from "./GalleryItem";
import GalleryEmptyScreen from "./GalleryEmptyScreen";
import { StyledInfoRoomBody } from "./styles/styles.js";
import { Base } from "@appserver/components/themes";

const InfoPanelBodyContent = ({
  t,
  selectedFolder,
  selectedItems,
  getFolderInfo,
  getIcon,
  getFolderIcon,
  getShareUsers,
  onSelectItem,
  setSharingPanelVisible,
  isRecycleBinFolder,
  isRecentFolder,
  isFavoritesFolder,
  isGallery,
  gallerySelected,
  personal,
  createThumbnail,
  culture,
}) => {
  const singleItem = (item) => {
    const dontShowLocation = item.isFolder && item.parentId === 0;
    const dontShowSize = item.isFolder && (isFavoritesFolder || isRecentFolder);
    const dontShowAccess =
      isRecycleBinFolder ||
      (item.isFolder && item.parentId === 0) ||
      item.rootFolderId === 7 ||
      (item.isFolder && item.pathParts && item.pathParts[0] === 7);

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
        personal={personal}
        culture={culture}
        createThumbnail={createThumbnail}
      />
    );
  };

  return isGallery ? (
    !gallerySelected ? (
      <GalleryEmptyScreen />
    ) : (
      <StyledInfoRoomBody>
        <GalleryItem selectedItem={gallerySelected} />
      </StyledInfoRoomBody>
    )
  ) : (
    <StyledInfoRoomBody>
      <>
        {selectedItems.length === 0 ? (
          singleItem({
            ...selectedFolder,
            isFolder: true,
          })
        ) : selectedItems.length === 1 ? (
          singleItem(selectedItems[0])
        ) : (
          <SeveralItems selectedItems={selectedItems} getIcon={getIcon} />
        )}
      </>
    </StyledInfoRoomBody>
  );
};

InfoPanelBodyContent.defaultProps = { theme: Base };

export default inject(
  ({
    auth,
    filesStore,
    settingsStore,
    filesActionsStore,
    dialogsStore,
    treeFoldersStore,
    selectedFolderStore,
  }) => {
    const { personal, culture } = auth.settingsStore;

    const {
      selection,
      bufferSelection,
      getFolderInfo,
      getShareUsers,
      gallerySelected,
      createThumbnail,
    } = filesStore;
    const { getIcon, getFolderIcon } = settingsStore;
    const { onSelectItem } = filesActionsStore;
    const { setSharingPanelVisible } = dialogsStore;
    const {
      isRecycleBinFolder,
      isRecentFolder,
      isFavoritesFolder,
    } = treeFoldersStore;

    const selectedItems =
      selection?.length > 0
        ? [...selection]
        : bufferSelection
        ? [bufferSelection]
        : [];

    return {
      selectedFolder: { ...selectedFolderStore },
      selectedItems: selectedItems,
      getFolderInfo,
      getShareUsers,
      getIcon,
      getFolderIcon,
      onSelectItem,
      setSharingPanelVisible,
      isRecycleBinFolder,
      isRecentFolder,
      isFavoritesFolder,
      gallerySelected,
      personal,
      createThumbnail,
      culture,
    };
  }
)(
  withRouter(
    withTranslation(["InfoPanel", "Home", "Common", "Translations"])(
      observer(InfoPanelBodyContent)
    )
  )
);
