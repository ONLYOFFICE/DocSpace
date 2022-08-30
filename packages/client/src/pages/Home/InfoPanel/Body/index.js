import React, { useEffect } from "react";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import { Base } from "@docspace/components/themes";
import withLoader from "../../../../HOCs/withLoader";
import Loaders from "@docspace/common/components/Loaders";

import { StyledInfoPanelBody } from "./styles/styles.js";

import Gallery from "./views/Gallery";
import Room from "./views/Room";
import Info from "./views/Info";

const InfoPanelBodyContent = ({
  t,
  isRoom,
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
  isFavoritesFolder,
  isRecentFolder,
  isRecycleBinFolder,
  isGallery,
  gallerySelected,
  personal,
  createThumbnail,
  culture,
  roomState,
  calculateisRoom,
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
  };

  const detailsProps = {
    selectedFolder,
    getFolderInfo,
    getIcon,
    getFolderIcon,
    getShareUsers,
    onSelectItem,
    setSharingPanelVisible,
    createThumbnail,
  };

  const roomProps = {
    selectedItems,
    selectedFolder,
    roomState,
    defaultProps,
    membersProps: {
      selfId,
      getShareUsers,
    },
    historyProps: {
      personal,
      culture,
    },
    detailsProps,
  };

  const galleryProps = {
    gallerySelected,
    personal,
    culture,
  };

  const getInfoPanelBodyContent = () => {
    if (isGallery) return <Gallery {...galleryProps} />;
    else if (isRoom) return <Room {...roomProps} />;
    else return <Info {...defaultProps} {...detailsProps} />;
  };

  useEffect(() => {
    if (selectedItems.length === 1) calculateisRoom(selectedItems[0]);
    else if (selectedItems.length === 0) calculateisRoom(selectedFolder);
  }, [selectedItems, selectedFolder]);

  return <StyledInfoPanelBody>{getInfoPanelBodyContent()}</StyledInfoPanelBody>;
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
    const { roomState, isRoom, calculateisRoom } = auth.infoPanelStore;
    const selfId = auth.userStore.user.id;

    const {
      selection,
      bufferSelection,
      getFolderInfo,
      getShareUsers,
      createThumbnail,
    } = filesStore;
    const { gallerySelected } = oformsStore;

    const {
      isRecycleBinFolder,
      isRecentFolder,
      isFavoritesFolder,
    } = treeFoldersStore;

    const { getIcon, getFolderIcon } = settingsStore;
    const { onSelectItem } = filesActionsStore;
    const { setSharingPanelVisible } = dialogsStore;
    const { isRootFolder } = selectedFolderStore;

    const selectedItems =
      selection?.length > 0
        ? [...selection]
        : bufferSelection
        ? [bufferSelection]
        : [];

    const selectedFolder = {
      ...selectedFolderStore,
      isRoom: !!selectedFolderStore.roomType,
    };

    console.log(
      "Selected items: ",
      selectedItems,
      "\nSelected folder: ",
      selectedFolder
    );

    return {
      selfId,
      selectedFolder: selectedFolder,
      selectedItems: selectedItems,

      getFolderInfo,
      getShareUsers,
      getIcon,
      getFolderIcon,
      onSelectItem,
      setSharingPanelVisible,

      isRootFolder,
      isFavoritesFolder,
      isRecentFolder,
      isRecycleBinFolder,

      gallerySelected,
      personal,
      createThumbnail,
      culture,
      isRoom,
      roomState,
      calculateisRoom,
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
