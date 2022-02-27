import React from "react";
import { observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@appserver/components/box";
import Button from "@appserver/components/button";
import FormStore from "@appserver/studio/src/store/SsoFormStore";
import ModalDialog from "@appserver/components/modal-dialog";
import Text from "@appserver/components/text";

import StyledModalDialog from "../styled-containers/StyledModalDialog";
import { addArguments } from "../../../../utils";

const DisableSsoConfirmationModal = () => {
  const { t } = useTranslation(["SingleSignOn", "Common"]);

  const onClose = addArguments(
    FormStore.onCloseModal,
    "confirmationDisableModal"
  );

  return (
    <StyledModalDialog
      contentHeight="100%"
      displayType="modal"
      onClose={onClose}
      visible={FormStore.confirmationDisableModal}
    >
      <ModalDialog.Header>{t("Common:Confirmation")}</ModalDialog.Header>

      <ModalDialog.Body>
        <Text>{t("ConfirmationText")}</Text>
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Box displayProp="flex" marginProp="12px 0 4px 0">
          <Button
            className="ok-button"
            label={t("Common:OKButton")}
            onClick={FormStore.onConfirmDisable}
            primary
            size="big"
          />
          <Button
            label={t("Common:CancelButton")}
            onClick={onClose}
            size="big"
          />
        </Box>
      </ModalDialog.Footer>
    </StyledModalDialog>
  );
};

export default observer(DisableSsoConfirmationModal);
