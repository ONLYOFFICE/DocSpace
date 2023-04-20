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

const EmptyTrashDialogComponent = (props) => {
  const {
    visible,
    t,
    tReady,
    isLoading,
    setEmptyTrashDialogVisible,
    emptyTrash,
    emptyArchive,

    isArchiveFolder,
  } = props;

  useEffect(() => {
    window.addEventListener("keydown", onKeyPress);

    return () => window.removeEventListener("keydown", onKeyPress);
  }, []);

  const onClose = () => setEmptyTrashDialogVisible(false);

  const onEmptyTrash = () => {
    onClose();
    const translations = {
      deleteOperation: t("Translations:DeleteOperation"),
      successOperation: isArchiveFolder
        ? t("SuccessEmptyArchived")
        : t("SuccessEmptyTrash"),
    };

    if (isArchiveFolder) {
      emptyArchive(translations);
    } else {
      emptyTrash(translations);
    }
  };

  const onKeyPress = (e) => {
    if (e.keyCode === 13) {
      onEmptyTrash();
    }
  };

  return (
    <StyledModal
      isLoading={!tReady}
      visible={visible}
      onClose={onClose}
      displayType="modal"
    >
      <ModalDialog.Header>{t("DeleteForeverTitle")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text noSelect>
          {isArchiveFolder
            ? t("DeleteForeverNoteArchive")
            : t("DeleteForeverNote")}
        </Text>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          id="empty-archive_delete-submit"
          key="OkButton"
          label={t("DeleteForeverButton")}
          size="normal"
          primary
          onClick={onEmptyTrash}
          isLoading={isLoading}
          scale
        />
        <Button
          id="empty-archive_delete-cancel"
          key="CancelButton"
          label={t("Common:CancelButton")}
          size="normal"
          onClick={onClose}
          isLoading={isLoading}
          scale
        />
      </ModalDialog.Footer>
    </StyledModal>
  );
};

const EmptyTrashDialog = withTranslation([
  "EmptyTrashDialog",
  "Common",
  "Translations",
])(EmptyTrashDialogComponent);

export default inject(
  ({ filesStore, filesActionsStore, treeFoldersStore, dialogsStore }) => {
    const { isLoading } = filesStore;
    const { emptyTrash, emptyArchive } = filesActionsStore;

    const { isArchiveFolder } = treeFoldersStore;

    const { emptyTrashDialogVisible: visible, setEmptyTrashDialogVisible } =
      dialogsStore;

    return {
      isLoading,

      visible,

      setEmptyTrashDialogVisible,
      emptyTrash,
      emptyArchive,

      isArchiveFolder,
    };
  }
)(observer(EmptyTrashDialog));
