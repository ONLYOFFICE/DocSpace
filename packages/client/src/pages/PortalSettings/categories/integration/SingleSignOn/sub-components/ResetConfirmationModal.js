import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@docspace/components/box";
import Button from "@docspace/components/button";
import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";

import StyledModalDialog from "../styled-containers/StyledModalDialog";

const ResetConfirmationModal = (props) => {
  const { t } = useTranslation(["SingleSignOn", "Common"]);
  const { closeResetModal, confirmationResetModal, confirmReset } = props;

  return (
    <StyledModalDialog
      contentHeight="100%"
      displayType="modal"
      onClose={closeResetModal}
      visible={confirmationResetModal}
    >
      <ModalDialog.Header>{t("Common:Confirmation")}</ModalDialog.Header>

      <ModalDialog.Body>
        <Text noSelect>{t("ConfirmationText")}</Text>
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Button
          label={t("Common:OKButton")}
          onClick={confirmReset}
          primary
          size="small"
        />
        <Button
          label={t("Common:CancelButton")}
          onClick={closeResetModal}
          size="small"
        />
      </ModalDialog.Footer>
    </StyledModalDialog>
  );
};

export default inject(({ ssoStore }) => {
  const { closeResetModal, confirmationResetModal, confirmReset } = ssoStore;

  return { closeResetModal, confirmationResetModal, confirmReset };
})(observer(ResetConfirmationModal));
