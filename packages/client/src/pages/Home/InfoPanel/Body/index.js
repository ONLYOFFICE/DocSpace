import React, { useEffect } from "react";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import withLoader from "@docspace/client/src/HOCs/withLoader";
import Loaders from "@docspace/common/components/Loaders";

import ViewHelper from "./helpers/ViewHelper";
import ItemTitle from "./sub-components/ItemTitle";

import { StyledInfoPanelBody } from "./styles/common";

const InfoPanelBodyContent = ({
  t,

  selection,
  setSelection,
  roomView,
  itemView,

  selectedItems,
  selectedFolder,

  getIcon,
  getFolderIcon,

  isGallery,
  isFileCategory,
  ...rest
}) => {
  const viewHelper = new ViewHelper({
    defaultProps: {
      t,
      selection,
      setSelection,
      personal: rest.personal,
      culture: rest.culture,
      isFileCategory,
      isRootFolder: rest.isRootFolder,
      isRecycleBinFolder: rest.isRecycleBinFolder,
      isRecentFolder: rest.isRecentFolder,
      isFavoritesFolder: rest.isFavoritesFolder,
    },
    detailsProps: {
      getFolderInfo: rest.getFolderInfo,
      getIcon,
      getFolderIcon,
      getShareUsers: rest.getShareUsers,
      onSelectItem: rest.onSelectItem,
      setSharingPanelVisible: rest.setSharingPanelVisible,
      createThumbnail: rest.createThumbnail,
    },
    membersProps: {
      selfId: rest.selfId,
      getRoomMembers: rest.getRoomMembers,
    },
    historyProps: {
      personal: rest.personal,
      culture: rest.culture,
    },
    galleryProps: {
      gallerySelected: rest.gallerySelected,
      personal: rest.personal,
      culture: rest.culture,
    },
  });

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

  const getView = (selection) => {
    if (Array.isArray(selection)) return viewHelper.SeveralItemsView();
    if (isFileCategory && selection.isSelectedFolder)
      return viewHelper.NoItemView();
    if (isGallery) return viewHelper.GalleryView();

    switch (selection.isRoom ? roomView : itemView) {
      case "members": {
        return viewHelper.MembersView();
      }
      case "history": {
        return viewHelper.HistoryView();
      }
      case "details": {
        return viewHelper.DetailsView();
      }
    }
  };

  useEffect(() => {
    const newSelection = getSelection();
    if (selection && selection.id === newSelection.id) return;
    setSelection(newSelection);
  }, [selectedItems, selectedFolder]);

  //////////////////////////////////////////////////////////

  useEffect(() => {
    console.log(
      "Selected items: ",
      selectedItems,
      "\nSelected folder: ",
      selectedFolder
    );
  }, [selectedItems, selectedFolder]);

  //////////////////////////////////////////////////////////

  if (!selection) return <Loaders.InfoPanelBodyLoader />;
  return (
    <StyledInfoPanelBody>
      <ItemTitle
        t={t}
        selection={selection}
        isGallery={isGallery}
        isFileCategory={isFileCategory}
        getIcon={getIcon}
      />
      {getView(selection)}
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
    const { selection, setSelection, roomView, itemView } = auth.infoPanelStore;
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

    const selectedItems =
      filesStoreSelection?.length > 0 ? [...filesStoreSelection] : [];

    const selectedFolder = {
      ...selectedFolderStore,
      isFolder: true,
      isRoom: !!selectedFolderStore.roomType,
    };

    return {
      selfId,
      personal,
      culture,

      selection,
      setSelection,
      roomView,
      itemView,

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
