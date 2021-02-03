import React from "react";
import { storiesOf } from "@storybook/react";
import { text, boolean, withKnobs } from "@storybook/addon-knobs/react";
import SelectedItem from ".";
import Section from "../../../.storybook/decorators/section";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";

function onClose(e) {
  console.log("onClose", e);
}

storiesOf("Components|SelectedItem", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => {
    return (
      <Section>
        <SelectedItem
          text={text("text", "Selected item")}
          isInline={boolean("isInline", true)}
          onClose={onClose}
          isDisabled={boolean("isDisabled", false)}
        />
      </Section>
    );
  });
