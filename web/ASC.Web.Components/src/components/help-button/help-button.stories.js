import React from "react";
import { storiesOf } from "@storybook/react";
import { withKnobs } from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import HelpButton from ".";
import Section from "../../../.storybook/decorators/section";

storiesOf("Components|Buttons", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("help button", () => {
    return (
      <Section>
        <div style={{marginTop: 70, marginLeft: 70}}>
          <HelpButton tooltipContent="tooltipContent" />
        </div>
        <div style={{marginTop: 20, marginLeft: 70}}>
          <HelpButton tooltipContent="tooltipContent_2" />
        </div>
        <div style={{marginTop: 20, marginLeft: 70}}>
          <HelpButton tooltipContent="tooltipContent_3" />
        </div>
      </Section>
    );
  });
