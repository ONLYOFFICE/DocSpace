import React, { useEffect } from "react";
import styled from "styled-components";
import ModalDialogContainer from "../ModalDialogContainer";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import ModalDialog from "@docspace/components/modal-dialog";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { RoomsType } from "@docspace/common/constants";

const StyledModal = styled(ModalDialogContainer)`
  max-width: 400px;

  .cancel-btn {
    display: inline-block;
    margin-left: 8px;
  }
`;

const RestoreRoomDialogComponent = (props) => {
  const {
    t,
    tReady,
    visible,
    restoreAll,
    setRestoreRoomDialogVisible,
    setRestoreAllArchive,
    setArchiveAction,
    items,
    hasPublicRoom,
  } = props;

  useEffect(() => {
    window.addEventListener("keydown", onKeyPress);

    return () => window.removeEventListener("keydown", onKeyPress);
  }, []);

  const onClose = () => {
    setRestoreAllArchive(false);
    setRestoreRoomDialogVisible(false);
  };

  const onAction = () => {
    const itemsRestoreHaveRights = items.filter(
      (item) => item.security.Move === true
    );

    setRestoreRoomDialogVisible(false);
    setArchiveAction("unarchive", itemsRestoreHaveRights, t).then(() => {
      setRestoreAllArchive(false);
    });
  };

  const onKeyPress = (e) => {
    if (e.keyCode === 13) {
      onAction();
    }
  };

  const getDescription = () => {
    if (hasPublicRoom) {
      return items.length > 1
        ? t("Files:WantToRestoreTheRooms")
        : t("Files:WantToRestoreTheRoom");
    }

    if (restoreAll) return t("ArchiveDialog:RestoreAllRooms");

    return items.length > 1
      ? t("ArchiveDialog:RestoreRooms")
      : t("ArchiveDialog:RestoreRoom");
  };

  const description = getDescription();

  return (
    <StyledModal
      isLoading={!tReady}
      visible={visible}
      onClose={onClose}
      displayType="modal"
    >
      <ModalDialog.Header>{t("Common:Restore")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text noSelect={true}>{description}</Text>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          id="restore-all_submit"
          key="OkButton"
          label={t("Common:Restore")}
          size="normal"
          primary
          onClick={onAction}
          scale
        />
        <Button
          id="restore-all_cancel"
          key="CancelButton"
          label={t("Common:CancelButton")}
          size="normal"
          onClick={onClose}
          scale
        />
      </ModalDialog.Footer>
    </StyledModal>
  );
};

const RestoreRoomDialog = withTranslation(["Files", "ArchiveDialog", "Common"])(
  RestoreRoomDialogComponent
);

export default inject(
  ({ filesStore, filesActionsStore, dialogsStore, selectedFolderStore }) => {
    const { roomsForRestore, selection, bufferSelection } = filesStore;
    const { setArchiveAction } = filesActionsStore;

    const {
      restoreRoomDialogVisible: visible,
      restoreAllArchive: restoreAll,
      setRestoreRoomDialogVisible,
      setRestoreAllArchive,
    } = dialogsStore;

    const items = restoreAll
      ? roomsForRestore
      : selection.length > 0
      ? selection
      : bufferSelection
      ? [bufferSelection]
      : [selectedFolderStore];

    const hasPublicRoom =
      items.findIndex((i) => i.roomType === RoomsType.PublicRoom) !== -1;

    return {
      visible,
      restoreAll,
      setRestoreRoomDialogVisible,
      setRestoreAllArchive,
      setArchiveAction,
      items,
      hasPublicRoom,
    };
  }
)(observer(RestoreRoomDialog));
