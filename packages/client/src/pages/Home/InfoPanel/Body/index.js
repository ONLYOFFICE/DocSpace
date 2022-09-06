import React, { useEffect } from "react";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import withLoader from "@docspace/client/src/HOCs/withLoader";
import Loaders from "@docspace/common/components/Loaders";

import ItemTitle from "./sub-components/ItemTitle";
import Gallery from "./views/Gallery";
import Room from "./views/Room";
import Info from "./views/Info";

import { StyledInfoPanelBody } from "./styles/common";

const InfoPanelBodyContent = ({
  t,

  selection,
  setSelection,
  roomState,

  selectedItems,
  selectedFolder,

  selfId,
  personal,
  culture,

  getFolderInfo,
  onSelectItem,
  getShareUsers,
  getRoomMembers,
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
    selection,
    setSelection,
    personal,
    culture,
    isFileCategory,
    isRootFolder,
    isRecycleBinFolder,
    isRecentFolder,
    isFavoritesFolder,
  };

  const titleProps = {
    selection,
    isGallery,
    isFileCategory,
    getIcon,
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
      getRoomMembers,
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

  useEffect(() => {
    const newSelection = getSelection();
    if (selection && selection.id === newSelection.id) return;
    setSelection(newSelection);
  }, [selectedItems, selectedFolder]);

  if (!selection) return <Loaders.InfoPanelBodyLoader />;
  return (
    <StyledInfoPanelBody>
      <ItemTitle {...defaultProps} {...titleProps} />

      {isGallery ? (
        <Gallery {...defaultProps} {...galleryProps} />
      ) : selection.isRoom ? (
        <Room {...defaultProps} {...roomProps} />
      ) : (
        <Info {...defaultProps} {...detailsProps} />
      )}
    </StyledInfoPanelBody>
  );
};

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
    const { selection, setSelection, roomState } = auth.infoPanelStore;
    const selfId = auth.userStore.user.id;

    const {
      selection: filesStoreSelection,
      bufferSelection,
      getFolderInfo,
      getShareUsers,
      getRoomMembers,
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
    const selectedItems =
      filesStoreSelection?.length > 0 ? [...filesStoreSelection] : [];

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

      selection,
      setSelection,
      roomState,

      selectedItems,
      selectedFolder,

      getFolderInfo,
      onSelectItem,
      getShareUsers,
      getRoomMembers,
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
