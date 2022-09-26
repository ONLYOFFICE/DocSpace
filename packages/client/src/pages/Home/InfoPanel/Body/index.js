import React, { useState, useMemo, useEffect } from "react";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import withLoader from "@docspace/client/src/HOCs/withLoader";

import Loaders from "@docspace/common/components/Loaders";

import ViewHelper from "./helpers/ViewHelper";
import ItemTitle from "./sub-components/ItemTitle";

import { StyledInfoPanelBody } from "./styles/common";
import { getRoomInfo } from "@docspace/common/api/rooms";

const InfoPanelBodyContent = ({
  t,

  selection,
  setSelection,
  normalizeSelection,

  selectionParentRoom,
  setSelectionParentRoom,

  roomsView,
  fileView,

  getIsFileCategory,
  getIsRoomCategory,
  getIsGallery,

  gallerySelected,

  setBufferSelection,

  isRootFolder,

  getIcon,
  getFolderIcon,

  ...props
}) => {
  const [selectedItems, setSelectedItems] = useState(props.selectedItems);
  const [selectedFolder, setSelectedFolder] = useState(props.selectedFolder);

  const isFileCategory = getIsFileCategory();
  const isRoomCategory = getIsRoomCategory();
  const isGallery = getIsGallery();

  const isNoItem = (isRootFolder || isGallery) && selection?.isSelectedFolder;
  const isSeveralItems = selectedItems.length > 1;

  const viewHelper = new ViewHelper({
    defaultProps: {
      t,
      selection,
      setSelection,
      isFileCategory,
      isRoomCategory,
      isGallery,
      isRootFolder,

      personal: props.personal,
      culture: props.culture,
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
    historyProps: {
      getHistory: props.getHistory,
      getRoomHistory: props.getRoomHistory,
      getFileHistory: props.getFileHistory,
      getItemIcon: props.getItemIcon,
      openFileAction: props.openFileAction,
    },
    galleryProps: {
      getIcon,
      gallerySelected,
    },
  });

  const getSelection = () => {
    return selectedItems.length === 0
      ? normalizeSelection({ ...selectedFolder, isSelectedFolder: true })
      : selectedItems.length === 1
      ? normalizeSelection({ ...selectedItems[0], isSelectedItem: true })
      : [...Array(selectedItems.length).keys()];
  };

  const getView = () => {
    if (isNoItem) return viewHelper.NoItemView();
    if (isGallery) return viewHelper.GalleryView();
    if (isSeveralItems) return viewHelper.SeveralItemsView();

    switch (isRoomCategory ? roomsView : fileView) {
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

  //////////////////////////////////////////////////////////

  // Removing mark set with choosing item from context menu
  // for it to updated after selctedItems or selectedFolder change
  // (only for non-oforms items)
  useEffect(() => {
    if (isGallery || !selection?.isContextMenuSelection) return;
    setSelection(normalizeSelection(selection));
  }, [selection]);

  // Setting selection after selectedItems or selectedFolder update
  useEffect(() => {
    if (selection && selection.isContextMenuSelection) return;
    const newSelection = getSelection();
    if (selection && selection.id === newSelection.id) return;
    setSelection(newSelection);
  }, [selectedItems, selectedFolder]);

  //////////////////////////////////////////////////////////

  // Updating SelectedItems only if
  // a) Length of an array changed
  // b) Single chosen item changed
  useEffect(() => {
    const selectedItemsLengthChanged =
      selectedItems.length !== props.selectedItems.length;
    const singleSelectedItemChanged =
      selectedItems[0]?.id !== props.selectedItems[0]?.id;

    if (selectedItemsLengthChanged || singleSelectedItemChanged)
      setSelectedItems(props.selectedItems);
  }, [props.selectedItems]);

  // Updating SelectedFolder only if
  //   a) Selected folder changed
  useEffect(() => {
    if (selectedFolder.id !== props.selectedFolder.id)
      setSelectedFolder(props.selectedFolder);
  }, [props.selectedFolder]);

  // Updating selectionParentRoom after selectFolder change
  // if it is located in another room
  useEffect(async () => {
    if (isGallery) return;

    const currentFolderRoomId = selectedFolder.pathParts[1];
    const storeRoomId = selectionParentRoom?.id;

    if (!currentFolderRoomId) return;
    if (currentFolderRoomId === storeRoomId) return;

    const newSelectionParentRoom = await getRoomInfo(currentFolderRoomId);
    if (storeRoomId !== newSelectionParentRoom.id) {
      setSelectionParentRoom(normalizeSelection(newSelectionParentRoom));
      console.log("\nfor-dev  Parent room: ", newSelectionParentRoom);
    }
  }, [selectedFolder]);

  //////////////////////////////////////////////////////////

  useEffect(() => {
    console.log("\nfor-dev  Selected items: ", selectedItems);
    console.log("\nfor-dev  Selected folder: ", selectedFolder);
  }, [selectedItems, selectedFolder]);

  //////////////////////////////////////////////////////////

  if (!selection) return null;
  return (
    <StyledInfoPanelBody>
      {!isNoItem && (
        <ItemTitle
          t={t}
          selection={
            isRoomCategory &&
            roomsView === "members" &&
            !selection.isRoom &&
            selectionParentRoom
              ? selectionParentRoom
              : selection
          }
          isSeveralItems={isSeveralItems}
          isGallery={isGallery}
          getIcon={getIcon}
          setBufferSelection={setBufferSelection}
        />
      )}
      {getView()}
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
    selectedFolderStore,
    oformsStore,
  }) => {
    const selfId = auth.userStore.user.id;
    const { personal, culture } = auth.settingsStore;
    const { getIcon, getFolderIcon } = settingsStore;
    const { onSelectItem, openFileAction } = filesActionsStore;
    const { setSharingPanelVisible } = dialogsStore;
    const { isRootFolder } = selectedFolderStore;
    const { gallerySelected } = oformsStore;

    const {
      selection,
      setSelection,
      selectionParentRoom,
      setSelectionParentRoom,
      normalizeSelection,
      roomsView,
      fileView,
      getItemIcon,
      getIsFileCategory,
      getIsRoomCategory,
      getIsGallery,
    } = auth.infoPanelStore;

    const {
      selection: filesStoreSelection,
      bufferSelection,
      setBufferSelection,
      getFolderInfo,
      getShareUsers,
      getRoomMembers,
      getHistory,
      getRoomHistory,
      getFileHistory,
      createThumbnail,
    } = filesStore;

    const selectedItems =
      filesStoreSelection?.length > 0 ? [...filesStoreSelection] : [];

    // const selectedItems =
    //   filesStoreSelection?.length > 0
    //     ? [...filesStoreSelection]
    //     : bufferSelection
    //     ? [bufferSelection]
    //     : [];

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
      fileView,
      getItemIcon,
      getIsFileCategory,
      getIsRoomCategory,
      getIsGallery,

      selectedItems,
      selectedFolder,
      setBufferSelection,

      getFolderInfo,
      onSelectItem,
      getShareUsers,
      getRoomMembers,
      getHistory,
      getRoomHistory,
      getFileHistory,
      setSharingPanelVisible,

      getIcon,
      getFolderIcon,
      createThumbnail,
      openFileAction,

      gallerySelected,

      isRootFolder,
    };
  }
)(
  withRouter(
    withTranslation(["InfoPanel", "FormGallery", "Common", "Translations"])(
      observer(InfoPanelBodyContent)
    )
  )
);
