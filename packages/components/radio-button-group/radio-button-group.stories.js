import React from "react";
import RadioButtonGroup from "./";

const disable = {
  table: {
    disable: true,
  },
};

export default {
  title: "Components/RadioButtonGroup",
  component: RadioButtonGroup,
  parameters: {
    docs: {
      description: {
        component: "RadioButtonGroup allow you to add group radiobutton",
      },
    },
  },
  argTypes: {
    options: {
      control: {
        type: "multi-select",
      },
      options: ["radio1", "radio2", "radio3"],
    },
    onClick: {
      cation: "onClick",
    },
    labelFirst: disable,
    labelSecond: disable,
    labelThird: disable,
  },
};

const Template = ({
  options,
  onClick,
  labelFirst,
  labelSecond,
  labelThird,
  ...args
}) => {
  const values = ["first", "second", "third"];
  const updateOptions = (options) => {
    const updatedOptions = options.map((item) => {
      switch (item) {
        case "radio1":
          return {
            value: values[0],
            label: labelFirst,
          };
        case "radio2":
          return {
            value: values[1],
            label: labelSecond,
          };
        case "radio3":
          return {
            value: values[2],
            label: labelThird,
          };
        default:
          break;
      }
    });
    return updatedOptions;
  };

  const updatedOptions = updateOptions(options);

  return (
    <RadioButtonGroup
      {...args}
      options={updatedOptions}
      selected={updatedOptions[0].value}
      onClick={(e) => {
        onClick(e);
      }}
    />
  );
};

export const Default = Template.bind({});
Default.args = {
  orientation: "horizontal",
  width: "100%",
  isDisabled: false,
  fontSize: "13px",
  fontWeight: "400",
  spacing: "15px",
  name: "group",
  options: ["radio1", "radio2", "radio3"],
  labelFirst: "First radiobtn",
  labelSecond: "Second radiobtn",
  labelThird: "Third radiobtn",
};
