import React, { useState, useEffect } from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { Button } from "@docspace/components";

const CreateRoomConfirmDialog = ({
  t,

  visible,
  setVisible,

  confirmDialogIsLoading,
  onCreateRoom,
}) => {
  const onContinue = async () => {
    await onCreateRoom(true);
    onClose();
  };

  const onClose = () => setVisible(false);

  return (
    <ModalDialog
      visible={visible || confirmDialogIsLoading}
      onClose={onClose}
      isLarge
      zIndex={310}
    >
      <ModalDialog.Header>{t("Common:Warning")}</ModalDialog.Header>
      <ModalDialog.Body>
        {t("CreateEditRoomDialog:CreateRoomConfirmation")}
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          label={t("Common:ContinueButton")}
          size="normal"
          primary
          isLoading={confirmDialogIsLoading}
          onClick={onContinue}
        />
        <Button
          label={t("Common:CancelButton")}
          size="normal"
          onClick={onClose}
        />
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

export default inject(({ dialogsStore, createEditRoomStore }) => {
  const {
    createRoomConfirmDialogVisible: visible,
    setCreateRoomConfirmDialogVisible: setVisible,
  } = dialogsStore;

  const { confirmDialogIsLoading, onCreateRoom } = createEditRoomStore;

  return {
    visible,
    setVisible,

    confirmDialogIsLoading,
    onCreateRoom,
  };
})(withTranslation(["Common, Files"])(observer(CreateRoomConfirmDialog)));
