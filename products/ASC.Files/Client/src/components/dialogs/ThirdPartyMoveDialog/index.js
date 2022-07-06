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
        margin-top: 3px;
      }

      .radio-option-title {
        font-weight: 600;
        font-size: 14px;
      }
    }
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

  const providerTitle = connectedCloudsTypeTitleTranslation(provider, t);

  const [resolveType, setResolveType] = useState("move");
  const onSelectResolveType = (e) => setResolveType(e.target.value);

  const radioOptions = [
    {
      label: (
        <Text className="radio-option-title">{t("Translations:Move")}</Text>
      ),
      value: "move",
    },
    {
      label: (
        <Text className="radio-option-title">{t("Translations:Copy")}</Text>
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

  return (
    <StyledOperationDialog
      isLoading={!tReady}
      visible={visible}
      zIndex={zIndex}
      onClose={onClose}
      displayType="modal"
      isLarge
    >
      <ModalDialog.Header>{t("MoveConfirmation")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text>{t("MoveConfirmationMessage", { provider: providerTitle })}</Text>

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
