import React, { useEffect } from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import Text from "@docspace/components/text";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

const LeaveRoomDialog = (props) => {
  const {
    t,
    tReady,
    visible,
    setIsVisible,
    setChangeRoomOwnerIsVisible,
  } = props;

  useEffect(() => {
    document.addEventListener("keyup", onKeyUp, false);

    return () => {
      document.removeEventListener("keyup", onKeyUp, false);
    };
  }, []);

  const onKeyUp = (e) => {
    if (e.keyCode === 27) onClose();
    if (e.keyCode === 13 || e.which === 13) onLeaveRoom();
  };

  const onLeaveRoom = () => {
    onClose();
    setChangeRoomOwnerIsVisible(true);
  };

  const onClose = () => setIsVisible(false);

  return (
    <ModalDialog isLoading={!tReady} visible={visible} onClose={onClose}>
      <ModalDialog.Header>{t("Files:LeaveTheRoom")}</ModalDialog.Header>
      <ModalDialog.Body>
        <div className="modal-dialog-content-body">
          <Text noSelect>{t("Files:LeaveRoomDescription")}</Text>
        </div>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="OkButton"
          label={t("Common:OKButton")}
          size="normal"
          primary
          scale
          onClick={onLeaveRoom}
        />
        <Button
          key="CancelButton"
          label={t("Common:CancelButton")}
          size="normal"
          scale
          onClick={onClose}
        />
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

export default inject(({ dialogsStore }) => {
  const {
    leaveRoomDialogVisible: visible,
    setLeaveRoomDialogVisible: setIsVisible,
    setChangeRoomOwnerIsVisible,
  } = dialogsStore;

  return {
    visible,
    setIsVisible,
    setChangeRoomOwnerIsVisible,
  };
})(observer(withTranslation(["Common", "Files"])(LeaveRoomDialog)));
