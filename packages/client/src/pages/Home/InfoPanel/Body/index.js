import React from "react";
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
  isRooms,
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
    else if (isRooms) return <Room {...roomProps} />;
    else return <Info {...defaultProps} {...detailsProps} />;
  };

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

    console.log(selectedItems);
    console.log({ ...selectedFolderStore });

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
      isFavoritesFolder,
      isRecentFolder,
      isRecycleBinFolder,

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
