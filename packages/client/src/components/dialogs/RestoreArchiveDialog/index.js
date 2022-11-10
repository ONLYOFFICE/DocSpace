import React, { useEffect } from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
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

const RestoreArchiveDialogComponent = (props) => {
  const {
    visible,
    t,
    tReady,

    setRestoreArchiveDialogVisible,

    setArchiveAction,
    folders,
  } = props;

  const [requestRunning, setRequestRunning] = React.useState(false);

  useEffect(() => {
    window.addEventListener("keydown", onKeyPress);

    return () => window.removeEventListener("keydown", onKeyPress);
  }, []);

  const onClose = () => {
    !requestRunning && setRestoreArchiveDialogVisible(false);
  };

  const onRestore = () => {
    setRequestRunning(true);

    setArchiveAction("unarchive", folders, t).then(() => {
      setRequestRunning(false);
      onClose();
    });
  };

  const onKeyPress = (e) => {
    if (e.keyCode === 13) {
      onRestore();
    }
  };

  return (
    <StyledModal
      isLoading={!tReady}
      visible={visible}
      onClose={onClose}
      displayType="modal"
    >
      <ModalDialog.Header>{t("Common:Restore")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text>{t("RestoreAllArchive")}</Text>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="OkButton"
          label={t("Common:Restore")}
          size="normal"
          primary
          onClick={onRestore}
          isLoading={requestRunning}
          scale
        />
        <Button
          key="CancelButton"
          label={t("Common:CancelButton")}
          size="normal"
          onClick={onClose}
          isDisabled={requestRunning}
          scale
        />
      </ModalDialog.Footer>
    </StyledModal>
  );
};

const RestoreArchiveDialog = withTranslation([
  "Files",
  "Common",
  "Translations",
])(RestoreArchiveDialogComponent);

export default inject(({ filesStore, filesActionsStore, dialogsStore }) => {
  const { folders } = filesStore;
  const { setArchiveAction } = filesActionsStore;

  const {
    restoreArchiveDialogVisible: visible,
    setRestoreArchiveDialogVisible,
  } = dialogsStore;

  return {
    visible,

    setRestoreArchiveDialogVisible,

    setArchiveAction,
    folders,
  };
})(withRouter(observer(RestoreArchiveDialog)));
