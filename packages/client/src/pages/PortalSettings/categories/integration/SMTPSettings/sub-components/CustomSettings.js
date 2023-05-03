import React, { useState } from "react";
import { inject, observer } from "mobx-react";

import TextInput from "@docspace/components/text-input";
import Text from "@docspace/components/text";
import ToggleButton from "@docspace/components/toggle-button";

import StyledComponent from "../StyledComponent";
import Checkbox from "@docspace/components/checkbox";

const HOST = "host",
  PORT = "port",
  SENDER_EMAIL_ADDRESS = "senderAddress",
  SENDER_DISPLAY_NAME = "senderDisplayName",
  HOST_LOGIN = "credentialsUserName",
  HOST_PASSWORD = "credentialsUserPassword",
  ENABLE_SSL = "enableSSL",
  AUTHENTICATION = "enableAuth",
  USE_NTLM = "useNtlm";
const CustomSettings = (props) => {
  const { t, smtpSettings } = props;
  const [state, setState] = useState(smtpSettings);

  const onChange = (e) => {
    const { name, value } = e.target;

    setState({
      ...state,
      [name]: value,
    });
  };

  const onChangeToggle = (e) => {
    const { checked } = e.currentTarget;

    setState({
      ...state,
      [AUTHENTICATION]: checked,
    });
  };

  const onChangeCheckbox = (e) => {
    const { checked, name } = e.target;

    setState({
      ...state,
      [name]: checked,
    });
  };

  const commonTextProps = {
    fontWeight: 600,
  };

  const enableAuthComponent = (
    <div className="smtp-settings_auth">
      <ToggleButton
        className="smtp-settings_toggle"
        isChecked={state[AUTHENTICATION]}
        onChange={onChangeToggle}
        label={t("Authentication")}
      />
      <Text {...commonTextProps} className="smtp-settings_login">
        {t("HostLogin")}
      </Text>
      <TextInput
        className="smtp-settings_input"
        name={HOST_LOGIN}
        placeholder={t("EnterLogin")}
        onChange={onChange}
        value={state[HOST_LOGIN]}
        isDisabled={!state[AUTHENTICATION]}
        scale
      />
      <Text {...commonTextProps}>{t("HostPassword")}</Text>

      <TextInput
        className="smtp-settings_input"
        name={HOST_PASSWORD}
        placeholder={t("EnterPassword")}
        onChange={onChange}
        value={state[HOST_PASSWORD]}
        isDisabled={!state[AUTHENTICATION]}
        scale
      />

      <Checkbox
        name={USE_NTLM}
        label={t("AuthViaNTLM")}
        isChecked={state[USE_NTLM]}
        onChange={onChangeCheckbox}
        isDisabled={!state[AUTHENTICATION]}
      />
    </div>
  );

  console.log("state", state);
  return (
    <StyledComponent>
      <Text {...commonTextProps}>{t("Host")}</Text>
      <TextInput
        className="smtp-settings_input"
        name={HOST}
        placeholder={t("EnterDomain")}
        onChange={onChange}
        value={state[HOST]}
        scale
      />
      <Text {...commonTextProps}>{t("Port")}</Text>
      <TextInput
        className="smtp-settings_input"
        name={PORT}
        placeholder={t("EnterPort")}
        onChange={onChange}
        value={state[PORT].toString()}
        scale
      />

      {enableAuthComponent}

      <Text {...commonTextProps}>{t("SenderDisplayName")}</Text>
      <TextInput
        className="smtp-settings_input"
        name={SENDER_DISPLAY_NAME}
        placeholder={t("EnterName")}
        onChange={onChange}
        value={state[SENDER_DISPLAY_NAME]}
        scale
      />
      <Text {...commonTextProps}>{t("SenderEmailAddress")}</Text>
      <TextInput
        className="smtp-settings_input"
        name={SENDER_EMAIL_ADDRESS}
        placeholder={t("EnterEmail")}
        onChange={onChange}
        value={state[SENDER_EMAIL_ADDRESS]}
        scale
      />

      <Checkbox
        name={ENABLE_SSL}
        label={t("EnableSSL")}
        isChecked={state[ENABLE_SSL]}
        onChange={onChangeCheckbox}
      />
    </StyledComponent>
  );
};

export default inject(({ setup }) => {
  const { integration } = setup;
  const { smtpSettings } = integration;

  return { smtpSettings };
})(observer(CustomSettings));
