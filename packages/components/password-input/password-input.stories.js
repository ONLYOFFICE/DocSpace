import React, { useState, useEffect } from "react";
import PasswordInput from ".";
import TextInput from "../text-input";

const Template = ({
  settingMinLength,
  settingsUpperCase,
  settingsDigits,
  settingsSpecSymbols,
  tooltipPasswordLength,
  onChange,
  onValidateInput,
  onCopyToClipboard,
  ...args
}) => {
  const [value, setValue] = useState("");
  const [fakeSettings, setFakSettings] = useState();

  useEffect(() => {
    setFakSettings({
      minLength: settingMinLength,
      upperCase: settingsUpperCase,
      digits: settingsDigits,
      specSymbols: settingsSpecSymbols,
    });
    setValue("");
  }, [
    settingMinLength,
    settingsUpperCase,
    settingsDigits,
    settingsSpecSymbols,
  ]);

  const onChangeHandler = (e) => {
    onChange(e.currentTarget.value);
    setValue(e.currentTarget.value);
  };

  const onValidateInputHandler = (e) => {
    onValidateInput(e);
  };

  return (
    <div style={{ height: "110px", display: "grid", gridGap: "24px" }}>
      <TextInput
        name="demoEmailInput"
        size="base"
        isDisabled={args.isDisabled}
        isReadOnly={true}
        scale={true}
        value="demo@gmail.com"
      />

      <PasswordInput
        {...args}
        inputValue={value}
        onChange={onChangeHandler}
        tooltipPasswordLength={`${tooltipPasswordLength} ${settingMinLength}`}
        passwordSettings={fakeSettings}
        onValidateInput={onValidateInputHandler}
        tooltipOffsetLeft={150}
      />
    </div>
  );
};

export const basic = Template.bind({});
basic.args = {
  isDisabled: false,
  settingMinLength: 6,
  settingsUpperCase: false,
  settingsDigits: false,
  settingsSpecSymbols: false,
  simpleView: false,
  inputName: "demoPasswordInput",
  emailInputName: "demoEmailInput",
  isDisableTooltip: false,
  isTextTooltipVisible: false,
  tooltipPasswordTitle: "Password must contain:",
  tooltipPasswordLength: "minimum length: ",
  tooltipPasswordDigits: "digits",
  tooltipPasswordCapital: "capital letters",
  tooltipPasswordSpecial: "special characters (!@#$%^&*)",
  generatorSpecial: "!@#$%^&*",
  placeholder: "password",
  maxLength: 30,
};
