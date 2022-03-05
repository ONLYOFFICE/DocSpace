import React, { useState } from "react";
import { withRouter } from "react-router";
import ModalDialogContainer from "../ModalDialogContainer";
import TextInput from "@appserver/components/text-input";
import Button from "@appserver/components/button";
import ModalDialog from "@appserver/components/modal-dialog";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import toastr from "@appserver/components/toast/toastr";

const CreateFolderDialogComponent = (props) => {
  const {
    visible,
    t,
    tReady,
    isLoading,
    setCreateFolderDialogVisible,
    createFolder,
    filter,
    fetchFiles,
    renameFolder,
    renameItem,
    setRenameItem,
    folderId,
    setAction,
  } = props;

  const [folderName, setFolderName] = useState(
    renameItem ? renameItem.title : ""
  );
  const [inputError, setInputError] = useState(false);

  const onClose = () => {
    setCreateFolderDialogVisible(false);
    setRenameItem(null);
    setAction({
      type: null,
      id: null,
      extension: null,
      title: "",
      templateId: null,
    });
  };

  const onCreate = async () => {
    if (folderName.length === 0) {
      setInputError(true);
      return;
    }

    try {
      (await renameItem)
        ? renameFolder(renameItem.id, folderName)
        : createFolder(folderId, folderName);

      await fetchFiles(folderId, filter);
    } catch (e) {
      toastr.error(e);
    }
    onClose();
  };

  const onChange = (e) => {
    setInputError(false);
    setFolderName(e.target.value);
  };

  const onKeyDown = (e) => {
    if (e.code === "Enter") onCreate();
  };

  const headerTranslate = renameItem ? t("Home:Rename") : t("Home:NewFolder");
  const okButtonTranslate = renameItem
    ? t("Common:SaveButton")
    : t("Common:Create");

  return (
    <ModalDialogContainer
      isLoading={!tReady}
      visible={visible}
      onClose={onClose}
      contentWidth={"400px"}
    >
      <ModalDialog.Header>{headerTranslate}</ModalDialog.Header>
      <ModalDialog.Body>
        <TextInput
          scale={true}
          id="folder-name"
          name="folder-name"
          value={folderName}
          onChange={onChange}
          onKeyDown={onKeyDown}
          placeholder={t("Home:NewFolder")}
          tabIndex={1}
          hasError={inputError}
          isAutoFocussed={true}
        />
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="OkButton"
          label={okButtonTranslate}
          size="big"
          primary
          onClick={onCreate}
          isLoading={isLoading}
          scale={true}
        />
        <Button
          className="button-dialog"
          key="CancelButton"
          label={t("Common:CancelButton")}
          size="big"
          onClick={onClose}
          isLoading={isLoading}
          scale={true}
        />
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

const CreateFolderDialog = withTranslation(["Common", "Translations", "Home"])(
  CreateFolderDialogComponent
);

export default inject(({ filesStore, dialogsStore }) => {
  const {
    fetchFiles,
    filter,
    isLoading,
    createFolder,
    renameFolder,
    selectedFolderStore,
    fileActionStore,
  } = filesStore;

  const {
    createFolderDialogVisible: visible,
    setCreateFolderDialogVisible,
    renameItem,
    setRenameItem,
  } = dialogsStore;

  return {
    isLoading,
    filter,
    visible,
    setCreateFolderDialogVisible,
    fetchFiles,
    createFolder,
    renameFolder,
    renameItem,
    setRenameItem,
    folderId: selectedFolderStore.id,
    setAction: fileActionStore.setAction,
  };
})(withRouter(observer(CreateFolderDialog)));
