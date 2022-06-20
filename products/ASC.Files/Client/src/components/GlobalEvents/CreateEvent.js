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

const CreateEvent = ({
  id,
  type,
  extension,
  title,
  templateId,
  fromTemplate,
  onClose,

  setIsLoading,
  createFile,
  createFolder,
  addActiveItems,
  openDocEditor,
  setIsUpdatingRowItem,
  gallerySelected,

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
  const [headerTitle, setHeaderTitle] = React.useState(null);
  const [startValue, setStartValue] = React.useState("");

  const { t } = useTranslation(["Translations", "Common"]);

  React.useEffect(() => {
    const defaultName = getDefaultFileName(extension);

    if (title) {
      const item = { fileExst: extension, title: title };

      setStartValue(getTitleWithoutExst(item, fromTemplate));
    } else {
      setStartValue(defaultName);
    }

    setHeaderTitle(defaultName);
    setVisible(true);
  }, [extension, title, fromTemplate]);

  const completeAction = (id, item) => {
    const isCancel =
      (id.currentTarget && id.currentTarget.dataset.action === "cancel") ||
      id.keyCode === 27;

    editCompleteAction(id, item, isCancel, type);
  };

  const onSave = (e, value, open = true) => {
    let item;
    let createdFileId, createdFolderId;

    const isMakeFormFromFile = templateId ? true : false;

    setIsLoading(true);

    const newValue = value;

    // delete it later
    // const itemId = e.currentTarget.dataset.itemid;

    if (value.trim() === "") {
      newValue =
        templateId === null
          ? getDefaultFileName(extension)
          : getTitleWithoutExst({ fileExst: extension });

      setStartValue(newValue);
    }

    let tab =
      !isDesktop && extension && open
        ? window.open(
            combineUrl(AppServerConfig.proxyURL, config.homepage, "/doceditor"),
            "_blank"
          )
        : null;

    if (!extension) {
      createFolder(parentId, newValue)
        .then((folder) => {
          item = folder;
          createdFolderId = folder.id;
          addActiveItems(null, [folder.id]);
        })
        .then(() => completeAction(id, item))
        .catch((e) => toastr.error(e))
        .finally(() => {
          const folderIds = [+id];
          createdFolderId && folderIds.push(createdFolderId);

          clearActiveOperations(null, folderIds);

          return setIsLoading(false);
        });
    } else {
      if (isMakeFormFromFile) {
        fileCopyAs(templateId, `${newValue}.${extension}`, parentId)
          .then((file) => {
            item = file;
            createdFileId = file.id;
            addActiveItems([file.id]);

            open && openDocEditor(file.id, file.providerKey, tab);
          })
          .then(() => completeAction(id, item))
          .catch((err) => {
            console.log("err", err);
            const isPasswordError = new RegExp("password");

            if (isPasswordError.test(err)) {
              toastr.error(
                t("Translations:FileProtected"),
                t("Common:Warning")
              );

              setIsUpdatingRowItem(false);

              setVisible(false);

              setFormCreationInfo({
                newTitle: `${newValue}.${extension}`,
                fromExst: ".docx",
                toExst: extension,
                open,
                actionId: id,
                fileInfo: {
                  id: templateId,
                  folderId: parentId,
                  fileExst: extension,
                },
              });

              setConvertPasswordDialogVisible(true);

              open && openDocEditor(null, null, tab);
            }
          })
          .finally(() => {
            const fileIds = [+id];
            createdFileId && fileIds.push(createdFileId);

            clearActiveOperations(fileIds);

            return setIsLoading(false);
          });
      } else if (fromTemplate) {
        createFile(
          parentId,
          `${newValue}.${extension}`,
          undefined,
          gallerySelected.id
        )
          .then((file) => {
            item = file;
            createdFileId = file.id;
            addActiveItems([file.id]);

            return open && openDocEditor(file.id, file.providerKey, tab);
          })
          .then(() => completeAction(id, item))
          .catch((e) => toastr.error(e))
          .finally(() => {
            const fileIds = [+id];
            createdFileId && fileIds.push(createdFileId);

            clearActiveOperations(fileIds);

            return setIsLoading(false);
          });
      } else {
        createFile(parentId, `${newValue}.${extension}`)
          .then((file) => {
            createdFileId = file.id;
            item = file;
            addActiveItems([file.id]);

            if (isPrivacy) {
              return setEncryptionAccess(file).then((encryptedFile) => {
                if (!encryptedFile) return Promise.resolve();
                toastr.info(t("Translations:EncryptedFileSaving"));

                return replaceFileStream(
                  file.id,
                  encryptedFile,
                  true,
                  false
                ).then(
                  () => open && openDocEditor(file.id, file.providerKey, tab)
                );
              });
            }

            return open && openDocEditor(file.id, file.providerKey, tab);
          })
          .then(() => completeAction(id, item))
          .catch((e) => toastr.error(e))
          .finally(() => {
            const fileIds = [+id];
            createdFileId && fileIds.push(createdFileId);

            clearActiveOperations(fileIds);

            return setIsLoading(false);
          });
      }
    }
  };

  const onCancel = React.useCallback(
    (e) => {
      onClose && onClose();
    },
    [onClose]
  );

  return (
    <Dialog
      visible={visible}
      title={headerTitle}
      startValue={startValue}
      onSave={onSave}
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
)(observer(CreateEvent));
