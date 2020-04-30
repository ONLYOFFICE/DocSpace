import React, { useCallback, useState } from "react";
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
  const { onClose, visible, t, filter, currentFolderId } = props;
  const [isLoading, setIsLoading] = useState(false);

  changeLanguage(i18n);

  const successMessage = "Success empty recycle bin";

  const onEmptyTrash = useCallback(() => {
    setIsLoading(true);
    files
      .emptyTrash()
      .then(res => {
        fetchFiles(currentFolderId, filter, store.dispatch)
        toastr.success(successMessage);
      }) //toastr.success("It was successfully deleted 24 from 24"); + progressBar
      .catch(err => toastr.error(err))
      .finally(() => {
        setIsLoading(false);
        onClose();
      });
  }, [onClose, filter, currentFolderId]);

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
