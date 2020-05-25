import React, { useCallback } from "react";
import ModalDialogContainer from "../ModalDialogContainer";
import { toastr, ModalDialog, Button, Text } from "asc-web-components";
import { withTranslation } from "react-i18next";
import i18n from "./i18n";
import { api, utils } from "asc-web-common";
import { fetchFiles } from "../../../store/files/actions";
import store from "../../../store/store";

const { files } = api;
const { changeLanguage } = utils;

const EmptyTrashDialogComponent = props => {
  const {
    onClose,
    visible,
    t,
    filter,
    currentFolderId,
    startFilesOperations,
    finishFilesOperations,
    setProgressValue,
    getProgress,
    isLoading
  } = props;

  changeLanguage(i18n);
  
  const loopEmptyTrash = useCallback(id => {
    const successMessage = "Success empty recycle bin";
    getProgress().then(res => {
      const currentProcess = res.find(x => x.id === id);
      if(currentProcess && currentProcess.progress !== 100) {
        setProgressValue(currentProcess.progress);
        setTimeout(() => loopEmptyTrash(id), 1000);
      } else {
        fetchFiles(currentFolderId, filter, store.dispatch).then(() => {
          setProgressValue(100);
          finishFilesOperations();
          toastr.success(successMessage);
        }).catch(err => finishFilesOperations(err))
      }
    }).catch(err => finishFilesOperations(err))
  }, [currentFolderId, filter, finishFilesOperations, setProgressValue, getProgress])

  const onEmptyTrash = useCallback(() => {
    startFilesOperations(t("DeleteOperation"));
    onClose();
    files.emptyTrash()
      .then(res => {
        const id = res[0] && res[0].id ? res[0].id : null;
        loopEmptyTrash(id);
      })
      .catch(err => finishFilesOperations(err))
  }, [onClose, loopEmptyTrash, startFilesOperations, finishFilesOperations, t]);

  return (
    <ModalDialogContainer>
      <ModalDialog
        visible={visible}
        onClose={onClose}
        headerContent={t("ConfirmationTitle")}
        bodyContent={
          <>
            <Text>{t("EmptyTrashDialogQuestion")}</Text>
            <Text>{t("EmptyTrashDialogMessage")}</Text>
            <Text>{t("EmptyTrashDialogWarning")}</Text>
          </>
        }
        footerContent={
          <>
            <Button
              key="OkButton"
              label={t("OKButton")}
              size="medium"
              primary
              onClick={onEmptyTrash}
              isLoading={isLoading}
            />
            <Button
              className="button-dialog"
              key="CancelButton"
              label={t("CancelButton")}
              size="medium"
              onClick={onClose}
              isLoading={isLoading}
            />
          </>
        }
      />
    </ModalDialogContainer>
  );
};

const ModalDialogContainerTranslated = withTranslation()(
  EmptyTrashDialogComponent
);

const EmptyTrashDialog = props => (
  <ModalDialogContainerTranslated i18n={i18n} {...props} />
);

export default EmptyTrashDialog;
