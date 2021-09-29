import React, { useCallback } from "react";
import { withRouter } from "react-router";
import ModalDialogContainer from "../ModalDialogContainer";
import Text from "@appserver/components/text";
import Button from "@appserver/components/button";
import ModalDialog from "@appserver/components/modal-dialog";
import { withTranslation } from "react-i18next";
import { getProgress, emptyTrash } from "@appserver/common/api/files";
import toastr from "@appserver/components/toast/toastr";
import { TIMEOUT } from "../../../helpers/constants";
import { inject, observer } from "mobx-react";

const EmptyTrashDialogComponent = (props) => {
  const {
    visible,
    t,
    tReady,
    filter,
    currentFolderId,
    setSecondaryProgressBarData,
    isLoading,
    clearSecondaryProgressData,
    fetchFiles,
    setEmptyTrashDialogVisible,
  } = props;

  const onClose = () => setEmptyTrashDialogVisible(false);

  const loopEmptyTrash = useCallback(
    (id) => {
      const successMessage = t("SuccessEmptyTrash");
      getProgress()
        .then((res) => {
          const currentProcess = res.find((x) => x.id === id);
          if (currentProcess && currentProcess.progress !== 100) {
            const newProgressData = {
              icon: "trash",
              visible: true,
              percent: currentProcess.progress,
              label: t("Translations:DeleteOperation"),
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
                  label: t("Translations:DeleteOperation"),
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
      label: t("Translations:DeleteOperation"),
      alert: false,
    };
    setSecondaryProgressBarData(newProgressData);
    onClose();
    emptyTrash()
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
    <ModalDialogContainer
      isLoading={!tReady}
      visible={visible}
      onClose={onClose}
    >
      <ModalDialog.Header>{t("DeleteForeverTitle")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text>{t("DeleteForeverNote")}</Text>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="OkButton"
          label={t("DeleteForeverButton")}
          size="medium"
          primary
          onClick={onEmptyTrash}
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

const EmptyTrashDialog = withTranslation([
  "EmptyTrashDialog",
  "Common",
  "Translations",
])(EmptyTrashDialogComponent);

export default inject(
  ({ filesStore, uploadDataStore, selectedFolderStore, dialogsStore }) => {
    const { secondaryProgressDataStore } = uploadDataStore;
    const { fetchFiles, filter, isLoading } = filesStore;
    const {
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = secondaryProgressDataStore;

    const {
      emptyTrashDialogVisible: visible,
      setEmptyTrashDialogVisible,
    } = dialogsStore;

    return {
      currentFolderId: selectedFolderStore.id,
      isLoading,
      filter,
      visible,

      fetchFiles,
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
      setEmptyTrashDialogVisible,
    };
  }
)(withRouter(observer(EmptyTrashDialog)));
