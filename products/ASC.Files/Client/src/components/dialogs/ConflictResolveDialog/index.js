import React, { useState } from "react";
import { withRouter } from "react-router";
import ModalDialog from "@appserver/components/modal-dialog";
import RadioButtonGroup from "@appserver/components/radio-button-group";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import { withTranslation, Trans } from "react-i18next";
import { inject, observer } from "mobx-react";
import { ConflictResolveType } from "@appserver/common/constants";
import toastr from "studio/toastr";
import styled from "styled-components";
import { convertFile } from "@appserver/common/api/files";

const StyledModalDialog = styled(ModalDialog)`
  .radio {
    padding-bottom: 8px;
  }

  .message {
    margin-bottom: 16px;
  }

  .select-action {
    margin-bottom: 12px;
  }

  .conflict-resolve-radio-button {
    label {
      display: flex;
      align-items: flex-start;
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
      line-height: 16px;
    }

    .radio-option-description {
      font-size: 12px;
      line-height: 16px;
      color: #a3a9ae;
    }
  }
`;

const ConflictResolveDialog = (props) => {
  const {
    t,
    tReady,
    visible,
    setConflictResolveDialogVisible,
    conflictResolveDialogData,
    items,
    itemOperationToFolder,
    activeFiles,
    setActiveFiles,
    setMoveToPanelVisible,
    setCopyPanelVisible,
    setThirdPartyMoveDialogVisible,
    theme,
  } = props;

  const {
    destFolderId,
    folderIds,
    fileIds,
    deleteAfter,
    folderTitle,
    isCopy,
    translations,
  } = conflictResolveDialogData;

  const [resolveType, setResolveType] = useState("overwrite");

  const onSelectResolveType = (e) => setResolveType(e.target.value);
  const onClose = () => setConflictResolveDialogVisible(false);
  const onClosePanels = () => {
    setConflictResolveDialogVisible(false);
    setMoveToPanelVisible(false);
    setCopyPanelVisible(false);
    setThirdPartyMoveDialogVisible(false);
  };
  const onCloseDialog = () => {
    let newActiveFiles = activeFiles;

    for (let item of fileIds) {
      newActiveFiles = newActiveFiles.filter((f) => f !== item);
    }

    setActiveFiles(newActiveFiles);
    onClose();
  };

  const getResolveType = () => {
    switch (resolveType) {
      case "skip":
        return ConflictResolveType.Skip;
      case "overwrite":
        return ConflictResolveType.Overwrite;
      case "create":
        return ConflictResolveType.Duplicate;

      default:
        return ConflictResolveType.Overwrite;
    }
  };

  const onAcceptType = async () => {
    const conflictResolveType = getResolveType();

    let newFileIds = fileIds;
    let newActiveFiles = activeFiles;
    if (conflictResolveType === ConflictResolveType.Skip) {
      for (let item of items) {
        newFileIds = newFileIds.filter((x) => x !== item.id);
        newActiveFiles = newActiveFiles.filter((f) => f !== item.id);
      }
    }

    setActiveFiles(newActiveFiles);
    if (!folderIds.length && !newFileIds.length) return onClosePanels();

    const data = {
      destFolderId,
      folderIds,
      fileIds: newFileIds,
      conflictResolveType,
      deleteAfter,
      isCopy,
      translations,
    };

    onClosePanels();
    try {
      await itemOperationToFolder(data);
    } catch (error) {
      toastr.error(error.message ? error.message : error);
    }
  };

  const radioOptions = [
    {
      label: (
        <div>
          <Text className="radio-option-title">{t("OverwriteTitle")}</Text>
          <Text className="radio-option-description">
            {" "}
            {t("OverwriteDescription")}
          </Text>
        </div>
      ),
      value: "overwrite",
    },
    {
      label: (
        <div>
          <Text className="radio-option-title">{t("CreateTitle")}</Text>
          <Text className="radio-option-description">
            {t("CreateDescription")}
          </Text>
        </div>
      ),

      value: "create",
    },
    {
      label: (
        <div>
          <Text className="radio-option-title">{t("SkipTitle")}</Text>
          <Text className="radio-option-description">
            {t("SkipDescription")}
          </Text>
        </div>
      ),
      value: "skip",
    },
  ];

  const filesCount = items.length;
  const singleFile = filesCount === 1;
  const file = items[0].title;

  return (
    <StyledModalDialog
      isLoading={!tReady}
      visible={visible}
      onClose={onCloseDialog}
      isLarge
      zIndex={312}
    >
      <ModalDialog.Header>{t("ConflictResolveTitle")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text className="message">
          {console.log(filesCount, folderTitle)}

          {singleFile ? (
            <Trans
              t={t}
              i18nKey="ConflictResolveDescription"
              ns="ConflictResolveDialog"
            >
              {{ file, folder: folderTitle }}
            </Trans>
          ) : (
            <Trans
              t={t}
              i18nKey="ConflictResolveDescriptionFiles"
              ns="ConflictResolveDialog"
            >
              {{ filesCount, folder: folderTitle }}
            </Trans>
          )}
        </Text>
        <Text className="select-action">
          {t("ConflictResolveSelectAction")}
        </Text>
        <RadioButtonGroup
          className="conflict-resolve-radio-button"
          orientation="vertical"
          fontSize="13px"
          fontWeight="400"
          name="group"
          onClick={onSelectResolveType}
          options={radioOptions}
          selected="overwrite"
        />
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="OkButton"
          label={t("Common:OKButton")}
          size="normal"
          primary
          onClick={onAcceptType}
        />
        <Button
          key="CancelButton"
          label={t("Common:CancelButton")}
          size="normal"
          onClick={onCloseDialog}
        />
      </ModalDialog.Footer>
    </StyledModalDialog>
  );
};

export default inject(({ auth, dialogsStore, uploadDataStore, filesStore }) => {
  const {
    conflictResolveDialogVisible: visible,
    setConflictResolveDialogVisible,
    conflictResolveDialogData,
    conflictResolveDialogItems: items,
    setMoveToPanelVisible,
    setCopyPanelVisible,
    setThirdPartyMoveDialogVisible,
  } = dialogsStore;

  const { itemOperationToFolder } = uploadDataStore;
  const { activeFiles, setActiveFiles } = filesStore;
  const { settingsStore } = auth;
  const { theme } = settingsStore;
  return {
    theme,
    items,
    visible,
    conflictResolveDialogData,
    setConflictResolveDialogVisible,
    itemOperationToFolder,
    activeFiles,
    setActiveFiles,
    setMoveToPanelVisible,
    setCopyPanelVisible,
    setThirdPartyMoveDialogVisible,
  };
})(
  withRouter(
    withTranslation(["ConflictResolveDialog", "Common"])(
      observer(ConflictResolveDialog)
    )
  )
);
