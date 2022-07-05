import React, { useEffect } from "react";
import { withRouter } from "react-router";
import ModalDialog from "@appserver/components/modal-dialog";
import { StyledDeleteDialog } from "./StyledDeleteDialog";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";
import toastr from "studio/toastr";
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
    isRootFolder,
  } = props;

  const selection = [];
  let i = 0;

  while (props.selection.length !== i) {
    const item = props.selection[i];
    if (!((isRootFolder && item.providerKey) || item.isEditing)) {
      if (item.access === 0 || item.access === 1 || unsubscribe) {
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
      FileRemoved: t("Home:FileRemoved"),
      FolderRemoved: t("Home:FolderRemoved"),
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

  const onClose = () => {
    setBufferSelection(null);
    setRemoveMediaItem(null);
    setDeleteDialogVisible(false);
  };

  const moveToTrashTitle = () => {
    if (unsubscribe) {
      return t("UnsubscribeTitle");
    } else {
      if (selection.length > 1) {
        return t("MoveToTrashItemsTitle");
      } else {
        return selection[0].isFolder
          ? t("MoveToTrashOneFolderTitle")
          : t("MoveToTrashOneFileTitle");
      }
    }
  };

  const moveToTrashNoteText = () => {
    if (selection.length > 1) {
      return t("MoveToTrashItemsNote");
    } else {
      return !selection[0].isFolder
        ? t("MoveToTrashOneFileNote")
        : personal
        ? ""
        : t("MoveToTrashOneFolderNote");
    }
  };

  const title =
    isPrivacyFolder || isRecycleBinFolder || selection[0]?.providerKey
      ? t("Common:Confirmation")
      : moveToTrashTitle();

  const noteText = unsubscribe ? t("UnsubscribeNote") : moveToTrashNoteText();

  const accessButtonLabel =
    isPrivacyFolder || isRecycleBinFolder || selection[0]?.providerKey
      ? t("Common:OKButton")
      : unsubscribe
      ? t("UnsubscribeButton")
      : t("MoveToTrashButton");

  return (
    <StyledDeleteDialog isLoading={!tReady} visible={visible} onClose={onClose}>
      <ModalDialog.Header>{title}</ModalDialog.Header>
      <ModalDialog.Body>
        <div className="modal-dialog-content-body">
          <Text className="delete_dialog-header-text" noSelect>
            {noteText}
          </Text>
        </div>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          className="button-dialog-accept"
          key="OkButton"
          label={accessButtonLabel}
          size="normalTouchscreen"
          primary
          onClick={unsubscribe ? onUnsubscribe : onDelete}
          isLoading={isLoading}
          isDisabled={!selection.length}
        />
        <Button
          className="button-dialog"
          key="CancelButton"
          label={t("Common:CancelButton")}
          size="normalTouchscreen"
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
])(DeleteDialogComponent);

export default inject(
  ({
    filesStore,
    selectedFolderStore,
    dialogsStore,
    filesActionsStore,
    treeFoldersStore,
    auth,
  }) => {
    const {
      selection,
      isLoading,
      bufferSelection,
      setBufferSelection,
    } = filesStore;
    const { deleteAction, unsubscribeAction } = filesActionsStore;
    const { isPrivacyFolder, isRecycleBinFolder } = treeFoldersStore;

    const {
      deleteDialogVisible: visible,
      setDeleteDialogVisible,
      removeMediaItem,
      setRemoveMediaItem,
      unsubscribe,
    } = dialogsStore;

    const { personal } = auth.settingsStore;

    return {
      selection: removeMediaItem
        ? [removeMediaItem]
        : selection.length
        ? selection
        : [bufferSelection],
      isLoading,
      isRootFolder: selectedFolderStore.isRootFolder,
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
    };
  }
)(withRouter(observer(DeleteDialog)));
