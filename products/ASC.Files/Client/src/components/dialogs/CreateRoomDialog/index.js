import React from "react";
import { inject, observer } from "mobx-react";
import ModalDialog from "@appserver/components/modal-dialog";
import { withTranslation, Trans } from "react-i18next";

const CreateRoomDialog = ({ visible, setCreateRoomDialogVisible }) => {
  const onClose = () => setCreateRoomDialogVisible(false);

  return (
    <ModalDialog displayType="aside" visible={visible} onClose={onClose}>
      <ModalDialog.Header>Create room</ModalDialog.Header>
      <ModalDialog.Body>Create room</ModalDialog.Body>
    </ModalDialog>
  );
};

export default inject(({ dialogsStore }) => {
  const {
    createRoomDialogVisible: visible,
    setCreateRoomDialogVisible,
  } = dialogsStore;

  return {
    visible,
    setCreateRoomDialogVisible,
  };
})(withTranslation(["CreateRoomDialog", "Common"])(observer(CreateRoomDialog)));
