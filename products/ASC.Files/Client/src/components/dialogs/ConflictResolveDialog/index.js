import React, { useState } from "react";
import { withRouter } from "react-router";
import ModalDialogContainer from "../ModalDialogContainer";
import ModalDialog from "@appserver/components/modal-dialog";
import RadioButtonGroup from "@appserver/components/radio-button-group";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { ConflictResolveType } from "@appserver/common/constants";
import toastr from "studio/toastr";

const ConflictResolveDialog = (props) => {
  const {
    t,
    tReady,
    visible,
    setConflictResolveDialogVisible,
    conflictResolveDialogData,
    items,
    itemOperationToFolder,
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
    if (conflictResolveType === ConflictResolveType.Skip) {
      for (let item of items) {
        newFileIds = newFileIds.filter((x) => x.id === item.id);
      }
    }

    if (!folderIds.length && !newFileIds.length) return onClose();

    const data = {
      destFolderId,
      folderIds,
      fileIds: newFileIds,
      conflictResolveType,
      deleteAfter,
      isCopy,
      translations,
    };

    onClose();
    try {
      await itemOperationToFolder(data);
    } catch (error) {
      toastr.error(error.message);
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
    <ModalDialogContainer
      isLoading={!tReady}
      visible={visible}
      onClose={onClose}
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
          size="medium"
          primary
          onClick={onAcceptType}
          //isLoading={isLoading}
        />
        <Button
          className="button-dialog"
          key="CancelButton"
          label={t("Common:CancelButton")}
          size="medium"
          onClick={onClose}
          //isLoading={isLoading}
        />
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

export default inject(({ dialogsStore, uploadDataStore }) => {
  const {
    conflictResolveDialogVisible: visible,
    setConflictResolveDialogVisible,
    conflictResolveDialogData,
    conflictResolveDialogItems: items,
  } = dialogsStore;

  const { itemOperationToFolder } = uploadDataStore;

  return {
    items,
    visible,
    conflictResolveDialogData,
    setConflictResolveDialogVisible,
    itemOperationToFolder,
  };
})(
  withRouter(
    withTranslation(["ConflictResolveDialog", "Common"])(
      observer(ConflictResolveDialog)
    )
  )
);
