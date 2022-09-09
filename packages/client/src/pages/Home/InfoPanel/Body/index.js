import React, { useState, useMemo, useEffect } from "react";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import withLoader from "@docspace/client/src/HOCs/withLoader";
import { getCategoryType } from "@docspace/client/src/helpers/utils";
import { CategoryType } from "@docspace/client/src/helpers/constants";
import Loaders from "@docspace/common/components/Loaders";

import ViewHelper from "./helpers/ViewHelper";
import ItemTitle from "./sub-components/ItemTitle";

import { StyledInfoPanelBody } from "./styles/common";
import { getRoomInfo } from "@docspace/common/api/rooms";

const InfoPanelBodyContent = ({
  t,

  selection,
  setSelection,
  selectionParentRoom,
  setSelectionParentRoom,
  normalizeSelection,
  roomsView,
  personalView,

  getIcon,
  getFolderIcon,

  isGallery,
  isFileCategory,
  ...props
}) => {
  const [selectedItems, setSelectedItems] = useState(props.selectedItems);
  const [selectedFolder, setSelectedFolder] = useState(props.selectedFolder);

  const categoryType = getCategoryType(location);
  let isRoomCategory =
    categoryType == CategoryType.Shared ||
    categoryType == CategoryType.SharedRoom ||
    categoryType == CategoryType.Archive ||
    categoryType == CategoryType.ArchivedRoom;

  const viewHelper = new ViewHelper({
    defaultProps: {
      t,
      selection,
      setSelection,
      isGallery,
      isFileCategory,
      personal: props.personal,
      culture: props.culture,
      isRootFolder: props.isRootFolder,
      isRecycleBinFolder: props.isRecycleBinFolder,
      isRecentFolder: props.isRecentFolder,
      isFavoritesFolder: props.isFavoritesFolder,
    },
    detailsProps: {
      getIcon,
      getFolderIcon,
      getFolderInfo: props.getFolderInfo,
      getShareUsers: props.getShareUsers,
      onSelectItem: props.onSelectItem,
      setSharingPanelVisible: props.setSharingPanelVisible,
      createThumbnail: props.createThumbnail,
    },
    membersProps: {
      selectionParentRoom,
      setSelectionParentRoom,
      selfId: props.selfId,
      getRoomMembers: props.getRoomMembers,
    },
    historyProps: {},
    galleryProps: {
      getIcon,
      gallerySelected: props.gallerySelected,
      personal: props.personal,
      culture: props.culture,
    },
  });

  const getSelection = () => {
    return selectedItems.length === 0
      ? normalizeSelection({ ...selectedFolder, isSelectedFolder: true })
      : selectedItems.length === 1
      ? normalizeSelection({ ...selectedItems[0], isSelectedItem: true })
      : [...Array(selectedItems.length).keys()];
  };

  const getView = (selection) => {
    if (Array.isArray(selection)) return viewHelper.SeveralItemsView();
    if (
      (isFileCategory && selection.isSelectedFolder) ||
      (isGallery && !selectedItems.length)
    )
      return viewHelper.NoItemView();

    if (isGallery) return viewHelper.GalleryView();

    switch (isRoomCategory ? roomsView : personalView) {
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
    if (selection && selection.isContextMenuSelection)
      setSelection({ ...selection, isContextMenuSelection: false });
  }, [selection]);

  useEffect(() => {
    if (selection && selection.isContextMenuSelection) return;

    const newSelection = getSelection();
    if (selection && selection.id === newSelection.id) return;
    setSelection(newSelection);
    // setCurrentRoomTitle({
    //   title: newSelection.title,
    //   isRoom: newSelection.isRoom,
    //   icon: newSelection.icon,
    //   logo: newSelection.logo,
    // });
  }, [selectedItems, selectedFolder]);

  //////////////////////////////////////////////////////////

  useEffect(() => {
    if (selectedItems.length !== props.selectedItems.length)
      setSelectedItems(props.selectedItems);
    else if (selectedItems[0]?.id !== props.selectedItems[0]?.id)
      setSelectedItems(props.selectedItems);
  }, [props.selectedItems]);

  useEffect(() => {
    if (selectedFolder.id !== props.selectedFolder.id) {
      setSelectedFolder(props.selectedFolder);
      console.log("\nSelected folder: ", selectedFolder);
    }
  }, [props.selectedFolder]);

  //////////////////////////////////////////////////////////

  // useEffect(() => {
  //   console.log("\nSelected items: ", selectedItems);
  //   console.log("\nSelected folder: ", selectedFolder);
  // }, [selectedItems, selectedFolder]);

  useEffect(async () => {
    const currentFolderRoomId = selectedFolder.pathParts[1];
    const storeRoomId = selectionParentRoom?.id;

    if (selection && selection.isRoom) {
      setSelectionParentRoom(selection);
      return;
    }
    if (currentFolderRoomId === storeRoomId) return;

    const newSelectionParentRoom = await getRoomInfo(currentFolderRoomId);
    if (storeRoomId !== newSelectionParentRoom.id) {
      setSelectionParentRoom(newSelectionParentRoom);
      console.log("Parent room: ", newSelectionParentRoom);
    }
  }, [selectedFolder]);

  useEffect(() => {
    console.log("\nSelected items: ", selectedItems);
    console.log("\nSelected folder: ", selectedFolder);
  }, [selectedItems, selectedFolder]);

  //////////////////////////////////////////////////////////

  if (!selection) return <Loaders.InfoPanelBodyLoader />;
  return (
    <StyledInfoPanelBody>
      <ItemTitle
        t={t}
        roomsView={roomsView}
        selection={
          isRoomCategory && roomsView === "members" && !selection.isRoom
            ? normalizeSelection(selectedFolder)
            : selection
        }
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
    const {
      selection,
      setSelection,
      selectionParentRoom,
      setSelectionParentRoom,
      normalizeSelection,

      roomsView,
      personalView,
    } = auth.infoPanelStore;
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
      selectionParentRoom,
      setSelectionParentRoom,
      normalizeSelection,
      roomsView,
      personalView,

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
    withTranslation([
      "InfoPanel",
      "FormGallery",
      "Home",
      "Common",
      "Translations",
    ])(
      withLoader(observer(InfoPanelBodyContent))(
        <Loaders.InfoPanelBodyLoader />
      )
    )
  )
);
