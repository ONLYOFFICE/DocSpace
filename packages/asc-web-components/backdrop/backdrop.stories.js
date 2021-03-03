import React, { useState } from "react";

import Backdrop from "./";
import Button from "../button";

export default {
  title: "Components/Backdrop",
  component: Backdrop,
  argTypes: {
    visible: {
      description: "Display or not",
    },
    zIndex: {
      description: "CSS z-index",
    },
    className: { description: "Accepts class" },
    id: { description: "Accepts id" },
    style: { description: "Accepts CSS style" },
    withBackground: {
      description:
        "The background is not displayed if the viewport width is less than 1024, set it to true for display",
    },
    isAside: { description: "Must be true if used with Aside component" },
    onClick: { action: "On Hide", table: { disable: true } },
  },
  parameters: {
    docs: {
      description: {
        component: "Backdrop for displaying modal dialogs or other components",
      },
      source: {
        code: `
      import Backdrop from "@appserver/components/backdrop";

<Backdrop visible={true} zIndex={200}/>`,
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
        size="medium"
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
