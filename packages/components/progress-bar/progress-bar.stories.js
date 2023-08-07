import React from "react";
import ProgressBar from "./";

export default {
  title: "Components/ProgressBar",
  component: ProgressBar,
  parameters: {
    docs: {
      description: {
        component:
          "A container that displays a process or operation as a progress bar",
      },
    },
  },
};

const Template = (args) => {
  return <ProgressBar {...args} />;
};
export const Default = Template.bind({});
Default.args = {
  label: "Operation in progress...",
  percent: 20,
};
