import React from "react";
import { useTranslation } from "react-i18next";

import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";

import ModalDialogContainer from "../ModalDialogContainer";

const PortalRenamingDialog = (props) => {
  const { t, ready } = useTranslation(["Settings", "Common"]);
  const { visible, onClose, onSave, isSaving } = props;

  return (
    <ModalDialogContainer
      isLoading={!ready}
      visible={visible}
      onClose={onClose}
      displayType="modal"
    >
      <ModalDialog.Header>{t("Settings:PortalRenaming")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text fontSize="13px" fontWeight={400} noSelect>
          {t("Settings:PortalRenamingModalText")}
        </Text>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="PortalRenamingContinueBtn"
          label={t("Common:ContinueButton")}
          size="normal"
          scale
          primary={true}
          onClick={onSave}
          isLoading={isSaving}
          tabIndex={3}
        />
        <Button
          key="CloseBtn"
          label={t("Common:CancelButton")}
          size="normal"
          scale
          onClick={onClose}
          isDisabled={isSaving}
          tabIndex={4}
        />
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

export default PortalRenamingDialog;
