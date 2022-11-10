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

const ArchiveDialogComponent = (props) => {
  const {
    t,
    tReady,

    visible,
    restoreAll,
    action,

    setArchiveDialogVisible,
    setRestoreAllArchive,
    setArchiveActionType,

    setArchiveAction,
    items,
  } = props;

  const [requestRunning, setRequestRunning] = React.useState(false);

  useEffect(() => {
    window.addEventListener("keydown", onKeyPress);

    return () => window.removeEventListener("keydown", onKeyPress);
  }, []);

  const onClose = () => {
    if (!requestRunning) {
      setRestoreAllArchive(false);
      setArchiveActionType(null);
      setArchiveDialogVisible(false);
    }
  };

  const onAction = () => {
    setRequestRunning(true);

    setArchiveAction(action, items, t).then(() => {
      setRequestRunning(false);
      onClose();
    });
  };

  const onKeyPress = (e) => {
    if (e.keyCode === 13) {
      onAction();
    }
  };

  const getDescription = () => {
    if (restoreAll) return t("ArchiveDialog:RestoreAllRooms");

    if (action === "archive") {
      return items.length > 1
        ? `${t("ArchiveDialog:ArchiveRooms")} ${t("Common:WantToContinue")}`
        : `${t("ArchiveDialog:ArchiveRoom")} ${t("Common:WantToContinue")}`;
    }

    if (action === "unarchive") {
      return items.length > 1
        ? t("ArchiveDialog:RestoreRooms")
        : t("ArchiveDialog:RestoreRoom");
    }
  };

  const header =
    action === "archive"
      ? t("ArchiveDialog:ArchiveHeader")
      : t("Common:Restore");
  const description = getDescription();
  const acceptButton =
    action === "archive" ? t("Common:OKButton") : t("Common:Restore");

  return (
    <StyledModal
      isLoading={!tReady}
      visible={visible}
      onClose={onClose}
      displayType="modal"
    >
      <ModalDialog.Header>{header}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text>{description}</Text>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="OkButton"
          label={acceptButton}
          size="normal"
          primary
          onClick={onAction}
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

const ArchiveDialog = withTranslation(["Files", "ArchiveDialog", "Common"])(
  ArchiveDialogComponent
);

export default inject(({ filesStore, filesActionsStore, dialogsStore }) => {
  const { folders, selection, bufferSelection } = filesStore;
  const { setArchiveAction } = filesActionsStore;

  const {
    archiveDialogVisible: visible,
    restoreAllArchive: restoreAll,
    archiveAction: action,

    setArchiveDialogVisible,
    setRestoreAllArchive,
    setArchiveAction: setArchiveActionType,
  } = dialogsStore;

  const items = restoreAll
    ? folders
    : selection.length > 0
    ? selection
    : [bufferSelection];

  return {
    visible,
    restoreAll,
    action,

    setArchiveDialogVisible,
    setRestoreAllArchive,
    setArchiveActionType,

    setArchiveAction,
    items,
  };
})(withRouter(observer(ArchiveDialog)));
