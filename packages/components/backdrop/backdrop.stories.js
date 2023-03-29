import React, { useState } from "react";

import Backdrop from "./";
import Button from "../button";

export default {
  title: "Components/Backdrop",
  component: Backdrop,
  subcomponents: { Button },
  argTypes: {
    onClick: { action: "On Hide", table: { disable: true } },
  },
  parameters: {
    docs: {
      description: {
        component: "Backdrop for displaying modal dialogs or other components",
      },
    },
  },
};

const Template = (args) => {
  const [isVisible, setIsVisible] = useState(args.visible);
  const toggleVisible = () => setIsVisible(!isVisible);
  return (
    <>
      <Button
        label="Show Backdrop"
        primary
        size="small"
        onClick={toggleVisible}
      />
      <Backdrop
        {...args}
        visible={isVisible}
        onClick={(e) => {
          args.onClick(e);
          toggleVisible(false);
        }}
      />
    </>
  );
};

export const Default = Template.bind({});
Default.args = {
  withBackground: true,
};
