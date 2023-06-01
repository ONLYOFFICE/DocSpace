import React, { useEffect } from "react";
import styled from "styled-components";

import ModalDialogContainer from "../ModalDialogContainer";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import ModalDialog from "@docspace/components/modal-dialog";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

const StyledModal = styled(ModalDialogContainer)`
  max-width: 400px;

  .cancel-btn {
    display: inline-block;
    margin-left: 8px;
  }
`;

const ArchiveDialogComponent = (props) => {
  const {
    t,
    tReady,
    visible,
    restoreAll,
    setArchiveDialogVisible,
    setArchiveAction,
    items,
  } = props;

  useEffect(() => {
    window.addEventListener("keydown", onKeyPress);

    return () => window.removeEventListener("keydown", onKeyPress);
  }, []);

  const onClose = () => {
    setArchiveDialogVisible(false);
  };

  const onAction = () => {
    setArchiveDialogVisible(false);
    setArchiveAction("archive", items, t);
  };

  const onKeyPress = (e) => {
    if (e.keyCode === 13) {
      onAction();
    }
  };

  const getDescription = () => {
    if (restoreAll) return t("ArchiveDialog:RestoreAllRooms");

    return items.length > 1
      ? `${t("ArchiveDialog:ArchiveRooms")} ${t("Common:WantToContinue")}`
      : `${t("ArchiveDialog:ArchiveRoom")} ${t("Common:WantToContinue")}`;
  };

  const description = getDescription();

  return (
    <StyledModal
      isLoading={!tReady}
      visible={visible}
      onClose={onClose}
      displayType="modal"
    >
      <ModalDialog.Header>
        {t("ArchiveDialog:ArchiveHeader")}
      </ModalDialog.Header>
      <ModalDialog.Body>
        <Text noSelect={true}>{description}</Text>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          id="shared_move-to-archived-modal_submit"
          key="OkButton"
          label={t("Common:OKButton")}
          size="normal"
          primary
          onClick={onAction}
          scale
        />
        <Button
          id="shared_move-to-archived-modal_cancel"
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

const ArchiveDialog = withTranslation(["Files", "ArchiveDialog", "Common"])(
  ArchiveDialogComponent
);

export default inject(
  ({ filesStore, filesActionsStore, dialogsStore, selectedFolderStore }) => {
    const { roomsForRestore, selection, bufferSelection } = filesStore;
    const { setArchiveAction } = filesActionsStore;

    const {
      archiveDialogVisible: visible,
      restoreAllArchive: restoreAll,
      setArchiveDialogVisible,
    } = dialogsStore;

    const items = restoreAll
      ? roomsForRestore
      : selection.length > 0
      ? selection
      : bufferSelection
      ? [bufferSelection]
      : [{ id: selectedFolderStore.id }];

    return {
      visible,
      restoreAll,
      setArchiveDialogVisible,
      setArchiveAction,
      items,
    };
  }
)(observer(ArchiveDialog));
