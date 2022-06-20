import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import toastr from "studio/toastr";

import { AppServerConfig } from "@appserver/common/constants";
import { combineUrl } from "@appserver/common/utils";

import config from "../../../package.json";

import { getTitleWithoutExst } from "../../helpers/files-helpers";
import { getDefaultFileName } from "../../helpers/utils";

import Dialog from "./sub-components/Dialog";

const RenameEvent = ({
  type,
  item,
  onClose,

  setIsLoading,
  createFile,
  createFolder,
  addActiveItems,
  openDocEditor,
  setIsUpdatingRowItem,
  gallerySelected,

  updateFile,
  renameFolder,

  parentId,

  isPrivacy,
  isDesktop,
  editCompleteAction,

  clearActiveOperations,
  fileCopyAs,

  setConvertPasswordDialogVisible,
  setConvertItem,
  setFormCreationInfo,

  replaceFileStream,
  setEncryptionAccess,
}) => {
  const [visible, setVisible] = React.useState(false);

  const [startValue, setStartValue] = React.useState("");

  const { t } = useTranslation(["Home", "Translations", "Common"]);

  React.useEffect(() => {
    setStartValue(getTitleWithoutExst(item, false));

    setVisible(true);
  }, [item]);

  const completeAction = (id) => {
    const isCancel =
      (id.currentTarget && id.currentTarget.dataset.action === "cancel") ||
      id.keyCode === 27;

    editCompleteAction(id, item, isCancel, type);
  };

  const onUpdate = React.useCallback((e, value) => {
    const originalTitle = getTitleWithoutExst(item);

    setIsLoading(true);
    let timerId;

    const isSameTitle =
      originalTitle.trim() === value.trim() || value.trim() === "";

    const isFile = item.fileExst || item.contentLength;

    if (isSameTitle) {
      setStartValue(originalTitle);

      return editCompleteAction(item.id, item, isSameTitle, type);
    } else {
      timerId = setTimeout(() => {
        isFile ? addActiveItems([item.id]) : addActiveItems(null, [item.id]);
      }, 500);
    }

    isFile
      ? updateFile(item.id, value)
          .then(() => completeAction(item.id))
          .then(() =>
            toastr.success(
              t("FileRenamed", {
                oldTitle: item.title,
                newTitle: value + item.fileExst,
              })
            )
          )
          .catch((err) => {
            toastr.error(err);
            completeAction(item.id);
          })
          .finally(() => {
            clearTimeout(timerId);
            timerId = null;
            clearActiveOperations([item.id]);

            setIsLoading(false);
            onClose();
          })
      : renameFolder(item.id, value)
          .then(() => completeAction(item.id))
          .then(() =>
            toastr.success(
              t("FolderRenamed", {
                folderTitle: item.title,
                newFoldedTitle: value,
              })
            )
          )
          .catch((err) => {
            toastr.error(err);
            completeAction(item.id);
          })
          .finally(() => {
            clearTimeout(timerId);
            timerId = null;
            clearActiveOperations(null, [item.id]);

            setIsLoading(false);
            onClose();
          });
  }, []);

  const onCancel = React.useCallback(
    (e) => {
      onClose && onClose();
    },
    [onClose]
  );

  return (
    <Dialog
      visible={visible}
      title={t("Home: Rename")}
      startValue={startValue}
      onSave={onUpdate}
      onCancel={onCancel}
      onClose={onClose}
    />
  );
};

export default inject(
  ({
    auth,
    filesStore,
    filesActionsStore,
    selectedFolderStore,
    treeFoldersStore,
    uploadDataStore,
    dialogsStore,
  }) => {
    const {
      setIsLoading,
      createFile,
      createFolder,
      addActiveItems,
      openDocEditor,
      setIsUpdatingRowItem,
      gallerySelected,

      updateFile,
      renameFolder,
    } = filesStore;

    const { editCompleteAction } = filesActionsStore;

    const { clearActiveOperations, fileCopyAs } = uploadDataStore;

    const { isRecycleBinFolder, isPrivacyFolder } = treeFoldersStore;

    const { id: parentId } = selectedFolderStore;

    const { replaceFileStream, setEncryptionAccess } = auth;

    const { isDesktopClient } = auth.settingsStore;

    const {
      setConvertPasswordDialogVisible,
      setConvertItem,
      setFormCreationInfo,
    } = dialogsStore;

    return {
      setIsLoading,
      createFile,
      createFolder,
      addActiveItems,
      openDocEditor,
      setIsUpdatingRowItem,
      gallerySelected,

      updateFile,
      renameFolder,

      parentId,

      isDesktop: isDesktopClient,
      isPrivacy: isPrivacyFolder,
      isTrashFolder: isRecycleBinFolder,
      editCompleteAction,

      clearActiveOperations,
      fileCopyAs,

      setConvertPasswordDialogVisible,
      setConvertItem,
      setFormCreationInfo,

      replaceFileStream,
      setEncryptionAccess,
    };
  }
)(observer(RenameEvent));
