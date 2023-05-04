import React, { useState } from "react";
import { inject, observer } from "mobx-react";

import TextInput from "@docspace/components/text-input";
import Text from "@docspace/components/text";
import ToggleButton from "@docspace/components/toggle-button";
import Checkbox from "@docspace/components/checkbox";
import FieldContainer from "@docspace/components/field-container";

import { StyledComponent } from "../StyledComponent";
import { SMTPSettingsFields } from "../constants";
import { EmailInput } from "@docspace/components";
import ButtonContainer from "./ButtonContainer";

const {
  HOST,
  PORT,
  SENDER_EMAIL_ADDRESS,
  SENDER_DISPLAY_NAME,
  HOST_LOGIN,
  ENABLE_SSL,
  HOST_PASSWORD,
  AUTHENTICATION,
  USE_NTLM,
} = SMTPSettingsFields;

const CustomSettings = (props) => {
  const { t, settings, setSMTPSettings, isLoading, theme } = props;
  const [emailError, setEmailError] = useState({
    hasError: false,
    isValid: true,
    errors: [],
  });

  const onChange = (e) => {
    const { name, value } = e.target;

    setSMTPSettings({
      ...settings,
      [name]: value,
    });
  };

  const onChangeToggle = (e) => {
    const { checked } = e.currentTarget;

    setSMTPSettings({
      ...settings,
      [AUTHENTICATION]: checked,
    });
  };

  const onChangeCheckbox = (e) => {
    const { checked, name } = e.target;

    setSMTPSettings({
      ...settings,
      [name]: checked,
    });
  };

  const onValidateEmailInput = (result) => {
    const { isValid, errors } = result;

    setEmailError({
      ...emailError,
      isValid,
      errors,
    });
  };

  const onSetValidationError = (hasError) => {
    setEmailError({
      ...emailError,
      hasError,
    });
  };

  const commonTextProps = {
    fontWeight: 600,
  };

  const requirementColor = {
    color: theme.client.settings.integration.smtp.requirementColor,
  };
  const enableAuthComponent = (
    <div className="smtp-settings_auth">
      <ToggleButton
        className="smtp-settings_toggle"
        isChecked={settings[AUTHENTICATION]}
        onChange={onChangeToggle}
        label={t("Authentication")}
        isDisabled={isLoading}
      />

      <div className="smtp-settings_title smtp-settings_login">
        <Text {...commonTextProps}>{t("HostLogin")}</Text>
        <Text as="span" color="#F21C0E">
          *
        </Text>
      </div>
      <TextInput
        className="smtp-settings_input"
        name={HOST_LOGIN}
        placeholder={t("EnterLogin")}
        onChange={onChange}
        value={settings[HOST_LOGIN]}
        isDisabled={isLoading || !settings[AUTHENTICATION]}
        scale
      />

      <div className="smtp-settings_title">
        <Text {...commonTextProps}>{t("HostPassword")}</Text>
        <Text as="span" color="#F21C0E">
          *
        </Text>
      </div>
      <TextInput
        className="smtp-settings_input"
        name={HOST_PASSWORD}
        placeholder={t("EnterPassword")}
        onChange={onChange}
        value={settings[HOST_PASSWORD]}
        isDisabled={isLoading || !settings[AUTHENTICATION]}
        scale
      />

      <Checkbox
        name={USE_NTLM}
        label={t("AuthViaNTLM")}
        isChecked={settings[USE_NTLM]}
        onChange={onChangeCheckbox}
        isDisabled={isLoading || !settings[AUTHENTICATION]}
      />
    </div>
  );

  return (
    <StyledComponent>
      <div className="smtp-settings_title">
        <Text {...commonTextProps}>{t("Host")}</Text>
        <Text as="span" {...requirementColor}>
          *
        </Text>
      </div>
      <TextInput
        isDisabled={isLoading}
        className="smtp-settings_input"
        name={HOST}
        placeholder={t("EnterDomain")}
        onChange={onChange}
        value={settings[HOST]}
        scale
      />

      <div className="smtp-settings_title">
        <Text {...commonTextProps}>{t("Port")}</Text>{" "}
        <Text as="span" {...requirementColor}>
          *
        </Text>
      </div>
      <TextInput
        isDisabled={isLoading}
        className="smtp-settings_input"
        name={PORT}
        placeholder={t("EnterPort")}
        onChange={onChange}
        value={settings[PORT].toString()}
        scale
      />
      {enableAuthComponent}

      <Text {...commonTextProps}>{t("SenderDisplayName")}</Text>
      <TextInput
        isDisabled={isLoading}
        className="smtp-settings_input"
        name={SENDER_DISPLAY_NAME}
        placeholder={t("EnterName")}
        onChange={onChange}
        value={settings[SENDER_DISPLAY_NAME]}
        scale
      />

      <div className="smtp-settings_title">
        <Text {...commonTextProps}>{t("SenderEmailAddress")}</Text>
        <Text as="span" {...requirementColor}>
          *
        </Text>
      </div>
      <FieldContainer
        className="smtp-settings_input"
        isVertical
        place="top"
        hasError={emailError.hasError}
        errorMessage={t("Common:IncorrectEmail")}
      >
        <EmailInput
          isDisabled={isLoading}
          value={settings[SENDER_EMAIL_ADDRESS]}
          onChange={onChange}
          onValidateInput={onValidateEmailInput}
          hasError={emailError.hasError}
          placeholder={t("EnterEmail")}
          scale
        />
      </FieldContainer>

      <Checkbox
        isDisabled={isLoading}
        name={ENABLE_SSL}
        label={t("EnableSSL")}
        isChecked={settings[ENABLE_SSL]}
        onChange={onChangeCheckbox}
      />
      <ButtonContainer
        t={t}
        onSetValidationError={onSetValidationError}
        isEmailValid={emailError.isValid}
      />
    </StyledComponent>
  );
};

export default inject(({ auth, setup }) => {
  const { settingsStore } = auth;
  const { theme } = settingsStore;
  const { integration, setSMTPSettings } = setup;
  const { smtpSettings } = integration;
  const { settings, isLoading } = smtpSettings;

  return { theme, settings, setSMTPSettings, isLoading };
})(observer(CustomSettings));
