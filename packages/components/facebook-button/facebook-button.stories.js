import React from "react";
import FacebookButton from ".";

export default {
  title: "Components/FacebookButton",
  component: FacebookButton,
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
  return (
    <div style={{ width: 100 }}>
      <FacebookButton {...args} />
    </div>
  );
};

export const Default = Template.bind({});
Default.args = {
  label: "facebook",
};
