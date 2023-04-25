import React from "react";
import Heading from ".";

export default {
  title: "Components/Heading",
  component: Heading,
  argTypes: {
    color: { control: "color" },
    headerText: { control: "text", description: "Header text" },
  },
  parameters: {
    docs: {
      description: {
        component: "Heading text structured in levels",
      },
    },
  },
};

const Template = ({ headerText, ...args }) => {
  return (
    <div style={{ margin: "7px" }}>
      <Heading {...args}>{headerText}</Heading>
    </div>
  );
};

export const Default = Template.bind({});
Default.args = {
  level: 1,
  title: "",
  truncate: false,
  isInline: false,
  size: "large",
  headerText: "Sample text Heading",
};
