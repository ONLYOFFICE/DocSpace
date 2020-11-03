import React, { useCallback, useEffect } from "react";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import ModalDialogContainer from "../ModalDialogContainer";
import { ModalDialog, Button, Text } from "asc-web-components";
import { withTranslation } from "react-i18next";
import { api, utils, toastr } from "asc-web-common";
import {
  fetchFiles,
  setProgressBarData,
  clearProgressData,
} from "../../../store/files/actions";
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
    setProgressBarData,
    isLoading,
    clearProgressData,
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
              visible: true,
              percent: currentProcess.progress,
              label: t("DeleteOperation"),
            };
            setProgressBarData(newProgressData);
            setTimeout(() => loopEmptyTrash(id), 1000);
          } else {
            fetchFiles(currentFolderId, filter)
              .then(() => {
                setProgressBarData({
                  visible: true,
                  percent: 100,
                  label: t("DeleteOperation"),
                });
                setTimeout(() => clearProgressData(), 5000);
                toastr.success(successMessage);
              })
              .catch((err) => {
                toastr.error(err);
                clearProgressData();
              });
          }
        })
        .catch((err) => {
          toastr.error(err);
          clearProgressData();
        });
    },
    [
      t,
      currentFolderId,
      filter,
      setProgressBarData,
      clearProgressData,
      fetchFiles,
    ]
  );

  const onEmptyTrash = useCallback(() => {
    const newProgressData = {
      visible: true,
      percent: 0,
      label: t("DeleteOperation"),
    };
    setProgressBarData(newProgressData);
    onClose();
    files
      .emptyTrash()
      .then((res) => {
        const id = res[0] && res[0].id ? res[0].id : null;
        loopEmptyTrash(id);
      })
      .catch((err) => {
        toastr.error(err);
        clearProgressData();
      });
  }, [onClose, loopEmptyTrash, setProgressBarData, t, clearProgressData]);

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
  setProgressBarData,
  clearProgressData,
  fetchFiles,
})(withRouter(EmptyTrashDialog));
