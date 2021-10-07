import React from "react";
import { withRouter } from "react-router";
import ModalDialogContainer from "../ModalDialogContainer";
import Text from "@appserver/components/text";
import Button from "@appserver/components/button";
import ModalDialog from "@appserver/components/modal-dialog";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

const EmptyTrashDialogComponent = (props) => {
  const {
    visible,
    t,
    tReady,
    isLoading,
    setEmptyTrashDialogVisible,
    emptyTrash,
  } = props;

  const onClose = () => setEmptyTrashDialogVisible(false);

  const onEmptyTrash = () => {
    onClose();
    const translations = { deleteOperation: t("Translations:DeleteOperation") };
    emptyTrash(translations);
  };

  return (
    <ModalDialogContainer
      isLoading={!tReady}
      visible={visible}
      onClose={onClose}
    >
      <ModalDialog.Header>{t("DeleteForeverTitle")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text>{t("DeleteForeverNote")}</Text>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="OkButton"
          label={t("DeleteForeverButton")}
          size="medium"
          primary
          onClick={onEmptyTrash}
          isLoading={isLoading}
        />
        <Button
          className="button-dialog"
          key="CancelButton"
          label={t("Common:CancelButton")}
          size="medium"
          onClick={onClose}
          isLoading={isLoading}
        />
      </ModalDialog.Footer>
    </ModalDialogContainer>
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
