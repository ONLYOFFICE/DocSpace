import React from "react";
import SelectorAddButton from "./";

export default {
  title: "Components/SelectorAddButton",
  component: SelectorAddButton,
  argTypes: { onClick: { action: "onClose" } },
};

const Template = ({ onClick, ...args }) => {
  return (
    <SelectorAddButton
      onClick={(e) => {
        !args.isDisabled && onClick(e);
      }}
    />
  );
};

export const Default = Template.bind({});
Default.args = {
  isDisabled: false,
  title: "Add item",
};
