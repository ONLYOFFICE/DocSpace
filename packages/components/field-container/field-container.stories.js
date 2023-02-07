import React, { useState } from "react";
import FieldContainer from ".";
import TextInput from "../text-input";

export default {
  title: "Components/FieldContainer",
  component: FieldContainer,
  argTypes: {
    errorColor: { control: "color" },
  },
  parameters: {
    docs: {
      description: {
        component: "Responsive form field container",
      },
    },
  },
};

const Template = (args) => {
  const [value, setValue] = useState("");
  return (
    <FieldContainer {...args}>
      <TextInput
        value={value}
        hasError={args.hasError}
        className="field-input"
        onChange={(e) => {
          setValue(e.target.value);
        }}
      />
    </FieldContainer>
  );
};

export const Default = Template.bind({});
Default.args = {
  isVertical: false,
  isRequired: false,
  hasError: false,
  labelVisible: true,
  labelText: "Name:",
  maxLabelWidth: "110px",
  tooltipContent: "Paste you tooltip content here",
  helpButtonHeaderContent: "Tooltip header",
  place: "top",
  errorMessage:
    "Error text. Lorem ipsum dolor sit amet, consectetuer adipiscing elit",
  errorColor: "#C96C27",
  errorMessageWidth: "293px",
};
