import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import toastr from "client/toastr";

import { getTitleWithoutExst } from "../../helpers/files-helpers";

import Dialog from "./sub-components/Dialog";

const RenameEvent = ({
  type,
  item,
  onClose,

  setIsLoading,
  addActiveItems,

  updateFile,
  renameFolder,

  editCompleteAction,
  clearActiveOperations,
}) => {
  const [visible, setVisible] = React.useState(false);

  const [startValue, setStartValue] = React.useState("");

  const { t } = useTranslation(["Files"]);

  React.useEffect(() => {
    setStartValue(getTitleWithoutExst(item, false));

    setVisible(true);
  }, [item]);

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
          .then(() => editCompleteAction(item.id, item, false, type))
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
            editCompleteAction(item.id, item, false, type);
          })
          .finally(() => {
            clearTimeout(timerId);
            timerId = null;
            clearActiveOperations([item.id]);

            setIsLoading(false);
            onClose();
          })
      : renameFolder(item.id, value)
          .then(() => editCompleteAction(item.id, item, false, type))
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
            editCompleteAction(item.id, item, false, type);
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
      t={t}
      visible={visible}
      title={t("Home: Rename")}
      startValue={startValue}
      onSave={onUpdate}
      onCancel={onCancel}
      onClose={onClose}
    />
  );
};

export default inject(({ filesStore, filesActionsStore, uploadDataStore }) => {
  const { setIsLoading, addActiveItems, updateFile, renameFolder } = filesStore;

  const { editCompleteAction } = filesActionsStore;

  const { clearActiveOperations } = uploadDataStore;

  return {
    setIsLoading,
    addActiveItems,
    updateFile,
    renameFolder,

    editCompleteAction,

    clearActiveOperations,
  };
})(observer(RenameEvent));
