import React from "react";
import SaveCancelButtons from "./";

export default {
  title: "Components/SaveCancelButtons",
  component: SaveCancelButtons,
  parameters: {
    docs: {
      description: {
        component:
          "Save and cancel buttons are located in the settings sections.",
      },
    },
  },
  argTypes: {
    onSaveClick: { action: "onSaveClick" },
    onCancelClick: { action: "onCancelClick" },
  },
};

const Template = ({ onSaveClick, onCancelClick, ...args }) => {
  return (
    <div style={{ position: "relative", height: "60px" }}>
      <SaveCancelButtons
        {...args}
        onSaveClick={() => onSaveClick("on Save button clicked")}
        onCancelClick={() => onCancelClick("on Cancel button clicked")}
      />
    </div>
  );
};

export const Default = Template.bind({});
Default.args = {
  showReminder: false,
  reminderTest: "You have unsaved changes",
  saveButtonLabel: "Save",
  cancelButtonLabel: "Cancel",
};
