import React from "react";
import { storiesOf } from "@storybook/react";
import { text, boolean, withKnobs } from "@storybook/addon-knobs/react";
import SelectorAddButton from ".";
import Section from "../../../.storybook/decorators/section";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";

storiesOf("Components|SelectorAddButton", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => {
    const isDisabled = boolean("isDisabled", false);
    return (
      <Section>
        <SelectorAddButton
          isDisabled={isDisabled}
          title={text("text", "Add item")}
          onClick={(e) => {
            !isDisabled && console.log("onClose", e);
          }}
        />
      </Section>
    );
  });
