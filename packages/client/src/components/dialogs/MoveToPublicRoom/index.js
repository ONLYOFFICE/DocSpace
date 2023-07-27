import React, { useEffect, useState } from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import { StyledMoveToPublicRoomDialog } from "./StyledMoveToPublicRoomDialog";
import Button from "@docspace/components/button";
import Text from "@docspace/components/text";
import { withTranslation } from "react-i18next";
import toastr from "@docspace/components/toast/toastr";
import { inject, observer } from "mobx-react";

let timerId;

const MoveToPublicRoomComponent = (props) => {
  const {
    t,
    tReady,
    visible,
    setIsVisible,
    setConflictResolveDialogVisible,
    setMoveToPanelVisible,
    setCopyPanelVisible,
    setRestoreAllPanelVisible,
    moveToPublicRoomData,
    checkFileConflicts,
    setConflictDialogData,
    setMovingInProgress,
    itemOperationToFolder,
    clearActiveOperations,
  } = props;

  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    document.addEventListener("keyup", onKeyUp, false);

    return () => {
      document.removeEventListener("keyup", onKeyUp, false);
    };
  }, []);

  useEffect(() => {
    return () => {
      clearTimeout(timerId);
      timerId = null;
    };
  });

  const onKeyUp = (e) => {
    if (e.keyCode === 27) onClose();
    if (e.keyCode === 13 || e.which === 13) onMoveTo();
  };

  const onClosePanels = () => {
    setIsVisible(false);
    setConflictResolveDialogVisible(false);
    setMoveToPanelVisible(false);
    setCopyPanelVisible(false);
    setRestoreAllPanelVisible(false);
  };

  const onClose = () => {
    setIsVisible(false);
  };

  const onMoveTo = () => {
    const { destFolderId, folderIds, fileIds, isCopy } = moveToPublicRoomData;

    if (!timerId)
      timerId = setTimeout(() => {
        setIsLoading(true);
      }, 500);

    checkFileConflicts(destFolderId, folderIds, fileIds)
      .then(async (conflicts) => {
        if (conflicts.length) {
          setConflictDialogData(conflicts, moveToPublicRoomData);
          setIsLoading(false);
        } else {
          setIsLoading(false);
          onClosePanels();
          const move = !isCopy;
          if (move) setMovingInProgress(move);
          await itemOperationToFolder(moveToPublicRoomData);
        }
      })
      .catch((e) => {
        toastr.error(e);
        setIsLoading(false);
        clearActiveOperations(fileIds, folderIds);
      })
      .finally(() => {
        clearTimeout(timerId);
        timerId = null;
      });
  };

  return (
    <StyledMoveToPublicRoomDialog
      isLoading={!tReady}
      visible={visible}
      onClose={onClose}
    >
      <ModalDialog.Header>
        {t("Files:MoveToPublicRoomTitle")}
      </ModalDialog.Header>
      <ModalDialog.Body>
        <div className="modal-dialog-content-body">
          <Text noSelect>{t("Files:MoveToPublicRoom")}</Text>
        </div>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          id="delete-file-modal_submit"
          key="OkButton"
          label={t("Common:OKButton")}
          size="normal"
          primary
          scale
          onClick={onMoveTo}
          isLoading={isLoading}
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
    </StyledMoveToPublicRoomDialog>
  );
};

const MoveToPublicRoomDialog = withTranslation([
  "Common",
  "Translations",
  "Files",
  "EmptyTrashDialog",
])(MoveToPublicRoomComponent);

export default inject(
  ({ dialogsStore, filesActionsStore, filesStore, uploadDataStore }) => {
    const { setMovingInProgress } = filesStore;

    const {
      moveToPublicRoomVisible,
      setMoveToPublicRoomVisible,
      setConflictResolveDialogVisible,
      setMoveToPanelVisible,
      setCopyPanelVisible,
      setRestoreAllPanelVisible,
      moveToPublicRoomData,
    } = dialogsStore;

    const { setConflictDialogData, checkFileConflicts } = filesActionsStore;
    const { itemOperationToFolder, clearActiveOperations } = uploadDataStore;

    return {
      visible: moveToPublicRoomVisible,
      setIsVisible: setMoveToPublicRoomVisible,
      setConflictResolveDialogVisible,
      setMoveToPanelVisible,
      setCopyPanelVisible,
      setRestoreAllPanelVisible,
      moveToPublicRoomData,
      checkFileConflicts,
      setConflictDialogData,
      setMovingInProgress,
      itemOperationToFolder,
      clearActiveOperations,
    };
  }
)(observer(MoveToPublicRoomDialog));
