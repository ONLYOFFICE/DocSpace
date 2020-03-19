import React, { useCallback, useState } from "react";
import ModalDialogContainer from "../ModalDialogContainer";
import { toastr, ModalDialog, Button, Text } from "asc-web-components";
import { withTranslation } from "react-i18next";
import i18n from "./i18n";
import { api, utils } from "asc-web-common";

const { files } = api;
const { changeLanguage } = utils;

const EmptyTrashDialogComponent = props => {
  const { onClose, visible, t } = props;
  const [isLoading, setIsLoading] = useState(false);

  changeLanguage(i18n);

  const onEmptyTrash = useCallback(() => {
    setIsLoading(true);
    files
      .emptyTrash()
      .then(res => {
        toastr.success("Success empty recycle bin");
      }) //toastr.success("It was successfully deleted 24 from 24"); + progressBar
      .catch(err => toastr.error(err))
      .finally(() => {
        setIsLoading(false);
        onClose();
      });
  }, [onClose]);

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
