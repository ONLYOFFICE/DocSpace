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
  updateSelection,
  setUpdateSelection,
  normalizeSelection,
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
    membersProps: { selectionParentRoom, setSelectionParentRoom },
    historyProps: { selectedFolder: selectedFolder },
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

  // Removing mark set with choosing item from context menu
  // for it to updated after selctedItems or selectedFolder change
  // (only for non-oforms items)
  useEffect(() => {
    if (selection && selection.isContextMenuSelection)
      setSelection({ ...selection, isContextMenuSelection: false });
  }, [selection]);

  // Setting selection after selectedItems or selectedFolder update
  useEffect(() => {
    if (selection?.isContextMenuSelection) return;

    const newSelection = calculateSelection({
      selectedItems: selectedItems,
      selectedFolder: selectedFolder,
    });

    if (
      selection?.id === newSelection.id &&
      selection?.isFolder === newSelection.isFolder &&
      !newSelection.length
    )
      return;

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
    if (!isRooms) return;

    const currentFolderRoomId =
      selectedFolder?.pathParts && selectedFolder.pathParts[1];
    const storeRoomId = selectionParentRoom?.id;

    if (!currentFolderRoomId) return;
    if (currentFolderRoomId === storeRoomId) return;

    const newSelectionParentRoom = await getRoomInfo(currentFolderRoomId);
    if (storeRoomId !== newSelectionParentRoom.id)
      setSelectionParentRoom(normalizeSelection(newSelectionParentRoom));
  }, [selectedFolder]);

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

export default inject(
  ({ auth, filesStore, selectedFolderStore, oformsStore, peopleStore }) => {
    const { isRootFolder } = selectedFolderStore;
    const { gallerySelected } = oformsStore;

    const {
      selection,
      calculateSelection,
      setSelection,
      updateSelection,
      setUpdateSelection,
      selectionParentRoom,
      setSelectionParentRoom,
      normalizeSelection,
      roomsView,
      fileView,
      getIsFiles,
      getIsRooms,
      getIsAccounts,
      getIsGallery,
    } = auth.infoPanelStore;

    const { selection: filesStoreSelection } = filesStore;

    const {
      selection: peopleStoreSelection,
      bufferSelection: peopleStoreBufferSelection,
    } = peopleStore.selectionStore;

    const selectedItems = getIsAccounts()
      ? peopleStoreSelection.length
        ? [...peopleStoreSelection]
        : peopleStoreBufferSelection
        ? [peopleStoreBufferSelection]
        : []
      : filesStoreSelection?.length > 0
      ? [...filesStoreSelection]
      : [];

    const selectedFolder = {
      ...selectedFolderStore,
      isFolder: true,
      isRoom: !!selectedFolderStore.roomType,
    };

    return {
      selection,
      setSelection,
      calculateSelection,

      updateSelection,
      setUpdateSelection,
      selectionParentRoom,
      setSelectionParentRoom,
      normalizeSelection,
      roomsView,
      fileView,

      getIsFiles,
      getIsRooms,
      getIsAccounts,
      getIsGallery,

      selectedItems,
      selectedFolder,

      gallerySelected,
      isRootFolder,
    };
  }
)(withRouter(observer(InfoPanelBodyContent)));
