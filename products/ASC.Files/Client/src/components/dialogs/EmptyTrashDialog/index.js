import React, { useCallback } from "react";
import ModalDialogContainer from "../ModalDialogContainer";
import { toastr, ModalDialog, Button, Text } from "asc-web-components";
import { withTranslation } from "react-i18next";
import i18n from "./i18n";
import { api, utils } from "asc-web-common";
import { fetchFiles, clearProgressData } from "../../../store/files/actions";
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
    setProgressBarData,
    getProgress,
    isLoading
  } = props;

  changeLanguage(i18n);

 
  const loopEmptyTrash = useCallback(id => {
    const successMessage = "Success empty recycle bin";
    getProgress().then(res => {
      const currentProcess = res.find(x => x.id === id);
      if(currentProcess && currentProcess.progress !== 100) {
        const newProgressData = { visible: true, percent: currentProcess.progress, label: t("DeleteOperation")};
        setProgressBarData(newProgressData);
        setTimeout(() => loopEmptyTrash(id), 1000);
      } else {
        fetchFiles(currentFolderId, filter, store.dispatch).then(() => {
          setProgressBarData({ visible: true, percent: 100, label: t("DeleteOperation")});
          setTimeout(() => clearProgressData(store.dispatch), 5000);
          toastr.success(successMessage);
        }).catch(err => {
          toastr.error(err);
          clearProgressData(store.dispatch);
        })
      }
    }).catch(err => {
      toastr.error(err);
      clearProgressData(store.dispatch);
    })
  }, [t, currentFolderId, filter, getProgress, setProgressBarData])

  const onEmptyTrash = useCallback(() => {
    const newProgressData = { visible: true, percent: 0, label: t("DeleteOperation")};
    setProgressBarData(newProgressData);
    onClose();
    files.emptyTrash()
      .then(res => {
        const id = res[0] && res[0].id ? res[0].id : null;
        loopEmptyTrash(id);
      })
      .catch(err => {
        toastr.error(err);
        clearProgressData(store.dispatch);
      })
  }, [onClose, loopEmptyTrash, setProgressBarData, t]);

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
