import React, { useCallback, useEffect } from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import ModalDialogContainer from "../ModalDialogContainer";
import { ModalDialog, Button, Text } from "asc-web-components";
import { withTranslation } from "react-i18next";
import { api, utils, toastr } from "asc-web-common";
import {
  fetchFiles,
  setSecondaryProgressBarData,
  clearSecondaryProgressData,
} from "../../../store/files/actions";
import { TIMEOUT } from "../../../helpers/constants";
import {
  getSelectedFolderId,
  getFilter,
  getIsLoading,
} from "../../../store/files/selectors";
import { createI18N } from "../../../helpers/i18n";

const i18n = createI18N({
  page: "EmptyTrashDialog",
  localesPath: "dialogs/EmptyTrashDialog",
});

const { files } = api;
const { changeLanguage } = utils;

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

  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  const loopEmptyTrash = useCallback(
    (id) => {
      const successMessage = "Success empty recycle bin";
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

const ModalDialogContainerTranslated = withTranslation()(
  EmptyTrashDialogComponent
);

const EmptyTrashDialog = (props) => (
  <ModalDialogContainerTranslated i18n={i18n} {...props} />
);

const mapStateToProps = (state) => {
  return {
    currentFolderId: getSelectedFolderId(state),
    filter: getFilter(state),
    isLoading: getIsLoading(state),
  };
};

export default connect(mapStateToProps, {
  setSecondaryProgressBarData,
  clearSecondaryProgressData,
  fetchFiles,
})(withRouter(EmptyTrashDialog));
