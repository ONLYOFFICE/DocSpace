import React, { useState } from "react";
import styled from "styled-components";

import { withTranslation } from "react-i18next";
import ModalDialog from "@appserver/components/modal-dialog";
import Text from "@appserver/components/text";
import Button from "@appserver/components/button";
import { inject, observer } from "mobx-react";
import toastr from "@appserver/components/toast/toastr";
import { connectedCloudsTypeTitleTranslation } from "../../../helpers/utils";

const StyledOperationDialog = styled(ModalDialog)`
  .operation-button {
    margin-right: 8px;
  }

  .modal-dialog-aside-footer {
    display: flex;
    width: 90%;
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
  checkFileConflicts,
  setConflictDialogData,
  setThirdPartyMoveDialogVisible,
  setBufferSelection,
  itemOperationToFolder,
  setMoveToPanelVisible,
}) => {
  const zIndex = 310;
  const deleteAfter = false; // TODO: get from settings

  const [isLoading, setIsLoading] = useState(false);

  const onClose = () => {
    setDestFolderId(false);
    setThirdPartyMoveDialogVisible(false);
    setMoveToPanelVisible(false);
    setBufferSelection(null);
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
        copy: t("Common:CopyOperation"),
        move: t("Translations:MoveToOperation"),
      },
    };

    setIsLoading(true);
    checkFileConflicts(destFolderId, folderIds, fileIds)
      .then(async (conflicts) => {
        if (conflicts.length) {
          setConflictDialogData(conflicts, data);
          setIsLoading(false);
        } else {
          setIsLoading(false);
          onClose();
          await itemOperationToFolder(data);
        }
      })
      .catch((err) => toastr.error(err))
      .finally(() => {
        setIsLoading(false);
      });
  };

  const providerTitle = connectedCloudsTypeTitleTranslation(provider, t);

  return (
    <StyledOperationDialog
      isLoading={!tReady}
      visible={visible}
      zIndex={zIndex}
      onClose={onClose}
      displayType="aside"
    >
      <ModalDialog.Header>{t("MoveConfirmation")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text>{t("MoveConfirmationMessage", { provider: providerTitle })}</Text>
        <br />
        <Text>{t("MoveConfirmationAlert")}</Text>
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Button
          className="operation-button"
          label={t("Translations:Move")}
          size="normal"
          scale
          primary
          onClick={startOperation}
          isLoading={isLoading}
          isDisabled={isLoading}
        />
        <Button
          data-copy="copy"
          className="operation-button"
          label={t("Translations:Copy")}
          size="normal"
          scale
          onClick={startOperation}
          isLoading={isLoading}
          isDisabled={isLoading}
        />
        <Button
          className="operation-button"
          label={t("Common:CancelButton")}
          size="normal"
          scale
          onClick={onClose}
          isLoading={isLoading}
          isDisabled={isLoading}
        />
      </ModalDialog.Footer>
    </StyledOperationDialog>
  );
};

export default inject(
  ({ filesStore, dialogsStore, filesActionsStore, uploadDataStore }) => {
    const {
      thirdPartyMoveDialogVisible: visible,
      setThirdPartyMoveDialogVisible,
      destFolderId,
      setDestFolderId,
      setMoveToPanelVisible,
    } = dialogsStore;
    const { bufferSelection, setBufferSelection } = filesStore;
    const { checkFileConflicts, setConflictDialogData } = filesActionsStore;
    const { itemOperationToFolder } = uploadDataStore;

    const selection = filesStore.selection.length
      ? filesStore.selection
      : [bufferSelection];

    return {
      visible,
      setThirdPartyMoveDialogVisible,
      destFolderId,
      setDestFolderId,
      provider: selection[0].providerKey,
      checkFileConflicts,
      selection,
      setBufferSelection,
      setConflictDialogData,
      itemOperationToFolder,
      setMoveToPanelVisible,
    };
  }
)(
  withTranslation(["ThirdPartyMoveDialog", "Common", "Translations"])(
    observer(PureThirdPartyMoveContainer)
  )
);
