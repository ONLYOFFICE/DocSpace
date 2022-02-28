import React, { useState } from "react";
import { withRouter } from "react-router";
import ModalDialogContainer from "../ModalDialogContainer";
import TextInput from "@appserver/components/text-input";
import Button from "@appserver/components/button";
import ModalDialog from "@appserver/components/modal-dialog";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

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
  } = props;

  const [folderName, setFolderName] = useState(
    renameItem ? renameItem.title : ""
  );

  const onClose = () => {
    setCreateFolderDialogVisible(false);
    setRenameItem(null);
  };

  const onCreate = () => {
    let folderId = filter.folder;
    if (folderId === "@my") folderId = 2;

    if (renameItem) {
      renameFolder(renameItem.id, folderName).then(() =>
        fetchFiles(folderId, filter)
      );
    } else {
      createFolder(folderId, folderName).then(() =>
        fetchFiles(folderId, filter)
      );
    }
    onClose();
  };

  const onChange = (e) => setFolderName(e.target.value);

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
        />
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="OkButton"
          label={okButtonTranslate}
          size="medium"
          primary
          onClick={onCreate}
          isLoading={isLoading}
        />
        <Button
          className="button-dialog"
          key="CancelButton"
          label={t("Common:CancelButton")}
          size="medium"
          onClick={onClose}
          isLoading={isLoading}
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
  };
})(withRouter(observer(CreateFolderDialog)));
