import React, { useState, useEffect, useCallback } from "react";
import Button from "@docspace/components/button";
import ModalDialog from "@docspace/components/modal-dialog";
import TextInput from "@docspace/components/text-input";
import PasswordInput from "@docspace/components/password-input";
import FieldContainer from "@docspace/components/field-container";

const FormConnection = (props) => {
  const { visible, t, tReady, item, saveSettings, onClose } = props;
  const { provider_key, key } = item;

  const [urlValue, setUrlValue] = useState("");
  const [loginValue, setLoginValue] = useState("");
  const [passwordValue, setPasswordValue] = useState("");

  const [isUrlValid, setIsUrlValid] = useState(true);
  const [isLoginValid, setIsLoginValid] = useState(true);
  const [isPasswordValid, setIsPasswordValid] = useState(true);

  const [isLoading, setIsLoading] = useState(false);

  const showUrlField =
    provider_key === "WebDav" ||
    provider_key === "SharePoint" ||
    key === "WebDav" ||
    key === "SharePoint";

  const onChangeUrl = (e) => {
    setIsUrlValid(true);
    setUrlValue(e.target.value);
  };
  const onChangeLogin = (e) => {
    setIsLoginValid(true);
    setLoginValue(e.target.value);
  };
  const onChangePassword = (e) => {
    setIsPasswordValid(true);
    setPasswordValue(e.target.value);
  };

  const onKeyUpHandler = useCallback(
    (e) => {
      if (e.keyCode === 13) onSave();
    },
    [urlValue, loginValue, passwordValue, showUrlField]
  );

  useEffect(() => {
    window.addEventListener("keyup", onKeyUpHandler);
    return () => window.removeEventListener("keyup", onKeyUpHandler);
  }, [onKeyUpHandler]);

  const onSave = useCallback(() => {
    const urlValid = !!urlValue.trim();
    const loginValid = !!loginValue.trim();
    const passwordValid = !!passwordValue.trim();

    if (!loginValid || !passwordValid || (showUrlField && !urlValid)) {
      showUrlField && setIsUrlValid(urlValid);
      setIsLoginValid(loginValid);
      setIsPasswordValid(passwordValid);
      return;
    }

    saveSettings(undefined, urlValue, loginValue, passwordValue);
  }, [urlValue, loginValue, passwordValue, showUrlField]);

  return (
    <ModalDialog
      //isLoading={!tReady}
      visible={visible}
      zIndex={310}
      displayType="modal"
      onClose={onClose}
    >
      <ModalDialog.Header>
        {t("Translations:ConnectingAccount")}
      </ModalDialog.Header>
      <ModalDialog.Body>
        <>
          {showUrlField && (
            <FieldContainer
              labelVisible
              isRequired
              labelText={t("ConnectionUrl")}
              isVertical
              hasError={!isUrlValid}
              errorMessage={t("Common:RequiredField")}
            >
              <TextInput
                isAutoFocussed={true}
                hasError={!isUrlValid}
                isDisabled={isLoading}
                tabIndex={1}
                scale
                value={urlValue}
                onChange={onChangeUrl}
              />
            </FieldContainer>
          )}

          <FieldContainer
            labelText={t("Login")}
            isRequired
            isVertical
            hasError={!isLoginValid}
            errorMessage={t("Common:RequiredField")}
          >
            <TextInput
              isAutoFocussed={!showUrlField}
              hasError={!isLoginValid}
              isDisabled={isLoading}
              tabIndex={2}
              scale
              value={loginValue}
              onChange={onChangeLogin}
            />
          </FieldContainer>
          <FieldContainer
            labelText={t("Common:Password")}
            isRequired
            isVertical
            hasError={!isPasswordValid}
            errorMessage={t("Common:RequiredField")}
          >
            <PasswordInput
              hasError={!isPasswordValid}
              isDisabled={isLoading}
              tabIndex={3}
              simpleView
              passwordSettings={{ minLength: 0 }}
              value={passwordValue}
              onChange={onChangePassword}
            />
          </FieldContainer>
        </>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          tabIndex={4}
          label={t("Common:SaveButton")}
          size="normal"
          primary
          onClick={onSave}
          isDisabled={isLoading}
          isLoading={isLoading}
        />
      </ModalDialog.Footer>
    </ModalDialog>
  );
};

export default FormConnection;
