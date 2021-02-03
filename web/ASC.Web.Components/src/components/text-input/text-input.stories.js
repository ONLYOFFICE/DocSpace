import React from "react";
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import { StringValue } from "react-values";
import {
  withKnobs,
  boolean,
  text,
  select,
  number,
} from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import TextInput from ".";

import Section from "../../../.storybook/decorators/section";

const sizeOptions = ["base", "middle", "big", "huge", "large"];

storiesOf("Components|Input", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("text", () => (
    <StringValue
      onChange={(e) => {
        action("onChange")(e);
      }}
    >
      {({ value, set }) => (
        <Section>
          <TextInput
            id={text("id", "")}
            name={text("name", "")}
            placeholder={text("placeholder", "This is placeholder")}
            maxLength={number("maxLength", 255)}
            size={select("size", sizeOptions, "base")}
            onBlur={action("onBlur")}
            onFocus={action("onFocus")}
            isAutoFocussed={boolean("isAutoFocussed", false)}
            isDisabled={boolean("isDisabled", false)}
            isReadOnly={boolean("isReadOnly", false)}
            hasError={boolean("hasError", false)}
            hasWarning={boolean("hasWarning", false)}
            scale={boolean("scale", false)}
            autoComplete={text("autoComplete", "off")}
            tabIndex={number("tabIndex", 1)}
            withBorder={boolean("withBorder", true)}
            mask={text("mask", null)}
            value={value}
            onChange={(e) => {
              set(e.target.value);
            }}
          />
        </Section>
      )}
    </StringValue>
  ));
