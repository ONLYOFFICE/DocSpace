import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";
import toastr from "@docspace/components/toast/toastr";
import Dialog from "./sub-components/Dialog";
import { getTitleWithoutExtension } from "SRC_DIR/helpers/filesUtils";

const RenameEvent = ({
  type,
  item,
  onClose,

  setIsLoading,
  addActiveItems,

  updateFile,
  renameFolder,

  completeAction,
  clearActiveOperations,

  setEventDialogVisible,
  eventDialogVisible,

  selectedFolderId,

  setSelectedFolder,
}) => {
  const [startValue, setStartValue] = React.useState("");

  const { t } = useTranslation(["Files"]);

  React.useEffect(() => {
    setStartValue(getTitleWithoutExtension(item, false));

    setEventDialogVisible(true);
  }, [item]);

  const onUpdate = React.useCallback((e, value) => {
    const originalTitle = getTitleWithoutExtension(item);

    let timerId;

    const isSameTitle =
      originalTitle.trim() === value.trim() || value.trim() === "";

    const isFile = item.fileExst || item.contentLength;

    if (isSameTitle) {
      setStartValue(originalTitle);

      onCancel();

      return completeAction(item, type);
    } else {
      timerId = setTimeout(() => {
        isFile ? addActiveItems([item.id]) : addActiveItems(null, [item.id]);
      }, 500);
    }

    isFile
      ? updateFile(item.id, value)
          .then(() => completeAction(item, type))
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
            completeAction(item, type);
          })
          .finally(() => {
            clearTimeout(timerId);
            timerId = null;
            clearActiveOperations([item.id]);

            onCancel();
          })
      : renameFolder(item.id, value)
          .then(() => completeAction(item, type))
          .then(() => {
            if (selectedFolderId === item.id) {
              setSelectedFolder({ title: value });
            }
            toastr.success(
              t("FolderRenamed", {
                folderTitle: item.title,
                newFoldedTitle: value,
              })
            );
          })
          .catch((err) => {
            toastr.error(err);
            completeAction(item, type);
          })
          .finally(() => {
            clearTimeout(timerId);
            timerId = null;
            clearActiveOperations(null, [item.id]);

            onCancel();
          });
  }, []);

  const onCancel = React.useCallback(
    (e) => {
      onClose && onClose(e);
      setEventDialogVisible(false);
    },
    [onClose, setEventDialogVisible]
  );

  return (
    <Dialog
      t={t}
      visible={eventDialogVisible}
      title={t("Files:Rename")}
      startValue={startValue}
      onSave={onUpdate}
      onCancel={onCancel}
      onClose={onCancel}
    />
  );
};

export default inject(
  ({
    filesStore,
    filesActionsStore,
    selectedFolderStore,
    uploadDataStore,
    dialogsStore,
  }) => {
    const { setIsLoading, addActiveItems, updateFile, renameFolder } =
      filesStore;

    const { id, setSelectedFolder } = selectedFolderStore;

    const { completeAction } = filesActionsStore;

    const { clearActiveOperations } = uploadDataStore;
    const { setEventDialogVisible, eventDialogVisible } = dialogsStore;

    return {
      setIsLoading,
      addActiveItems,
      updateFile,
      renameFolder,

      completeAction,

      clearActiveOperations,
      setEventDialogVisible,
      eventDialogVisible,

      selectedFolderId: id,

      setSelectedFolder,
    };
  }
)(observer(RenameEvent));
