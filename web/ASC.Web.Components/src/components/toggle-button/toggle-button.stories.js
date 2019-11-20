import React from "react";
import { storiesOf } from "@storybook/react";
import { withKnobs, boolean, text } from "@storybook/addon-knobs/react";
import { action } from "@storybook/addon-actions";
import { BooleanValue } from "react-values";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import ToggleButton from ".";
import Section from "../../../.storybook/decorators/section";

storiesOf("Components|Input", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("toggle button", () => {
    return (
      <Section>
        <BooleanValue>
          {({ value, toggle }) => (
            <ToggleButton
              isChecked={value}
              isDisabled={boolean("isDisabled", false)}
              label={text("label", "label text")}
              onChange={e => {
                toggle(e.target.checked);
                action("onChange")(e);
              }}
            />
          )}
        </BooleanValue>
      </Section>
    );
  });
