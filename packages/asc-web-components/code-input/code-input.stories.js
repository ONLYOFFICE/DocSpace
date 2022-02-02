import React from "react";
import CodeInput from ".";

export default {
  title: "Components/CodeInput",
  component: CodeInput,
  parameters: {
    docs: {
      description: {
        component: "Used to display an code input.",
      },
    },
  },
};

const Template = (args) => <CodeInput {...args} />;

export const Default = Template.bind({});
