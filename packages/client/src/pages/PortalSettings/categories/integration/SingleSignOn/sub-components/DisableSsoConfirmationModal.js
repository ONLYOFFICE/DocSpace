import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@docspace/components/box";
import Button from "@docspace/components/button";
import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";

import StyledModalDialog from "../styled-containers/StyledModalDialog";

const DisableSsoConfirmationModal = (props) => {
  const { t } = useTranslation(["SingleSignOn", "Common"]);
  const {
    closeConfirmationDisableModal,
    confirmationDisableModal,
    confirmDisable,
  } = props;

  return (
    <StyledModalDialog
      contentHeight="100%"
      displayType="modal"
      onClose={closeConfirmationDisableModal}
      visible={confirmationDisableModal}
    >
      <ModalDialog.Header>{t("Common:Confirmation")}</ModalDialog.Header>

      <ModalDialog.Body>
        <Text noSelect>{t("ConfirmationText")}</Text>
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Button
          id="ok-button"
          label={t("Common:OKButton")}
          onClick={confirmDisable}
          primary
          scale
          size="normal"
        />
        <Button
          id="cancel-button"
          label={t("Common:CancelButton")}
          onClick={closeConfirmationDisableModal}
          scale
          size="normal"
        />
      </ModalDialog.Footer>
    </StyledModalDialog>
  );
};

export default inject(({ ssoStore }) => {
  const {
    closeConfirmationDisableModal,
    confirmationDisableModal,
    confirmDisable,
  } = ssoStore;

  return {
    closeConfirmationDisableModal,
    confirmationDisableModal,
    confirmDisable,
  };
})(observer(DisableSsoConfirmationModal));
