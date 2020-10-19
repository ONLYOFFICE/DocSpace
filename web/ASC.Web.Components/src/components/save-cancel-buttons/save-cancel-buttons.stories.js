import React from "react";
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import { text, boolean, withKnobs } from "@storybook/addon-knobs/react";
import SaveCancelButtons from ".";
import Section from "../../../.storybook/decorators/section";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";

storiesOf("Components|SaveCancelButtons", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => {
    return (
      <Section>
        <SaveCancelButtons
          onSaveClick={() => action("on Save button clicked")}
          onCancelClick={() => action("on Cancel button clicked")}
          showReminder={boolean("showReminder", false)}
          reminderTest={text("reminderTest", "You have unsaved changes")}
          saveButtonLabel={text("saveButtonLabel", "Save")}
          cancelButtonLabel={text("cancelButtonLabel", "Cancel")}
        />
      </Section>
    );
  });
