import React, { useState } from "react";
import styled from "styled-components";

import { withTranslation } from "react-i18next";
import ModalDialog from "@appserver/components/modal-dialog";
import Text from "@appserver/components/text";
import Button from "@appserver/components/button";
import { inject, observer } from "mobx-react";
import toastr from "@appserver/components/toast/toastr";
import { connectedCloudsTypeTitleTranslation } from "../../../helpers/utils";
import RadioButtonGroup from "@appserver/components/radio-button-group";

const StyledOperationDialog = styled(ModalDialog)`
  .modal-dialog-aside-footer {
    display: flex;
    width: 90%;
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
  const [actionType, setActionType] = useState("move");

  const onClosePanels = () => {
    setDestFolderId(false);
    setThirdPartyMoveDialogVisible(false);
    setMoveToPanelVisible(false);
    setBufferSelection(null);
  };
  const onClose = () => setThirdPartyMoveDialogVisible(false);

  const startOperation = () => {
    const isCopy = actionType === "copy";

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
          onClosePanels();
          await itemOperationToFolder(data);
        }
      })
      .catch((err) => toastr.error(err))
      .finally(() => {
        setIsLoading(false);
      });
  };

  const providerTitle = connectedCloudsTypeTitleTranslation(provider, t);

  const onSelectActionType = (e) => setActionType(e.target.value);

  const radioOptions = [
    {
      label: (
        <div>
          <Text fontWeight={600} fontSize={"14px"}>
            {t("Translations:Move")}
          </Text>
        </div>
      ),
      value: "move",
    },
    {
      label: (
        <div>
          <Text fontWeight={600} fontSize={"14px"}>
            {t("Translations:Copy")}
          </Text>
        </div>
      ),

      value: "copy",
    },
  ];
  return (
    <StyledOperationDialog
      isLoading={!tReady}
      visible={conflictResolveDialogVisible ? false : visible}
      zIndex={zIndex}
      onClose={onClose}
    >
      <ModalDialog.Header>{t("MoveConfirmation")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text>{t("MoveConfirmationMessage", { provider: providerTitle })}</Text>

        <Text className="third-party-move-dialog-text">
          {t("ConflictResolveDialog:ConflictResolveSelectAction")}
        </Text>
        <RadioButtonGroup
          className="third-party-move-radio-button"
          orientation="vertical"
          fontSize="13px"
          fontWeight="400"
          name="group"
          onClick={onSelectActionType}
          options={radioOptions}
          selected={actionType}
        />
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Button
          className="operation-button"
          label={t("Common:OKButton")}
          size="normalTouchscreen"
          primary
          onClick={startOperation}
          isLoading={isLoading}
          isDisabled={isLoading}
        />

        <Button
          className="operation-button"
          label={t("Common:CancelButton")}
          size="normalTouchscreen"
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
    "Common",
    "Translations",
    "ConflictResolveDialog",
  ])(observer(PureThirdPartyMoveContainer))
);
