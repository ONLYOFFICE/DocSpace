import React from "react";
import styled from "styled-components";

import { withTranslation } from "react-i18next";
import ModalDialog from "@appserver/components/modal-dialog";
import Text from "@appserver/components/text";
import Button from "@appserver/components/button";
import { inject, observer } from "mobx-react";

const StyledOperationDialog = styled(ModalDialog)`
  .operation-button {
    margin-right: 8px;
  }
`;

const PureThirdPartyMoveContainer = ({
  t,
  tReady,
  visible,
  provider,
  selection,
  destFolderId,
  setDestFolderId,
  checkOperationConflict,
  setThirdPartyMoveDialogVisible,
}) => {
  const zIndex = 310;
  const deleteAfter = false; // TODO: get from settings

  const onClose = () => {
    setDestFolderId(false);
    setThirdPartyMoveDialogVisible(false);
  };

  const startOperation = (e) => {
    const isCopy = e.target.dataset.copy;
    const folderIds = [];
    const fileIds = [];

    for (let item of selection) {
      if (item.fileExst) {
        fileIds.push(item.id);
      } else {
        folderIds.push(item.id);
      }
    }

    const data = {
      destFolderId,
      folderIds,
      fileIds,
      deleteAfter,
      isCopy,
      translations: {
        copy: t("Translations:CopyOperation"),
        move: t("Translations:MoveToOperation"),
      },
    };

    checkOperationConflict(data);
    onClose();
  };

  return (
    <StyledOperationDialog
      isLoading={!tReady}
      visible={visible}
      zIndex={zIndex}
      onClose={onClose}
    >
      <ModalDialog.Header>{t("MoveConfirmation")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text>{t("MoveConfirmationMessage", { provider })}</Text>
        <br />
        <Text>{t("MoveConfirmationAlert")}</Text>
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Button
          className="operation-button"
          label={t("Translations:Move")}
          size="big"
          primary
          onClick={startOperation}
        />
        <Button
          data-copy="copy"
          className="operation-button"
          label={t("Translations:Copy")}
          size="big"
          onClick={startOperation}
        />
        <Button
          className="operation-button"
          label={t("Common:CancelButton")}
          size="big"
          onClick={onClose}
        />
      </ModalDialog.Footer>
    </StyledOperationDialog>
  );
};

export default inject(({ filesStore, dialogsStore, filesActionsStore }) => {
  const {
    thirdPartyMoveDialogVisible: visible,
    setThirdPartyMoveDialogVisible,
    destFolderId,
    setDestFolderId,
  } = dialogsStore;
  const { selection } = filesStore;
  const { checkOperationConflict } = filesActionsStore;

  return {
    visible,
    setThirdPartyMoveDialogVisible,
    destFolderId,
    setDestFolderId,
    provider: selection[0].providerKey,
    checkOperationConflict,
    selection,
  };
})(
  withTranslation(["ThirdPartyMoveDialog", "Common", "Translations"])(
    observer(PureThirdPartyMoveContainer)
  )
);
