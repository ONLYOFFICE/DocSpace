import React from "react";
import { useTranslation } from "react-i18next";
import ModalDialog from "@docspace/components/modal-dialog";
import Button from "@docspace/components/button";
import ModalDialogContainer from "../ModalDialogContainer";

const LogoutConnectionDialog = ({
  visible,
  onClose,
  onRemoveSession,
  data,
  loading,
}) => {
  const { t } = useTranslation(["Profile", "Common"]);

  return (
    <ModalDialogContainer
      visible={visible}
      onClose={onClose}
      displayType="modal"
    >
      <ModalDialog.Header>
        {t("Profile:LogoutActiveConnection")}
      </ModalDialog.Header>
      <ModalDialog.Body>
        {t("Profile:LogoutFrom", {
          platform: data.platform,
          browser: data.browser,
        })}
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="LogoutBtn"
          label={t("Profile:LogoutBtn")}
          size="normal"
          scale
          primary={true}
          onClick={() => onRemoveSession(data.id)}
          isLoading={loading}
        />
        <Button
          key="CloseBtn"
          label={t("Common:CancelButton")}
          size="normal"
          scale
          onClick={onClose}
          isDisabled={loading}
        />
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

export default LogoutConnectionDialog;
