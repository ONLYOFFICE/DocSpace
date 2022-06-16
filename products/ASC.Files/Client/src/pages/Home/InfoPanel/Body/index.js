import { inject, observer } from "mobx-react";
import React from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import GalleryItem from "./views/Gallery/GalleryItem";
import GalleryEmptyScreen from "./views/Gallery/GalleryEmptyScreen";
import { StyledInfoRoomBody } from "./styles/styles.js";
import { Base } from "@appserver/components/themes";
import Details from "./views/Details";
import Members from "./views/Members";
import History from "./views/History";
import EmptyScreen from "./EmptyScreen";
import withLoader from "../../../../HOCs/withLoader";
import Loaders from "@appserver/common/components/Loaders";

const InfoPanelBodyContent = ({
  t,
  selfId,
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
  roomState,
}) => {
  const defaultProps = {
    t,
    selectedItems,
    personal,
    culture,
    isRootFolder,
    isRecycleBinFolder,
    isRecentFolder,
    isFavoritesFolder,
    isShareFolder,
    isCommonFolder,
    isPrivacyFolder,
  };

  const filesProps = {
    selectedFolder,
    getFolderInfo,
    getIcon,
    getFolderIcon,
    getShareUsers,
    onSelectItem,
    setSharingPanelVisible,
    createThumbnail,
  };

  const virtualRoomProps = {
    roomState,
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
      {isPrivacyFolder ? (
        <>
          {roomState === "members" ? (
            <Members t={t} selfId={selfId} />
          ) : roomState === "history" ? (
            <History t={t} personal={personal} culture={culture} />
          ) : (
            <Details {...defaultProps} {...filesProps} />
          )}
        </>
      ) : (
        <Details {...defaultProps} {...filesProps} />
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
    const { roomState } = auth.infoPanelStore;
    const selfId = auth.userStore.user.id;

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
      selfId,
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
      roomState,
    };
  }
)(
  withRouter(
    withTranslation(["InfoPanel", "Home", "Common", "Translations"])(
      withLoader(observer(InfoPanelBodyContent))(
        <Loaders.InfoPanelBodyLoader isFolder />
      )
    )
  )
);
