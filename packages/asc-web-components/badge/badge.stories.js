import React from "react";

import Badge from "./";

export default {
  title: "Components/Badge",
  component: Badge,
  argTypes: {
    backgroundColor: { control: "color", description: "CSS background-color" },
    color: { control: "color", description: "CSS color" },
    label: { control: "text", description: "Value" },
    borderRadius: { description: "CSS border-radius" },
    className: { description: "Accepts class" },
    fontSize: { description: "CSS font-size" },
    fontWeight: { description: "CSS font-weight" },
    id: { description: "Accepts id" },
    maxWidth: { description: "CSS max-width" },
    onClick: { description: "onClick event" },
    padding: { description: "CSS padding" },
    style: { description: "Accepts css style" },
  },
};

const Template = (args) => <Badge {...args} />;
const NumberTemplate = (args) => <Badge {...args} />;
const TextTemplate = (args) => <Badge {...args} />;
const MixedTemplate = (args) => <Badge {...args} />;

export const Default = Template.bind({});
Default.args = {
  label: 24,
};
export const NumberBadge = NumberTemplate.bind({});
NumberBadge.argTypes = {
  label: { control: "number" },
};
NumberBadge.args = {
  label: 3,
};
export const TextBadge = TextTemplate.bind({});
TextBadge.argTypes = {
  label: { control: "text" },
};
TextBadge.args = {
  label: "New",
};
export const MixedBadge = MixedTemplate.bind({});
MixedBadge.argTypes = {
  label: { control: "text" },
};
MixedBadge.args = {
  label: "Ver.2",
};
