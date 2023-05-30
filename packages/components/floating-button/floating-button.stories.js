import React from "react";

import FloatingButton from ".";

export default {
  title: "components/FloatingButton",
  component: FloatingButton,
};

const Template = (args) => <FloatingButton {...args} />;

export const Default = Template.bind({});

Default.args = {
  id: undefined,
  className: undefined,
  style: undefined,
  icon: "upload",
  alert: false,
  percent: 0,
};
