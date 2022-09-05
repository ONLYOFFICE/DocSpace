import React, { useEffect, useState } from "react";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { ReactSVG } from "react-svg";

import Gallery from "./views/Gallery";
import Room from "./views/Room";
import Info from "./views/Info";
import { StyledInfoPanelBody, StyledTitle } from "./styles/common";

import { Base } from "@docspace/components/themes";
import withLoader from "../../../../HOCs/withLoader";
import Loaders from "@docspace/common/components/Loaders";
import { Text } from "@docspace/components";
import ItemContextOptions from "./sub-components/ItemContextOptions";
import ItemTitle from "./sub-components/ItemTitle";

const InfoPanelBodyContent = ({
  t,
  selectedItems,
  selectedFolder,

  selfId,
  personal,
  culture,

  isRoom,
  roomState,
  setIsRoom,

  getFolderInfo,
  onSelectItem,
  getShareUsers,
  setSharingPanelVisible,

  getIcon,
  getFolderIcon,
  createThumbnail,

  isGallery,
  gallerySelected,

  isFileCategory,
  isRootFolder,
  isFavoritesFolder,
  isRecentFolder,
  isRecycleBinFolder,
}) => {
  const defaultProps = {
    t,
    personal,
    culture,
    isFileCategory,
    isRootFolder,
    isRecycleBinFolder,
    isRecentFolder,
    isFavoritesFolder,
  };

  const detailsProps = {
    getFolderInfo,
    getIcon,
    getFolderIcon,
    getShareUsers,
    onSelectItem,
    setSharingPanelVisible,
    createThumbnail,
  };

  const roomProps = {
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

  const getSelection = () => {
    if (selectedItems.length > 1) return selectedItems;

    const newSelection =
      selectedItems.length === 0
        ? { ...selectedFolder, isSelectedFolder: true }
        : { ...selectedItems[0], isSelectedItem: true };

    const getItemIcon = (size) =>
      newSelection.isRoom
        ? newSelection.logo && newSelection.logo.big
          ? newSelection.logo.big
          : newSelection.icon
        : newSelection.isFolder
        ? getFolderIcon(newSelection.providerKey, size)
        : getIcon(size, newSelection.fileExst || ".file");

    newSelection.icon = getItemIcon(32);
    newSelection.hasCustonThumbnail = !!newSelection.thumbnailUrl;
    newSelection.thumbnailUrl = newSelection.thumbnailUrl || getItemIcon(96);

    return newSelection;
  };

  const selection = getSelection();

  useEffect(() => {
    setIsRoom(selection.isRoom);
  }, [selection]);

  return (
    <StyledInfoPanelBody>
      <ItemTitle
        t={t}
        selection={selection}
        isGallery={isGallery}
        isFileCategory={isFileCategory}
        getIcon={getIcon}
      />

      {isGallery ? (
        <Gallery selection={selection} {...galleryProps} />
      ) : isRoom ? (
        <Room selection={selection} {...roomProps} />
      ) : (
        <Info selection={selection} {...defaultProps} {...detailsProps} />
      )}
    </StyledInfoPanelBody>
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
    const { roomState, isRoom, setIsRoom } = auth.infoPanelStore;
    const selfId = auth.userStore.user.id;

    const {
      selection,
      bufferSelection,
      getFolderInfo,
      getShareUsers,
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
    const { gallerySelected } = oformsStore;

    const isFileCategory =
      isRootFolder &&
      (isRecycleBinFolder || isRecentFolder || isFavoritesFolder);

    // const selectedItems =
    // selection?.length > 0
    //   ? [...selection]
    //   : bufferSelection
    //   ? [bufferSelection]
    //   : [];
    const selectedItems = selection?.length > 0 ? [...selection] : [];

    const selectedFolder = {
      ...selectedFolderStore,
      isFolder: true,
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
      personal,
      culture,

      selectedItems,
      selectedFolder,

      isRoom,
      roomState,
      setIsRoom,

      getFolderInfo,
      onSelectItem,
      getShareUsers,
      setSharingPanelVisible,

      getIcon,
      getFolderIcon,
      createThumbnail,

      gallerySelected,

      isFileCategory,
      isRootFolder,
      isFavoritesFolder,
      isRecentFolder,
      isRecycleBinFolder,
    };
  }
)(
  withRouter(
    withTranslation(["InfoPanel", "Home", "Common", "Translations"])(
      withLoader(observer(InfoPanelBodyContent))(
        <Loaders.InfoPanelBodyLoader />
      )
    )
  )
);
