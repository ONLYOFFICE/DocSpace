import React, { useState, useCallback } from "react";
import ModalDialog from "@appserver/components/modal-dialog";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import SimulatePassword from "../../SimulatePassword";
import StyledComponent from "./StyledConvertPasswordDialog";

const ConvertPasswordDialogComponent = (props) => {
  const { t, visible, setConvertPasswordDialogVisible, isTabletView } = props;

  const [password, setPassword] = useState("");
  const [passwordValid, setPasswordValid] = useState(true);

  const onClose = () => {
    setConvertPasswordDialogVisible(false);
  };
  const onConvert = () => {
    let hasError = false;

    const pass = password.trim();
    if (!pass) {
      hasError = true;
      setPasswordValid(false);
    }

    if (hasError) return;
  };
  const onChangePassword = useCallback(
    (password) => {
      !passwordValid && setPasswordValid(true);
      setPassword(password);
    },
    [onChangePassword, passwordValid]
  );
  console.log("ConvertPasswordDialogComponent", !passwordValid);
  return (
    <ModalDialog visible={visible} onClose={onClose}>
      <ModalDialog.Header>
        {t("ConvertDialog:ConvertAndOpenTitle")}
      </ModalDialog.Header>
      <ModalDialog.Body>
        <StyledComponent>
          <div className="convert-password-dialog_content">
            <div className="convert-password-dialog_caption">
              <Text>{t("ConversionPasswordCaption")}</Text>
            </div>
            <div className="password-input">
              <SimulatePassword
                inputMaxWidth={"512px"}
                inputBlockMaxWidth={"536px"}
                onChange={onChangePassword}
                hasError={!passwordValid}
              />
            </div>
          </div>
        </StyledComponent>
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <StyledComponent isTabletView={isTabletView}>
          <div className="convert-password_footer">
            <Button
              id="convert-password-dialog_button-accept"
              className="convert-password-dialog_button"
              key="ContinueButton"
              label={t("Convert")}
              size="medium"
              primary
              onClick={onConvert}
            />
            <Button
              className="convert-password-dialog_button"
              key="CloseButton"
              label={t("Common:Cancel")}
              size="medium"
              onClick={onClose}
            />
          </div>
        </StyledComponent>
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

const ConvertPasswordDialog = withTranslation([
  "ConvertPasswordDialog",
  "ConvertDialog",
  "Home",
  "Common",
])(ConvertPasswordDialogComponent);

export default inject(({ auth, dialogsStore, uploadDataStore }) => {
  const {
    convertPasswordDialogVisible: visible,
    setConvertPasswordDialogVisible,
  } = dialogsStore;
  const { copyAsAction } = uploadDataStore;

  const { settingsStore } = auth;
  const { isTabletView } = settingsStore;

  return {
    visible,
    setConvertPasswordDialogVisible,
    isTabletView,
  };
})(observer(ConvertPasswordDialog));
