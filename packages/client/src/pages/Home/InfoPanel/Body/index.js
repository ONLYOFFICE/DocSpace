import React, { useState, useEffect } from "react";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";

import ViewHelper from "./helpers/ViewHelper";
import ItemTitle from "./sub-components/ItemTitle";

import { StyledInfoPanelBody } from "./styles/common";
import { getRoomInfo } from "@docspace/common/api/rooms";

const InfoPanelBodyContent = ({
  selection,
  setSelection,
  calculateSelection,
  normalizeSelection,
  isItemChanged,
  selectionParentRoom,
  setSelectionParentRoom,
  roomsView,
  fileView,
  getIsFiles,
  getIsRooms,
  getIsAccounts,
  getIsGallery,
  gallerySelected,
  isRootFolder,
  ...props
}) => {
  const [selectedItems, setSelectedItems] = useState(props.selectedItems);
  const [selectedFolder, setSelectedFolder] = useState(props.selectedFolder);

  const isFiles = getIsFiles();
  const isRooms = getIsRooms();
  const isAccounts = getIsAccounts();
  const isGallery = getIsGallery();
  const isSeveralItems = props.selectedItems.length > 1;
  const isNoItem =
    (isGallery && !gallerySelected) ||
    (!selection?.title && !isSeveralItems && !isAccounts) ||
    ((isRootFolder || isAccounts) &&
      selection?.isSelectedFolder &&
      !selection?.wasContextMenuSelection);

  const defaultProps = {
    selection,
    isFiles,
    isRooms,
    isAccounts,
    isGallery,
    isRootFolder,
    isSeveralItems,
  };

  const viewHelper = new ViewHelper({
    defaultProps: defaultProps,
    detailsProps: {},
    membersProps: {},
    historyProps: { selectedFolder },
    accountsProps: {},
    galleryProps: {},
  });

  const getView = () => {
    if (isNoItem) return viewHelper.NoItemView();
    if (isSeveralItems) return viewHelper.SeveralItemsView();
    if (isGallery) return viewHelper.GalleryView();
    if (isAccounts) return viewHelper.AccountsView();

    switch (isRooms ? roomsView : fileView) {
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

  // Updating SelectedItems only if
  // a) Length of an array changed
  // b) Single chosen item changed
  useEffect(() => {
    const selectedItemsLengthChanged =
      selectedItems.length !== props.selectedItems.length;

    const singleSelectedItemChanged =
      selectedItems[0] &&
      props.selectedItems[0] &&
      isItemChanged(selectedItems[0], props.selectedItems[0]);

    if (selectedItemsLengthChanged || singleSelectedItemChanged)
      setSelectedItems(props.selectedItems);
  }, [props.selectedItems]);

  // Updating SelectedFolder only if
  //   a) Selected folder changed
  useEffect(() => {
    const selectedFolderChanged = isItemChanged(
      selectedFolder,
      props.selectedFolder
    );

    if (selectedFolderChanged) setSelectedFolder(props.selectedFolder);
  }, [props.selectedFolder]);

  // Updating selectionParentRoom after selectFolder change
  // if it is located in another room
  useEffect(async () => {
    if (!isRooms) return;

    const currentFolderRoomId =
      selectedFolder?.pathParts && selectedFolder.pathParts[1];
    const storeRoomId = selectionParentRoom?.id;
    if (!currentFolderRoomId || currentFolderRoomId === storeRoomId) return;

    const newSelectionParentRoom = await getRoomInfo(currentFolderRoomId);
    if (storeRoomId === newSelectionParentRoom.id) return;

    setSelectionParentRoom(normalizeSelection(newSelectionParentRoom));
  }, [selectedFolder]);

  //////////////////////////////////////////////////////////

  // Setting selection after selectedItems or selectedFolder update
  useEffect(() => {
    setSelection(calculateSelection());
  }, [selectedItems, selectedFolder]);

  //////////////////////////////////////////////////////////

  useEffect(() => {
    console.log("\nfor-dev  Selected items: ", selectedItems);
    console.log("\nfor-dev  Selected folder: ", selectedFolder);
  }, [selectedItems, selectedFolder]);

  //////////////////////////////////////////////////////////

  if (!selection && !isGallery) return null;

  return (
    <StyledInfoPanelBody>
      {!isNoItem && (
        <ItemTitle {...defaultProps} selectionLength={selectedItems.length} />
      )}
      {getView()}
    </StyledInfoPanelBody>
  );
};

export default inject(({ auth, selectedFolderStore }) => {
  const {
    selection,
    setSelection,
    calculateSelection,
    normalizeSelection,
    isItemChanged,
    selectionParentRoom,
    setSelectionParentRoom,
    roomsView,
    fileView,
    getIsFiles,
    getIsRooms,
    getIsAccounts,
    getIsGallery,
  } = auth.infoPanelStore;

  const { isRootFolder } = selectedFolderStore;

  const selectedItems = auth.infoPanelStore.getSelectedItems();
  const selectedFolder = auth.infoPanelStore.getSelectedFolder();

  return {
    selection,
    setSelection,
    calculateSelection,
    normalizeSelection,
    isItemChanged,
    selectionParentRoom,
    setSelectionParentRoom,
    roomsView,
    fileView,
    getIsFiles,
    getIsRooms,
    getIsAccounts,
    getIsGallery,

    selectedItems,
    selectedFolder,

    isRootFolder,
  };
})(withRouter(observer(InfoPanelBodyContent)));
