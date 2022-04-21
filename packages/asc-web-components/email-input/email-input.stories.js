import React, { useState } from "react";
import { EmailSettings } from "../utils/email";
import EmailInput from "./";

const Template = ({
  allowDomainPunycode,
  allowLocalPartPunycode,
  allowDomainIp,
  allowStrictLocalPart,
  allowSpaces,
  allowName,
  allowLocalDomainName,
  ...rest
}) => {
  const [emailValue, setEmailValue] = useState("");

  const onChangeHandler = (value) => {
    setEmailValue(value);
  };
  const settings = EmailSettings.parse({
    allowDomainPunycode,
    allowLocalPartPunycode,
    allowDomainIp,
    allowStrictLocalPart,
    allowSpaces,
    allowName,
    allowLocalDomainName,
  });
  return (
    <EmailInput
      {...rest}
      value={emailValue}
      emailSettings={settings}
      onValidateInput={(isEmailValid) => rest.onValidateInput(isEmailValid)}
      onChange={(e) => {
        rest.onChange(e.target.value);
        onChangeHandler(e.target.value);
      }}
    />
  );
};

export const basic = Template.bind({});
basic.args = {
  allowDomainPunycode: false,
  allowLocalPartPunycode: false,
  allowDomainIp: false,
  allowSpaces: false,
  allowName: false,
  allowLocalDomainName: false,
  allowStrictLocalPart: true,
  placeholder: "Input email",
  size: "base",
};
