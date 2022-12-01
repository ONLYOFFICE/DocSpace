import React, { useEffect } from "react";
import { withRouter } from "react-router";
import ModalDialog from "@docspace/components/modal-dialog";
import { StyledDeleteDialog } from "./StyledDeleteDialog";
import Button from "@docspace/components/button";
import Text from "@docspace/components/text";
import { withTranslation } from "react-i18next";
import toastr from "@docspace/components/toast/toastr";
import { inject, observer } from "mobx-react";

const DeleteDialogComponent = (props) => {
  const {
    t,
    deleteAction,
    unsubscribeAction,
    setBufferSelection,
    setRemoveMediaItem,
    setDeleteDialogVisible,
    personal,
    visible,
    tReady,
    isLoading,
    unsubscribe,
    isPrivacyFolder,
    isRecycleBinFolder,
    isRoomDelete,
    setIsRoomDelete,
    deleteRoomsAction,
  } = props;

  const selection = [];
  let i = 0;

  while (props.selection.length !== i) {
    const item = props.selection[i];

    if (!item?.isEditing) {
      if (item?.access === 0 || item?.access === 1 || unsubscribe) {
        selection.push(item);
      }
    }
    i++;
  }

  useEffect(() => {
    document.addEventListener("keyup", onKeyUp, false);

    return () => {
      document.removeEventListener("keyup", onKeyUp, false);
    };
  }, []);

  const onKeyUp = (e) => {
    if (e.keyCode === 27) onClose();
    if (e.keyCode === 13 || e.which === 13) onDelete();
  };

  const onDelete = () => {
    onClose();

    const translations = {
      deleteOperation: t("Translations:DeleteOperation"),
      deleteFromTrash: t("Translations:DeleteFromTrash"),
      deleteSelectedElem: t("Translations:DeleteSelectedElem"),
      FileRemoved: t("Files:FileRemoved"),
      FolderRemoved: t("Files:FolderRemoved"),
    };

    if (!selection.length) return;

    deleteAction(translations, selection);
  };

  const onUnsubscribe = () => {
    onClose();

    if (!selection.length) return;

    let filesId = [];
    let foldersId = [];

    selection.map((item) => {
      item.fileExst ? filesId.push(item.id) : foldersId.push(item.id);
    });

    unsubscribeAction(filesId, foldersId).catch((err) => toastr.error(err));
  };

  const onDeleteRoom = async () => {
    const translations = {
      deleteOperation: t("Translations:DeleteOperation"),
      successRemoveFile: t("Files:FileRemoved"),
      successRemoveFolder: t("Files:FolderRemoved"),
      successRemoveRoom: t("Files:RoomRemoved"),
      successRemoveRooms: t("Files:RoomsRemoved"),
    };

    const itemId = selection.map((s) => s.id);

    await deleteRoomsAction(itemId, translations);

    onClose();
  };

  const onClose = () => {
    setBufferSelection(null);
    setRemoveMediaItem(null);
    setIsRoomDelete(false);
    setDeleteDialogVisible(false);
  };

  const moveToTrashTitle = () => {
    if (unsubscribe) return t("UnsubscribeTitle");
    return t("MoveToTrashTitle");
  };

  const moveToTrashNoteText = () => {
    const isFolder = selection[0]?.isFolder || !!selection[0]?.parentId;

    if (selection.length > 1) {
      if (isRoomDelete)
        return `${t("DeleteRooms")} ${t("Common:WantToContinue")}`;
      return t("MoveToTrashItems");
    } else {
      if (isRoomDelete)
        return `${t("DeleteRoom")} ${t("Common:WantToContinue")}`;

      return !isFolder
        ? t("MoveToTrashFile")
        : personal
        ? ""
        : t("MoveToTrashFolder");
    }
  };

  const title = isRoomDelete
    ? t("EmptyTrashDialog:DeleteForeverTitle")
    : isPrivacyFolder || isRecycleBinFolder || selection[0]?.providerKey
    ? t("Common:Confirmation")
    : moveToTrashTitle();

  const noteText = unsubscribe ? t("UnsubscribeNote") : moveToTrashNoteText();

  const accessButtonLabel = isRoomDelete
    ? t("EmptyTrashDialog:DeleteForeverButton")
    : isPrivacyFolder || isRecycleBinFolder || selection[0]?.providerKey
    ? t("Common:OKButton")
    : unsubscribe
    ? t("UnsubscribeButton")
    : t("MoveToTrashButton");

  return (
    <StyledDeleteDialog isLoading={!tReady} visible={visible} onClose={onClose}>
      <ModalDialog.Header>{title}</ModalDialog.Header>
      <ModalDialog.Body>
        <div className="modal-dialog-content-body">
          <Text noSelect>{noteText}</Text>
        </div>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          id="delete-file-modal_submit"
          key="OkButton"
          label={accessButtonLabel}
          size="normal"
          primary
          scale
          onClick={
            isRoomDelete ? onDeleteRoom : unsubscribe ? onUnsubscribe : onDelete
          }
          isLoading={isLoading}
          isDisabled={!selection.length}
        />
        <Button
          id="delete-file-modal_cancel"
          key="CancelButton"
          label={t("Common:CancelButton")}
          size="normal"
          scale
          onClick={onClose}
          isLoading={isLoading}
        />
      </ModalDialog.Footer>
    </StyledDeleteDialog>
  );
};

const DeleteDialog = withTranslation([
  "DeleteDialog",
  "Common",
  "Translations",
  "Files",
  "EmptyTrashDialog",
])(DeleteDialogComponent);

export default inject(
  ({ filesStore, dialogsStore, filesActionsStore, treeFoldersStore, auth }) => {
    const {
      selection,
      isLoading,
      bufferSelection,
      setBufferSelection,
    } = filesStore;
    const {
      deleteAction,
      unsubscribeAction,
      deleteRoomsAction,
    } = filesActionsStore;
    const { isPrivacyFolder, isRecycleBinFolder } = treeFoldersStore;

    const {
      deleteDialogVisible: visible,
      setDeleteDialogVisible,
      removeMediaItem,
      setRemoveMediaItem,
      unsubscribe,
      isRoomDelete,
      setIsRoomDelete,
    } = dialogsStore;

    const { personal } = auth.settingsStore;

    return {
      selection: removeMediaItem
        ? [removeMediaItem]
        : selection.length
        ? selection
        : [bufferSelection],
      isLoading,
      visible,
      isPrivacyFolder,
      isRecycleBinFolder,

      setDeleteDialogVisible,
      deleteAction,
      unsubscribeAction,
      unsubscribe,

      setRemoveMediaItem,

      personal,
      setBufferSelection,

      isRoomDelete,
      setIsRoomDelete,
      deleteRoomsAction,
    };
  }
)(withRouter(observer(DeleteDialog)));
