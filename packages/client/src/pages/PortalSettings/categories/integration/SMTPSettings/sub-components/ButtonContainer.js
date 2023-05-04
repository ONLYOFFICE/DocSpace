import React, { useState } from "react";
import { inject, observer } from "mobx-react";

import Button from "@docspace/components/button";
import toastr from "@docspace/components/toast/toastr";

import { ButtonStyledComponent } from "../StyledComponent";
import { SMTPSettingsFields } from "../constants";

const {
  HOST,
  PORT,
  SENDER_EMAIL_ADDRESS,
  HOST_LOGIN,
  HOST_PASSWORD,
  AUTHENTICATION,
} = SMTPSettingsFields;

let timerId = null;
const ButtonContainer = (props) => {
  const {
    t,
    isEmailValid,
    onSetValidationError,
    settings,
    setSMTPSettingsLoading,
    updateSMTPSettings,
    resetSMTPSettings,
    isLoading,
    isDefaultSettings,
    isSMTPInitialSettings,
  } = props;

  const [buttonOperation, setButtonOperation] = useState({
    save: false,
    reset: false,
    send: false,
  });

  const isValidForm = () => {
    if (
      (settings[AUTHENTICATION] &&
        (settings[HOST_PASSWORD]?.trim() === "" ||
          settings[HOST_LOGIN]?.trim() === "")) ||
      settings[HOST]?.trim() === "" ||
      settings[PORT]?.toString()?.trim() === "" ||
      settings[SENDER_EMAIL_ADDRESS]?.trim() === ""
    )
      return false;

    return true;
  };
  const onClick = async () => {
    onSetValidationError(!isEmailValid);

    if (!isEmailValid) return;

    timerId = setTimeout(() => {
      setSMTPSettingsLoading(true);
      setButtonOperation({ ...buttonOperation, save: true });
    }, [200]);

    try {
      await updateSMTPSettings();
      toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
    } catch (e) {
      toastr.error(e);
    }

    clearTimeout(timerId);
    timerId = null;
    setSMTPSettingsLoading(false);
    setButtonOperation({ ...buttonOperation, save: false });
  };

  const onClickSendTestMail = async () => {};

  const onClickDefaultSettings = async () => {
    timerId = setTimeout(() => {
      setSMTPSettingsLoading(true);
      setButtonOperation({ ...buttonOperation, reset: true });
    }, [200]);

    try {
      await resetSMTPSettings();
      toastr.success(t("Settings:SuccessfullySaveSettingsMessage"));
    } catch (e) {
      toastr.error(e);
    }

    clearTimeout(timerId);
    timerId = null;
    setSMTPSettingsLoading(false);
    setButtonOperation({ ...buttonOperation, reset: false });
  };

  return (
    <ButtonStyledComponent>
      <Button
        label={t("Common:SaveButton")}
        size="small"
        primary
        onClick={onClick}
        isDisabled={isLoading || !isValidForm() || isSMTPInitialSettings}
        isLoading={buttonOperation.save}
      />
      <Button
        label={t("Settings:DefaultSettings")}
        size="small"
        onClick={onClickDefaultSettings}
        isLoading={buttonOperation.reset}
        isDisabled={isLoading || isDefaultSettings}
      />
      <Button
        label={t("SendTestMail")}
        size="small"
        onClick={onClickSendTestMail}
        isDisabled={isLoading}
      />
    </ButtonStyledComponent>
  );
};

export default inject(({ setup }) => {
  const {
    integration,
    setSMTPSettingsLoading,
    updateSMTPSettings,
    resetSMTPSettings,
    isSMTPInitialSettings,
  } = setup;
  const { smtpSettings } = integration;
  const { settings, isLoading, isDefaultSettings } = smtpSettings;
  return {
    isSMTPInitialSettings,
    isDefaultSettings,
    settings,
    setSMTPSettingsLoading,
    updateSMTPSettings,
    resetSMTPSettings,
    isLoading,
  };
})(observer(ButtonContainer));
