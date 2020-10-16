import React from "react";
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import { StringValue } from "react-values";
import { text, boolean, withKnobs, color } from "@storybook/addon-knobs/react";
import FieldContainer from ".";
import TextInput from "../text-input";
import Section from "../../../.storybook/decorators/section";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";

storiesOf("Components|FieldContainer", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => (
    <StringValue
      onChange={(e) => {
        action("onChange")(e);
      }}
    >
      {({ value, set }) => (
        <Section>
          <div style={{ marginTop: 100, marginLeft: 50 }}>
            <FieldContainer
              isVertical={boolean("isVertical", false)}
              isRequired={boolean("isRequired", false)}
              hasError={boolean("hasError", false)}
              labelVisible={boolean("labelVisible", true)}
              labelText={text("labelText", "Name:")}
              maxLabelWidth={text("maxLabelWidth", "110px")}
              tooltipContent={text(
                "tooltipContent",
                "Paste you tooltip content here"
              )}
              helpButtonHeaderContent={text(
                "helpButtonHeaderContent",
                "Tooltip header"
              )}
              place="top"
              errorMessage={text(
                "errorMessage",
                "Error text. Lorem ipsum dolor sit amet, consectetuer adipiscing elit"
              )}
              errorColor={color("errorColor", "#C96C27")}
              errorMessageWidth={text("errorMessageWidth", "293px")}
            >
              <TextInput
                value={value}
                hasError={boolean("hasError", false)}
                className="field-input"
                onChange={(e) => {
                  set(e.target.value);
                }}
              />
            </FieldContainer>
          </div>
        </Section>
      )}
    </StringValue>
  ));
