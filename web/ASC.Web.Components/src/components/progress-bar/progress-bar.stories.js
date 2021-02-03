import React from "react";
import { storiesOf } from "@storybook/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import { withKnobs, number, text } from "@storybook/addon-knobs/react";
import ProgressBar from "./";

storiesOf("Components|ProgressBar", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => (
    <ProgressBar
      style={{ marginTop: 16 }}
      label={text("label", "Uploading files: 20 of 100")}
      percent={number("value", 20)}
      dropDownContent={text("dropDownContent", "You content here")}
    />
  ));
