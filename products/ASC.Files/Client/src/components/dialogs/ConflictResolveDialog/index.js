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

const StyledModalDialog = styled(ModalDialog)`
  .conflict-resolve-dialog-text {
    padding-bottom: 12px;
  }
  .conflict-resolve-dialog-text-description {
    padding-bottom: 16px;
  }

  .conflict-resolve-radio-button {
    label {
      display: flex;
      align-items: flex-start;
    }
    svg {
      overflow: visible;
      margin-right: 8px;
      margin-top: 3px;
    }
  }

  .modal-dialog-aside-footer {
    display: grid;
    grid-template-columns: 1fr 1fr;
    grid-gap: 10px;
    width: 100%;
  }

  .button-dialog-accept {
    margin-right: 8px;
  }

  .modal-dialog-aside-footer {
    border-top: ${(props) => props.theme.button.border.baseDisabled};
    margin-left: -16px;
    margin-right: -16px;
    padding-left: 16px;
    padding-right: 16px;
    padding-top: 16px;
  }

  .modal-dialog-modal-footer {
    border-top: ${(props) => props.theme.button.border.baseDisabled};
    margin-left: -12px;
    margin-right: -12px;
    padding-left: 12px;
    padding-right: 12px;
    padding-top: 12px;
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
          <Text fontWeight={600} fontSize={"14px"}>
            {t("OverwriteTitle")}
          </Text>
          <Text color={theme.text.disableColor} fontSize={"12px"}>
            {t("OverwriteDescription")}
          </Text>
        </div>
      ),
      value: "overwrite",
    },
    {
      label: (
        <div>
          <Text fontWeight={600} fontSize={"14px"}>
            {t("CreateTitle")}
          </Text>
          <Text color={theme.text.disableColor} fontSize={"12px"}>
            {t("CreateDescription")}
          </Text>
        </div>
      ),

      value: "create",
    },
    {
      label: (
        <div>
          <Text fontWeight={600} fontSize={"14px"}>
            {t("SkipTitle")}
          </Text>
          <Text color={theme.text.disableColor} fontSize={"12px"}>
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
      theme={theme}
      style={{ maxWidth: "520px" }}
    >
      <ModalDialog.Header>{t("ConflictResolveTitle")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text className="conflict-resolve-dialog-text-description">
          {singleFile ? (
            <Trans
              t={t}
              file={filesCount}
              folder={folderTitle}
              i18nKey="ConflictResolveDescription"
              ns="ConflictResolveDialog"
            >
              The file with the name <strong>{{ file }}</strong> already exists
              in the folder
              <strong>{{ folder: folderTitle }}</strong>.
            </Trans>
          ) : (
            <Trans
              t={t}
              filesCount={filesCount}
              folder={folderTitle}
              i18nKey="ConflictResolveDescriptionFiles"
              ns="ConflictResolveDialog"
            >
              {{ filesCount }} documents with the same name already exist in the
              folder <strong>{{ folder: folderTitle }}</strong>.
            </Trans>
          )}
        </Text>
        <Text className="conflict-resolve-dialog-text">
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
          className="button-dialog-accept"
          key="OkButton"
          label={t("Common:OKButton")}
          size="normalTouchscreen"
          primary
          onClick={onAcceptType}
          //isLoading={isLoading}
        />
        <Button
          className="button-dialog"
          key="CancelButton"
          label={t("Common:CancelButton")}
          size="normalTouchscreen"
          onClick={onCloseDialog}
          //isLoading={isLoading}
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
