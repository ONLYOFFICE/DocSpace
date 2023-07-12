import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import ModalDialog from "@docspace/components/modal-dialog";
import Checkbox from "@docspace/components/checkbox";
import Button from "@docspace/components/button";
import Box from "@docspace/components/box";
import Text from "@docspace/components/text";
import ModalDialogContainer from "../ModalDialogContainer";

const LogoutAllConnectionDialog = ({
  visible,
  onClose,
  onRemoveAllSessions,
  loading,
  onRemoveAllExceptThis,
}) => {
  const { t } = useTranslation(["Profile", "Common"]);
  const [isChecked, setIsChecked] = useState(false);

  const onChangeCheckbox = () => {
    setIsChecked((prev) => !prev);
  };

  return (
    <ModalDialogContainer
      visible={visible}
      onClose={onClose}
      displayType="modal"
    >
      <ModalDialog.Header>
        {t("Profile:LogoutAllActiveConnections")}
      </ModalDialog.Header>
      <ModalDialog.Body>
        <Text as="p">{t("Profile:LogoutDescription")}</Text>
        <Text as="p" style={{ margin: "15px 0" }}>
          {t("Profile:DescriptionForSecurity")}
        </Text>
        <Box displayProp="flex" alignItems="center">
          <Checkbox
            className="change-password"
            isChecked={isChecked}
            onChange={onChangeCheckbox}
          />
          {t("Profile:ChangePasswordAfterLoggingOut")}
        </Box>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          className="logout"
          key="LogoutBtn"
          label={t("Profile:LogoutBtn")}
          size="normal"
          scale
          primary={true}
          onClick={() =>
            isChecked ? onRemoveAllSessions() : onRemoveAllExceptThis()
          }
          isLoading={loading}
        />
        <Button
          className="cancel-button"
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

export default LogoutAllConnectionDialog;
