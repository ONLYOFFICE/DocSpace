import React, { useState, useCallback, useEffect } from "react";
import ModalDialog from "@appserver/components/modal-dialog";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import { withTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import SimulatePassword from "../../SimulatePassword";
import StyledComponent from "./StyledConvertPasswordDialog";

const ConvertPasswordDialogComponent = (props) => {
  const {
    t,
    visible,
    setConvertPasswordDialogVisible,
    isTabletView,
    copyAsAction,
    formCreationInfo,
    setFormCreationInfo,
    setPasswordEntryProcess,
  } = props;

  const [password, setPassword] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [passwordValid, setPasswordValid] = useState(true);

  const dialogHeading =
    formCreationInfo.fromExst === ".docxf" &&
    formCreationInfo.toExst === ".oform"
      ? t("Common:MakeForm")
      : "";

  const onClose = () => {
    setConvertPasswordDialogVisible(false);
    setFormCreationInfo(null);
  };
  const onConvert = () => {
    let hasError = false;

    const pass = password.trim();
    if (!pass) {
      hasError = true;
      setPasswordValid(false);
    }

    if (hasError) return;

    setIsLoading(true);
  };

  useEffect(() => {
    const { newTitle, fileInfo } = formCreationInfo;
    const { id, folderId } = fileInfo;

    isLoading &&
      copyAsAction(id, newTitle, folderId, false, password)
        .then(() => {
          onClose();
        })
        .catch((err) => {
          console.log("err", err);
          setPasswordValid(false);
          setIsLoading(false);
        });
  }, [isLoading]);

  useEffect(() => {
    setPasswordEntryProcess(true);

    return () => {
      setPasswordEntryProcess(false);
    };
  }, []);

  const onChangePassword = useCallback(
    (password) => {
      !passwordValid && setPasswordValid(true);
      setPassword(password);
    },
    [onChangePassword, passwordValid]
  );

  return (
    <ModalDialog visible={visible} onClose={onClose}>
      <ModalDialog.Header>{dialogHeading}</ModalDialog.Header>
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
                isDisabled={isLoading}
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
              label={t("Common:SaveButton")}
              size="medium"
              primary
              onClick={onConvert}
            />
            <Button
              className="convert-password-dialog_button"
              key="CloseButton"
              label={t("Common:CloseButton")}
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
  "Common",
])(ConvertPasswordDialogComponent);

export default inject(({ filesStore, auth, dialogsStore, uploadDataStore }) => {
  const {
    convertPasswordDialogVisible: visible,
    setConvertPasswordDialogVisible,
    setFormCreationInfo,
    formCreationInfo,
  } = dialogsStore;
  const { copyAsAction } = uploadDataStore;
  const { setPasswordEntryProcess } = filesStore;
  const { settingsStore } = auth;
  const { isTabletView } = settingsStore;

  return {
    visible,
    setConvertPasswordDialogVisible,
    isTabletView,
    copyAsAction,
    formCreationInfo,
    setFormCreationInfo,
    setPasswordEntryProcess,
  };
})(observer(ConvertPasswordDialog));
