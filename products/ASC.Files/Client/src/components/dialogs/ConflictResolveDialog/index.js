import React, { useState } from "react";
import { withRouter } from "react-router";
import ModalDialog from "@appserver/components/modal-dialog";
import RadioButtonGroup from "@appserver/components/radio-button-group";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { ConflictResolveType } from "@appserver/common/constants";
import toastr from "studio/toastr";
import styled from "styled-components";

const StyledModalDialog = styled(ModalDialog)`
  .conflict-resolve-dialog-text {
    padding-bottom: 8px;
  }

  .conflict-resolve-radio-button {
    svg {
      overflow: visible;
    }
  }

  .modal-dialog-aside-footer {
    display: grid;
    grid-template-columns: 1fr 1fr;
    grid-gap: 10px;
    width: 90%;
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
          <Text>{t("OverwriteTitle")}</Text>
          <Text>{t("OverwriteDescription")}</Text>
        </div>
      ),
      value: "overwrite",
    },
    {
      label: (
        <div>
          <Text>{t("CreateTitle")}</Text>
          <Text>{t("CreateDescription")}</Text>
        </div>
      ),

      value: "create",
    },
    {
      label: (
        <div>
          <Text>{t("SkipTitle")}</Text>
          <Text>{t("SkipDescription")}</Text>
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
      displayType="aside"
    >
      <ModalDialog.Header>{t("ConflictResolveTitle")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text className="conflict-resolve-dialog-text">
          {singleFile
            ? t("ConflictResolveDescription", { file, folder: folderTitle })
            : t("ConflictResolveDescriptionFiles", {
                filesCount,
                folder: folderTitle,
              })}
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
          size="small"
          primary
          scale
          onClick={onAcceptType}
          //isLoading={isLoading}
        />
        <Button
          className="button-dialog"
          key="CancelButton"
          label={t("Common:CancelButton")}
          size="small"
          scale
          onClick={onCloseDialog}
          //isLoading={isLoading}
        />
      </ModalDialog.Footer>
    </StyledModalDialog>
  );
};

export default inject(({ dialogsStore, uploadDataStore, filesStore }) => {
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

  return {
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
