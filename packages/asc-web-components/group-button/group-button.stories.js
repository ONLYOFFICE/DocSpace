import React from "react";
import { storiesOf } from "@storybook/react";
import { withKnobs, boolean, text } from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import GroupButton from ".";
import DropDownItem from "../drop-down-item";

storiesOf("Components|GroupButton", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => (
    <GroupButton
      label={text("Label", "Base group button")}
      disabled={boolean("disabled", false)}
      isDropdown={boolean("isDropdown", false)}
      opened={boolean("opened", false)}
    >
      <DropDownItem label="Action 1" />
      <DropDownItem label="Action 2" />
      <DropDownItem label="Action 3" />
    </GroupButton>
  ));
