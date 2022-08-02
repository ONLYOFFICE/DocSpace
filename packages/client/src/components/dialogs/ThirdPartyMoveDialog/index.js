import React, { useState } from "react";
import styled from "styled-components";

import { withTranslation } from "react-i18next";
import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import { inject, observer } from "mobx-react";
import toastr from "@docspace/components/toast/toastr";
import { connectedCloudsTypeTitleTranslation } from "@docspace/client/src/helpers/filesUtils";
import RadioButtonGroup from "@docspace/components/radio-button-group";

const StyledOperationDialog = styled(ModalDialog)`
  .operation-button {
    margin-right: 8px;
  }

  .select-action-wrapper {
    margin-top: 16px;

    .select-action {
      margin-bottom: 12px;
    }

    .conflict-resolve-radio-button {
      label {
        display: flex;
        align-items: center;
        &:not(:last-child) {
          margin-bottom: 12px;
        }
      }

      svg {
        overflow: visible;
        margin-right: 8px;
      }

      span {
        display: flex;
        align-items: center;
        .radio-option-title {
          font-weight: 600;
          font-size: 14px;
          line-height: 16px;
        }
      }
    }
  }
  .third-party-move-radio-button {
    margin-top: 12px;
    label:not(:last-child) {
      margin-bottom: 12px;
    }
  }
  .modal-dialog-modal-footer {
    border-top: ${(props) => props.theme.button.border.baseDisabled};
    margin-left: -12px;
    margin-right: -12px;
    padding-left: 12px;
    padding-right: 12px;
    padding-top: 12px;
    .operation-button {
      margin-right: 8px;
    }
  }
  .modal-dialog-aside-footer {
    border-top: ${(props) => props.theme.button.border.baseDisabled};
    margin-left: -16px;
    margin-right: -16px;
    padding-left: 16px;
    padding-right: 16px;
    padding-top: 16px;

    display: grid;
    grid-template-columns: 1fr 1fr;
    grid-gap: 10px;
    width: 100%;
  }
  .third-party-move-dialog-text {
    margin-top: 16px;
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
  conflictResolveDialogVisible,
}) => {
  const zIndex = 310;
  const deleteAfter = false; // TODO: get from settings

  const [isLoading, setIsLoading] = useState(false);

  const onClosePanels = () => {
    setDestFolderId(false);
    setThirdPartyMoveDialogVisible(false);
    setMoveToPanelVisible(false);
    setBufferSelection(null);
  };
  const onClose = () => setThirdPartyMoveDialogVisible(false);

  const providerTitle = connectedCloudsTypeTitleTranslation(provider, t);

  const [resolveType, setResolveType] = useState("move");
  const onSelectResolveType = (e) => setResolveType(e.target.value);

  const radioOptions = [
    {
      label: <Text className="radio-option-title">{t("MoveFileOption")}</Text>,
      value: "move",
    },
    {
      label: (
        <Text className="radio-option-title">{t("CreateFileCopyOption")}</Text>
      ),
      value: "copy",
    },
  ];

  const startOperation = (e) => {
    const isCopy = resolveType === "copy";
    const folderIds = [];
    const fileIds = [];

    console.log(isCopy);

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
    };

    setIsLoading(true);
    checkFileConflicts(destFolderId, folderIds, fileIds)
      .then(async (conflicts) => {
        if (conflicts.length) {
          setConflictDialogData(conflicts, data);
          setIsLoading(false);
        } else {
          setIsLoading(false);
          onClosePanels();
          await itemOperationToFolder(data);
        }
      })
      .catch((err) => toastr.error(err))
      .finally(() => {
        setIsLoading(false);
      });
  };

  return (
    <StyledOperationDialog
      isLoading={!tReady}
      visible={conflictResolveDialogVisible ? false : visible}
      zIndex={zIndex}
      onClose={onClose}
      displayType="modal"
      isLarge
    >
      <ModalDialog.Header>{t("MoveConfirmation")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text>{t("MoveConfirmationAlert", { provider: providerTitle })}</Text>

        <div className="select-action-wrapper">
          <Text className="select-action">
            {t("ConflictResolveDialog:ConflictResolveSelectAction")}
          </Text>
          <RadioButtonGroup
            className="conflict-resolve-radio-button"
            orientation="vertical"
            fontSize="13px"
            fontWeight="400"
            name="group"
            onClick={onSelectResolveType}
            options={radioOptions}
            selected="move"
          />
        </div>
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Button
          label={t("Common:OKButton")}
          size="normal"
          primary
          onClick={startOperation}
          isLoading={isLoading}
          isDisabled={isLoading}
        />
        <Button
          label={t("Common:CancelButton")}
          size="normal"
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
      conflictResolveDialogVisible,
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
      provider: selection[0]?.providerKey,
      checkFileConflicts,
      selection,
      setBufferSelection,
      setConflictDialogData,
      itemOperationToFolder,
      setMoveToPanelVisible,
      conflictResolveDialogVisible,
    };
  }
)(
  withTranslation([
    "ThirdPartyMoveDialog",
    "ConflictResolveDialog",
    "Common",
    "Translations",
  ])(observer(PureThirdPartyMoveContainer))
);
