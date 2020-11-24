import React from "react";
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import {
  withKnobs,
  boolean,
  select,
  color,
  number,
} from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import IconButton from ".";
import { Icons } from "../icons";
import Section from "../../../.storybook/decorators/section";

storiesOf("Components|Buttons", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("icon button", () => {
    const iconNames = Object.keys(Icons);

    return (
      <Section>
        <IconButton
          color={color("color", "#d0d5da")}
          size={number("size", 25)}
          iconName={select("iconName", iconNames, "SearchIcon")}
          isFill={boolean("isFill", false)}
          isDisabled={boolean("isDisabled", false)}
          onClick={action("clicked")}
        />
      </Section>
    );
  });
