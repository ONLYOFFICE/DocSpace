import { inject, observer } from "mobx-react";
import React from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import SeveralItems from "./SeveralItems";
import SingleItem from "./SingleItem";
import GalleryItem from "./GalleryItem";
import GalleryEmptyScreen from "./GalleryEmptyScreen";
import { StyledInfoRoomBody } from "./styles/styles.js";
import { Base } from "@docspace/components/themes";
import EmptyScreen from "./EmptyScreen";
import withLoader from "../../../../HOCs/withLoader";
import Loaders from "@docspace/common/components/Loaders";

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
      <>
        {selectedItems.length === 0 ? (
          // Can get future changes, currently only "My documents" displays its info
          isRootFolder &&
          (isRecycleBinFolder ||
            isRecentFolder ||
            isFavoritesFolder ||
            isShareFolder ||
            isCommonFolder ||
            isPrivacyFolder) ? (
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
    oformsStore,
  }) => {
    const { personal, culture } = auth.settingsStore;

    const {
      selection,
      bufferSelection,
      getFolderInfo,
      getShareUsers,
      createThumbnail,
    } = filesStore;
    const { gallerySelected } = oformsStore;
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
    withTranslation(["InfoPanel", "Files", "Common", "Translations"])(
      withLoader(observer(InfoPanelBodyContent))(
        <Loaders.InfoPanelBodyLoader isFolder />
      )
    )
  )
);
