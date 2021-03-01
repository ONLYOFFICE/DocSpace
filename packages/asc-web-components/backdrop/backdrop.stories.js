import React from "react";
import { BooleanValue } from "react-values";

import Backdrop from "./";
import Button from "../button";

export default {
  title: "Components/Backdrop",
  component: Backdrop,
  argTypes: {
    visible: {
      control: false,
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
    onClickButton: { action: "On Show", table: { disable: true } },
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
  decorators: [
    (Story, props) => (
      <BooleanValue>
        {({ value, toggle }) => {
          props.args.value = value;
          props.args.toggle = toggle;
          console.log(props);

          return (
            <>
              <Button
                label="Show Backdrop"
                primary
                size="medium"
                onClick={(e) => {
                  props.args.onClickButton(e);
                  toggle(true);
                }}
              />
              <Story />
            </>
          );
        }}
      </BooleanValue>
    ),
  ],
};

const Template = (args) => (
  <Backdrop
    {...args}
    visible={args.value}
    onClick={(e) => {
      args.onClick(e);
      args.toggle();
    }}
  />
);

export const Default = Template.bind({});
