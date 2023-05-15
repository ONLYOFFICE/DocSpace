import React, { useState } from "react";
import FillingRoleSelector from ".";

export default {
  title: "Components/FillingRoleSelector",
  component: FillingRoleSelector,
};

const Template = ({ props }) => {
  return (
    <>
      <FillingRoleSelector {...props} />
    </>
  );
};

export const Default = Template.bind({});
Default.args = {};
