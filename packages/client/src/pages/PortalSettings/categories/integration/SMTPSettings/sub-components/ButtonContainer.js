import React from "react";
import { inject, observer } from "mobx-react";

import Button from "@docspace/components/button";

import { ButtonStyledComponent } from "../StyledComponent";
import { SMTPSettingsFields } from "../constants";
const {
  HOST,
  PORT,
  SENDER_EMAIL_ADDRESS,
  HOST_LOGIN,
  HOST_PASSWORD,
} = SMTPSettingsFields;
const ButtonContainer = (props) => {
  const {
    t,
    updateSMTPSettings,
    isEmailValid,
    onSetValidationError,
    settings,
  } = props;

  const isValidForm = () => {
    if (
      settings[HOST_PASSWORD].trim() === "" ||
      settings[HOST_LOGIN].trim() === "" ||
      settings[HOST].trim() === "" ||
      settings[PORT].toString().trim() === "" ||
      settings[SENDER_EMAIL_ADDRESS].trim() === ""
    )
      return false;

    return true;
  };
  const onClick = async () => {
    onSetValidationError(!isEmailValid);

    if (!isEmailValid) return;

    updateSMTPSettings();
  };

  return (
    <ButtonStyledComponent>
      <Button
        label={t("Common:SaveButton")}
        size="normal"
        primary
        onClick={onClick}
        isDisabled={!isValidForm()}
      />
    </ButtonStyledComponent>
  );
};

export default inject(({ setup }) => {
  const { updateSMTPSettings, integration } = setup;
  const { smtpSettings } = integration;
  return {
    updateSMTPSettings,
    settings: smtpSettings.settings,
  };
})(observer(ButtonContainer));
