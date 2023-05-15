import React, { useState } from "react";
import FillingRoleSelector from ".";

export default {
  title: "Components/FillingRoleSelector",
  component: FillingRoleSelector,
};

const Template = ({ props }) => {
  return (
    <>
      <FillingRoleSelector {...props} style={{ width: "20%" }} />
    </>
  );
};

export const Default = Template.bind({});
Default.args = {};
