import React, { useEffect } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import ModalDialogContainer from "../ModalDialogContainer";
import Text from "@appserver/components/text";
import Button from "@appserver/components/button";
import ModalDialog from "@appserver/components/modal-dialog";
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
      successOperation: t("SuccessEmptyTrash"),
    };
    emptyTrash(translations);
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
        <Text>{t("DeleteForeverNote")}</Text>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="OkButton"
          label={t("DeleteForeverButton")}
          size="small"
          primary
          onClick={onEmptyTrash}
          isLoading={isLoading}
        />
        <Button
          className="cancel-btn"
          key="CancelButton"
          label={t("Common:CancelButton")}
          size="small"
          onClick={onClose}
          isLoading={isLoading}
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

export default inject(({ filesStore, filesActionsStore, dialogsStore }) => {
  const { fetchFiles, filter, isLoading } = filesStore;
  const { emptyTrash } = filesActionsStore;

  const {
    emptyTrashDialogVisible: visible,
    setEmptyTrashDialogVisible,
  } = dialogsStore;

  return {
    isLoading,
    filter,
    visible,

    fetchFiles,
    setEmptyTrashDialogVisible,
    emptyTrash,
  };
})(withRouter(observer(EmptyTrashDialog)));
