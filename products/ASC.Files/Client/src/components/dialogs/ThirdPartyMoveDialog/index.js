import React from "react";
import styled from "styled-components";

import { withTranslation } from "react-i18next";
import ModalDialog from "@appserver/components/modal-dialog";
import Text from "@appserver/components/text";
import Button from "@appserver/components/button";
import { inject, observer } from "mobx-react";

const StyledOperationDialog = styled.div`
  .operation-button {
    margin-right: 8px;
  }
`;

const PureThirdPartyMoveContainer = ({
  t,
  visible,
  provider,
  selection,
  destFolderId,
  copyToAction,
  moveToAction,
  setDestFolderId,
  setThirdPartyMoveDialogVisible,
}) => {
  const zIndex = 310;
  const conflictResolveType = 0; //Skip = 0, Overwrite = 1, Duplicate = 2 TODO: get from settings
  const deleteAfter = true; // TODO: get from settings

  const onClose = () => {
    setDestFolderId(false);
    setThirdPartyMoveDialogVisible(false);
  };

  const getOperationItems = () => {
    const folderIds = [];
    const fileIds = [];

    for (let item of selection) {
      if (item.fileExst) {
        fileIds.push(item.id);
      } else {
        folderIds.push(item.id);
      }
    }
    return [folderIds, fileIds];
  };

  const startMoveOperation = () => {
    const result = getOperationItems();
    const folderIds = result[0];
    const fileIds = result[1];

    moveToAction(
      destFolderId,
      folderIds,
      fileIds,
      conflictResolveType,
      deleteAfter
    );
    onClose();
  };

  const startCopyOperation = () => {
    const result = getOperationItems();
    const folderIds = result[0];
    const fileIds = result[1];

    copyToAction(
      destFolderId,
      folderIds,
      fileIds,
      conflictResolveType,
      deleteAfter
    );
    onClose();
  };

  return (
    <StyledOperationDialog>
      <ModalDialog visible={visible} zIndex={zIndex} onClose={onClose}>
        <ModalDialog.Header>{t("MoveConfirmation")}</ModalDialog.Header>
        <ModalDialog.Body>
          <Text>{t("MoveConfirmationMessage", { provider })}</Text>
          <br />
          <Text>{t("MoveConfirmationAlert")}</Text>
        </ModalDialog.Body>

        <ModalDialog.Footer>
          <Button
            className="operation-button"
            label={t("Move")}
            size="big"
            primary
            onClick={startMoveOperation}
          />
          <Button
            className="operation-button"
            label={t("Copy")}
            size="big"
            onClick={startCopyOperation}
          />
          <Button
            className="operation-button"
            label={t("CancelButton")}
            size="big"
            onClick={onClose}
          />
        </ModalDialog.Footer>
      </ModalDialog>
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
  const { copyToAction, moveToAction } = filesActionsStore;

  return {
    visible,
    setThirdPartyMoveDialogVisible,
    destFolderId,
    setDestFolderId,
    provider: selection[0].providerKey,
    copyToAction,
    moveToAction,
    selection,
  };
})(
  withTranslation("ThirdPartyMoveDialog")(observer(PureThirdPartyMoveContainer))
);
