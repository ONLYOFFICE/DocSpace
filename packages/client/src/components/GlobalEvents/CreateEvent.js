import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import toastr from "@docspace/components/toast/toastr";

import { AppServerConfig } from "@docspace/common/constants";
import { combineUrl } from "@docspace/common/utils";

import config from "PACKAGE_FILE";

import { getTitleWithoutExst } from "../../helpers/files-helpers";
import { getDefaultFileName } from "@docspace/client/src/helpers/filesUtils";

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
  setGallerySelected,
  setCreatedItem,

  parentId,

  isPrivacy,
  isDesktop,
  editCompleteAction,

  clearActiveOperations,
  fileCopyAs,

  setConvertPasswordDialogVisible,
  setFormCreationInfo,

  replaceFileStream,
  setEncryptionAccess,

  setEventDialogVisible,
  eventDialogVisible,
}) => {
  const [headerTitle, setHeaderTitle] = React.useState(null);
  const [startValue, setStartValue] = React.useState("");

  const { t } = useTranslation(["Translations", "Common"]);

  const onCloseAction = () => {
    if (gallerySelected) {
      setGallerySelected && setGallerySelected(null);
    }
    onClose && onClose();
  };

  React.useEffect(() => {
    const defaultName = getDefaultFileName(extension);

    if (title) {
      const item = { fileExst: extension, title: title };

      setStartValue(getTitleWithoutExst(item, fromTemplate));
    } else {
      setStartValue(defaultName);
    }

    setHeaderTitle(defaultName);
    setEventDialogVisible(true);

    return () => {
      setEventDialogVisible(false);
    };
  }, [extension, title, fromTemplate]);

  const onSave = (e, value, open = true) => {
    let item;
    let createdFileId, createdFolderId;

    const isMakeFormFromFile = templateId ? true : false;

    setIsLoading(true);

    let newValue = value;

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
            combineUrl(AppServerConfig.proxyURL, config.homepage, `/doceditor`),
            "_blank"
          )
        : null;

    if (!extension) {
      createFolder(parentId, newValue)
        .then((folder) => {
          item = folder;
          createdFolderId = folder.id;
          addActiveItems(null, [folder.id]);
          setCreatedItem({ id: createdFolderId, type: "folder" });
        })
        .then(() => editCompleteAction(item, type, true))
        .catch((e) => toastr.error(e))
        .finally(() => {
          const folderIds = [+id];
          createdFolderId && folderIds.push(createdFolderId);

          clearActiveOperations(null, folderIds);
          onCloseAction();
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
          .then(() => editCompleteAction(item, type))
          .catch((err) => {
            let errorMessage = "";
            if (typeof err === "object") {
              errorMessage =
                err?.response?.data?.error?.message ||
                err?.statusText ||
                err?.message ||
                "";
            } else {
              errorMessage = err;
            }

            if (errorMessage.indexOf("password") == -1) {
              toastr.error(errorMessage, t("Common:Warning"));
              return;
            }

            toastr.error(t("Translations:FileProtected"), t("Common:Warning"));

            setEventDialogVisible(false);

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

            // open && openDocEditor(null, null, null);
          })
          .finally(() => {
            const fileIds = [+id];
            createdFileId && fileIds.push(createdFileId);

            clearActiveOperations(fileIds);
            onCloseAction();
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
            setCreatedItem({ id: createdFileId, type: "file" });
            addActiveItems([file.id]);

            return open && openDocEditor(file.id, file.providerKey, tab);
          })
          .then(() => editCompleteAction(item, type))
          .catch((e) => toastr.error(e))
          .finally(() => {
            const fileIds = [+id];
            createdFileId && fileIds.push(createdFileId);

            clearActiveOperations(fileIds);
            onCloseAction();
            return setIsLoading(false);
          });
      } else {
        createFile(parentId, `${newValue}.${extension}`)
          .then((file) => {
            createdFileId = file.id;
            item = file;
            setCreatedItem({ id: createdFileId, type: "file" });
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
          .then(() => editCompleteAction(item, type))
          .catch((e) => toastr.error(e))
          .finally(() => {
            const fileIds = [+id];
            createdFileId && fileIds.push(createdFileId);

            clearActiveOperations(fileIds);
            onCloseAction();
            return setIsLoading(false);
          });
      }
    }
  };

  const onCancel = React.useCallback(
    (e) => {
      onCloseAction && onCloseAction();
    },
    [onCloseAction]
  );

  return (
    <Dialog
      t={t}
      visible={eventDialogVisible}
      title={headerTitle}
      startValue={startValue}
      onSave={onSave}
      onCancel={onCancel}
      onClose={onCloseAction}
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
    oformsStore,
  }) => {
    const {
      setIsLoading,
      createFile,
      createFolder,
      addActiveItems,
      openDocEditor,
      setIsUpdatingRowItem,
      setCreatedItem,
    } = filesStore;

    const { gallerySelected, setGallerySelected } = oformsStore;

    const { editCompleteAction } = filesActionsStore;

    const { clearActiveOperations, fileCopyAs } = uploadDataStore;

    const { isRecycleBinFolder, isPrivacyFolder } = treeFoldersStore;

    const { id: parentId } = selectedFolderStore;

    const { replaceFileStream, setEncryptionAccess } = auth;

    const { isDesktopClient } = auth.settingsStore;

    const {
      setConvertPasswordDialogVisible,
      setEventDialogVisible,
      setFormCreationInfo,
      eventDialogVisible,
    } = dialogsStore;

    return {
      setEventDialogVisible,
      eventDialogVisible,
      setIsLoading,
      createFile,
      createFolder,
      addActiveItems,
      openDocEditor,
      setIsUpdatingRowItem,
      gallerySelected,
      setGallerySelected,
      setCreatedItem,

      parentId,

      isDesktop: isDesktopClient,
      isPrivacy: isPrivacyFolder,
      isTrashFolder: isRecycleBinFolder,
      editCompleteAction,

      clearActiveOperations,
      fileCopyAs,

      setConvertPasswordDialogVisible,
      setFormCreationInfo,

      replaceFileStream,
      setEncryptionAccess,
    };
  }
)(observer(CreateEvent));
