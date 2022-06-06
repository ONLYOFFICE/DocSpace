import { inject, observer } from "mobx-react";
import React from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import GalleryItem from "./views/Gallery/GalleryItem";
import GalleryEmptyScreen from "./views/Gallery/GalleryEmptyScreen";
import { StyledInfoRoomBody } from "./styles/styles.js";
import { Base } from "@appserver/components/themes";
import FilesInfoPanel from "./views/Files";
import VirtualRoomInfoPanel from "./views/VirtualRoom";

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
  isRootFolder,
  isRecycleBinFolder,
  isRecentFolder,
  isFavoritesFolder,
  isShareFolder,
  isCommonFolder,
  isPrivacyFolder,
  isGallery,
  gallerySelected,
  personal,
  createThumbnail,
  culture,
}) => {
  return isGallery ? (
    !gallerySelected ? (
      <GalleryEmptyScreen />
    ) : (
      <StyledInfoRoomBody>
        <GalleryItem
          selectedItem={gallerySelected}
          personal={personal}
          culture={culture}
        />
      </StyledInfoRoomBody>
    )
  ) : (
    <StyledInfoRoomBody>
      {isPrivacyFolder ? (
        <VirtualRoomInfoPanel />
      ) : (
        <FilesInfoPanel
          t={t}
          selectedFolder={selectedFolder}
          selectedItems={selectedItems}
          getFolderInfo={getFolderInfo}
          getIcon={getIcon}
          getFolderIcon={getFolderIcon}
          getShareUsers={getShareUsers}
          onSelectItem={onSelectItem}
          setSharingPanelVisible={setSharingPanelVisible}
          createThumbnail={createThumbnail}
          personal={personal}
          culture={culture}
          isRootFolder={isRootFolder}
          isRecycleBinFolder={isRecycleBinFolder}
          isRecentFolder={isRecentFolder}
          isFavoritesFolder={isFavoritesFolder}
          isShareFolder={isShareFolder}
          isCommonFolder={isCommonFolder}
          isPrivacyFolder={isPrivacyFolder}
        />
      )}
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
    const { isRootFolder } = selectedFolderStore;
    const {
      isRecycleBinFolder,
      isRecentFolder,
      isFavoritesFolder,
      isShareFolder,
      isCommonFolder,
      isPrivacyFolder,
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

      isRootFolder,
      isRecycleBinFolder,
      isRecentFolder,
      isFavoritesFolder,
      isShareFolder,
      isCommonFolder,
      isPrivacyFolder,

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
