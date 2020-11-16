import React from "react";
import { storiesOf } from "@storybook/react";
import { withKnobs, select, boolean } from "@storybook/addon-knobs/react";
import Section from "../../../.storybook/decorators/section";

import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import FloatingButton from "./index";
import { bool } from "prop-types";

storiesOf("Components|Floating Button", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("floating button", () => {
    const iconVariables = ["upload", "file", "trash", "move", "duplicate"];
    return (
      <Section>
        <h1>Floating button</h1>
        <FloatingButton
          icon={select("icon", iconVariables, "upload")}
          alert={boolean("alert", false)}
        />
      </Section>
    );
  });
