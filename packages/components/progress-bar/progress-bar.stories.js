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
  return <ProgressBar {...args} style={{ marginTop: 16 }} />;
};
export const Default = Template.bind({});
Default.args = {
  label: "Uploading files: 20 of 100",
  percent: 20,
  dropDownContent: "You content here",
};
