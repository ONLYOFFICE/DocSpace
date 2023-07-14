import React from "react";
import styled from "styled-components";
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

const StyledWrapper = styled.div`
  position: relative;
  height: 300px;

  .positionAbsolute {
    position: absolute;
  }
`;

const Template = ({ onSaveClick, onCancelClick, ...args }) => {
  const isAutoDocs =
    typeof window !== "undefined" && window?.location?.href.includes("docs");

  return (
    <StyledWrapper>
      <SaveCancelButtons
        {...args}
        className={
          isAutoDocs && !args.displaySettings
            ? `positionAbsolute ${args.className}`
            : args.className
        }
        onSaveClick={() => onSaveClick("on Save button clicked")}
        onCancelClick={() => onCancelClick("on Cancel button clicked")}
      />
    </StyledWrapper>
  );
};

export const Default = Template.bind({});
Default.args = {
  showReminder: false,
  reminderTest: "You have unsaved changes",
  saveButtonLabel: "Save",
  cancelButtonLabel: "Cancel",
};
