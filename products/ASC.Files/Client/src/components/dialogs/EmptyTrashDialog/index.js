import React, { useCallback } from "react";
import { withRouter } from "react-router";
import ModalDialogContainer from "../ModalDialogContainer";
import { ModalDialog, Button, Text } from "asc-web-components";
import { withTranslation } from "react-i18next";
import { api, toastr } from "asc-web-common";
import { TIMEOUT } from "../../../helpers/constants";
import { inject, observer } from "mobx-react";

const { files } = api;

const EmptyTrashDialogComponent = (props) => {
  const {
    onClose,
    visible,
    t,
    filter,
    currentFolderId,
    setSecondaryProgressBarData,
    isLoading,
    clearSecondaryProgressData,
    fetchFiles,
  } = props;

  const loopEmptyTrash = useCallback(
    (id) => {
      const successMessage = t("SuccessEmptyTrash");
      api.files
        .getProgress()
        .then((res) => {
          const currentProcess = res.find((x) => x.id === id);
          if (currentProcess && currentProcess.progress !== 100) {
            const newProgressData = {
              icon: "trash",
              visible: true,
              percent: currentProcess.progress,
              label: t("DeleteOperation"),
              alert: false,
            };
            setSecondaryProgressBarData(newProgressData);
            setTimeout(() => loopEmptyTrash(id), 1000);
          } else {
            fetchFiles(currentFolderId, filter)
              .then(() => {
                setSecondaryProgressBarData({
                  icon: "trash",
                  visible: true,
                  percent: 100,
                  label: t("DeleteOperation"),
                  alert: false,
                });
                setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
                toastr.success(successMessage);
              })
              .catch((err) => {
                setSecondaryProgressBarData({
                  visible: true,
                  alert: true,
                });
                //toastr.error(err);
                setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
              });
          }
        })
        .catch((err) => {
          setSecondaryProgressBarData({
            visible: true,
            alert: true,
          });
          //toastr.error(err);
          setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
        });
    },
    [
      t,
      currentFolderId,
      filter,
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
      fetchFiles,
    ]
  );

  const onEmptyTrash = useCallback(() => {
    const newProgressData = {
      icon: "trash",
      visible: true,
      percent: 0,
      label: t("DeleteOperation"),
      alert: false,
    };
    setSecondaryProgressBarData(newProgressData);
    onClose();
    files
      .emptyTrash()
      .then((res) => {
        const id = res[0] && res[0].id ? res[0].id : null;
        loopEmptyTrash(id);
      })
      .catch((err) => {
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        //toastr.error(err);
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      });
  }, [
    onClose,
    loopEmptyTrash,
    setSecondaryProgressBarData,
    t,
    clearSecondaryProgressData,
  ]);

  return (
    <ModalDialogContainer>
      <ModalDialog visible={visible} onClose={onClose}>
        <ModalDialog.Header>{t("ConfirmationTitle")}</ModalDialog.Header>
        <ModalDialog.Body>
          <Text>{t("EmptyTrashDialogQuestion")}</Text>
          <Text>{t("EmptyTrashDialogMessage")}</Text>
        </ModalDialog.Body>
        <ModalDialog.Footer>
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
        </ModalDialog.Footer>
      </ModalDialog>
    </ModalDialogContainer>
  );
};

const EmptyTrashDialog = withTranslation("EmptyTrashDialog")(
  EmptyTrashDialogComponent
);

export default inject(
  ({ initFilesStore, filesStore, uploadDataStore, selectedFolderStore }) => {
    const { isLoading } = initFilesStore;
    const { secondaryProgressDataStore } = uploadDataStore;
    const { fetchFiles, filter } = filesStore;
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = secondaryProgressDataStore;

    return {
      currentFolderId: selectedFolderStore.id,
      isLoading,
      filter,

      fetchFiles,
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    };
  }
)(withRouter(observer(EmptyTrashDialog)));
