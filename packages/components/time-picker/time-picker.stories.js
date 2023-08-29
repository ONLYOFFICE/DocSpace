import React from "react";
import TimePicker from "./";

export default {
  title: "Components/TimePicker",
  component: TimePicker,
  argTypes: {
    hasError: { control: "boolean" },
    onChange: { action: "onChange" },
  },
  parameters: {
    docs: {
      description: {
        component: "Time input",
      },
    },
  },
};

const Template = ({ ...args }) => {
  return <TimePicker hasError={false} {...args} />;
};

export const Default = Template.bind({});
