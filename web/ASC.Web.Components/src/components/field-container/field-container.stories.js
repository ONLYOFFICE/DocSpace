import React from "react";
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import { StringValue } from "react-values";
import { text, boolean, withKnobs } from "@storybook/addon-knobs/react";
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
      onChange={e => {
        action("onChange")(e);
      }}
    >
      {({ value, set }) => (
        <Section>
          <FieldContainer
            isVertical={boolean("isVertical", false)}
            isRequired={boolean("isRequired", false)}
            hasError={boolean("hasError", false)}
            labelText={text("labelText", "Name:")}
            horLabelWidth={text("horLabelWidth", "110px")}
          >
            <TextInput
              value={value}
              hasError={boolean("hasError", false)}
              className="field-input"
              onChange={e => {
                set(e.target.value);
              }}
            />
          </FieldContainer>
        </Section>
      )}
    </StringValue>
  ))
  .add("with tooltip", () => (
    <StringValue
      onChange={e => {
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
              labelText={text("labelText", "Name:")}
              tooltipContent={"Paste you tooltip content here"}
              place="top"
            >
              <TextInput
                value={value}
                hasError={boolean("hasError", false)}
                className="field-input"
                onChange={e => {
                  set(e.target.value);
                }}
              />
            </FieldContainer>
          </div>
          <div style={{ marginTop: 200, marginLeft: 50 }}>
            <FieldContainer
              isVertical={boolean("isVertical", false)}
              isRequired={boolean("isRequired", false)}
              hasError={boolean("hasError", false)}
              labelText={text("labelText", "Name:")}
              tooltipContent={"Paste you tooltip content here"}
              place="top"
            >
              <TextInput
                value={value}
                hasError={boolean("hasError", false)}
                className="field-input"
                onChange={e => {
                  set(e.target.value);
                }}
              />
            </FieldContainer>
          </div>
        </Section>
      )}
    </StringValue>
  ));
