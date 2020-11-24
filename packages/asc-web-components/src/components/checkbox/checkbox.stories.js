import React from "react";
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import { BooleanValue } from "react-values";
import { withKnobs, boolean, text } from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import Checkbox from ".";
import Section from "../../../.storybook/decorators/section";

storiesOf("Components|Input", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("checkbox", () => (
    <Section>
      <BooleanValue>
        {({ value, toggle }) => (
          <Checkbox
            id={text("id", "id")}
            name={text("name", "name")}
            value={text("value", "value")}
            label={text("label", "label")}
            isChecked={value}
            isIndeterminate={boolean("isIndeterminate", false)}
            isDisabled={boolean("isDisabled", false)}
            onChange={(e) => {
              action("onChange")(e);
              toggle(e.target.checked);
            }}
          />
        )}
      </BooleanValue>
    </Section>
  ));
