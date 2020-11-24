import React from "react";
import { storiesOf } from "@storybook/react";
import { StringValue } from "react-values";
import { withKnobs, boolean } from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import PasswordInput from ".";
import TextInput from "../text-input";
import Section from "../../../.storybook/decorators/section";

storiesOf("Components|Input", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("password input", () => {
    const isDisabled = boolean("isDisabled", false);
    const settingsUpperCase = boolean("settingsUpperCase", false);
    const settingsDigits = boolean("settingsDigits", false);
    const settingsSpecSymbols = boolean("settingsSpecSymbols", false);
    const simpleView = boolean("simpleView", false);
    const hideNewPasswordButton = boolean("hideNewPasswordButton", false);
    const isDisableTooltip = boolean("isDisableTooltip", false);
    const isTextTooltipVisible = boolean("isTextTooltipVisible", false);

    const fakeSettings = {
      minLength: 6,
      upperCase: settingsUpperCase,
      digits: settingsDigits,
      specSymbols: settingsSpecSymbols,
    };

    const tooltipPasswordLength =
      "from " + fakeSettings.minLength + " to 30 characters";

    return (
      <Section>
        <div style={{ height: "110px" }}></div>
        <TextInput
          name="demoEmailInput"
          size="base"
          isDisabled={isDisabled}
          isReadOnly={true}
          scale={true}
          value="demo@gmail.com"
        />
        <br />
        <StringValue>
          {({ value, set }) => (
            <PasswordInput
              simpleView={simpleView}
              inputName="demoPasswordInput"
              emailInputName="demoEmailInput"
              inputValue={value}
              onChange={(e) => {
                set(e.target.value);
              }}
              clipActionResource="Copy e-mail and password"
              clipEmailResource="E-mail: "
              clipPasswordResource="Password: "
              clipCopiedResource="Copied"
              hideNewPasswordButton={hideNewPasswordButton}
              isDisableTooltip={isDisableTooltip}
              isTextTooltipVisible={isTextTooltipVisible}
              tooltipPasswordTitle="Password must contain:"
              tooltipPasswordLength={tooltipPasswordLength}
              tooltipPasswordDigits="digits"
              tooltipPasswordCapital="capital letters"
              tooltipPasswordSpecial="special characters (!@#$%^&*)"
              generatorSpecial="!@#$%^&*"
              passwordSettings={fakeSettings}
              isDisabled={isDisabled}
              placeholder="password"
              maxLength={30}
              onValidateInput={(a) => console.log(a)}
              onCopyToClipboard={(b) =>
                console.log("Data " + b + " copied to clipboard")
              }
              //tooltipOffsetLeft={150}
            />
          )}
        </StringValue>
      </Section>
    );
  });
