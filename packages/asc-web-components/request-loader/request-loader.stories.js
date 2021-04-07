import React from "react";
import RequestLoader from "./";

export default {
  title: "Components/Loaders",
  component: RequestLoader,
  parameters: {
    docs: {
      description: {
        component:
          "equestLoader component is used for displaying loading actions on a page",
      },
    },
  },
  argTypes: {
    loaderColor: { control: "color" },
    fontColor: { control: "color" },
  },
};

const Template = (args) => {
  return <RequestLoader {...args} />;
};

export const Default = Template.bind({});
Default.args = {
  visible: true,
  zIndex: 256,
  loaderSize: "16px",
  loaderColor: "#999",
  label: "Loading... Please wait...",
  fontSize: "12px",
  fontColor: "#999",
};
