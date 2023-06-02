import React from "react";

import FloatingButton from ".";

export default {
  title: "components/FloatingButton",
  component: FloatingButton,
};

const Template = (args) => (
  <div
    style={{
      height: "600px",
      display: "flex",
      justifyContent: "flex-start",
      position: "relative",
    }}
  >
    <FloatingButton {...args} />
  </div>
);

export const Default = Template.bind({});

Default.args = {
  id: undefined,
  className: undefined,
  style: undefined,
  icon: "upload",
  alert: false,
  percent: 0,
};
