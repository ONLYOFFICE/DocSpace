import React, { useState } from "react";
import TextInput from "./";

export default {
  title: "Components/TextInput",
  component: TextInput,
  parameters: {
    docs: {
      description: {
        component: "Input field for single-line strings",
      },
    },
  },
  argTypes: {
    onBlur: { action: "onBlur" },
    onFocus: { action: "onFocus" },
    onChange: { action: "onChange" },
  },
};

const Template = ({ onChange, value, ...args }) => {
  const [val, setValue] = useState(value);

  return (
    <TextInput
      {...args}
      value={val}
      onChange={(e) => {
        setValue(e.target.value);
        onChange(e.target.value);
      }}
    />
  );
};

export const Default = Template.bind({});
Default.args = {
  id: "",
  name: "",
  placeholder: "This is placeholder",
  maxLength: 255,
  size: "base",
  isAutoFocussed: false,
  isDisabled: false,
  isReadOnly: false,
  hasError: false,
  hasWarning: false,
  scale: false,
  autoComplete: "off",
  tabIndex: 1,
  withBorder: true,
  mask: null,
  value: "",
};
