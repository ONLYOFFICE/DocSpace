import React from "react";
import { inject, observer } from "mobx-react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import InputBlock from "@docspace/components/input-block";
import TextInput from "@docspace/components/text-input";
import copy from "copy-to-clipboard";
import toastr from "@docspace/components/toast/toastr";

const StyledDeleteDialog = styled(ModalDialog)`
  .modal-dialog-content-body {
    display: flex;
    gap: 12px;

    .modal-dialog-content-body_text-input {
      color: #a3a9ae;
    }
  }

  .modal-dialog-aside-header {
    margin: 0 -24px 0 -16px;
    padding: 0 0 0 16px;
  }
`;

const RoomSharingDialog = ({ t, tReady, visible, setIsVisible }) => {
  const roomHref = window.location.href;

  const onClose = () => {
    setIsVisible(false);
  };

  const onCopy = () => {
    copy(roomHref);
    toastr.success(t("Files:LinkSuccessfullyCopied"));
    onClose();
  };

  return (
    <StyledDeleteDialog isLoading={!tReady} visible={visible} onClose={onClose}>
      <ModalDialog.Header>{t("Files:ShareRoom")}</ModalDialog.Header>
      <ModalDialog.Body>
        <div className="modal-dialog-content-body">
          <TextInput
            className="modal-dialog-content-body_text-input"
            value={roomHref}
            scale
            isReadOnly
            isDisabled
          />
          <Button
            label={t("Translations:Copy")}
            size="small"
            onClick={onCopy}
            isLoading={!tReady}
          />
        </div>
      </ModalDialog.Body>
    </StyledDeleteDialog>
  );
};

export default inject(({ dialogsStore }) => {
  const { setRoomSharingPanelVisible, roomSharingPanelVisible } = dialogsStore;

  return {
    visible: roomSharingPanelVisible,
    setIsVisible: setRoomSharingPanelVisible,
  };
})(
  withTranslation(["Files", "Common", "Translations"])(
    observer(RoomSharingDialog)
  )
);
