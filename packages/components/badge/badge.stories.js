import React from "react";

import Badge from "./";

export default {
  title: "Components/Badge",
  component: Badge,
  parameters: {
    docs: {
      description: {
        component: "Used for buttons, numbers or status markers next to icons.",
      },
    },
  },
};

const Template = (args) => <Badge {...args} />;
const NumberTemplate = (args) => <Badge {...args} />;
const TextTemplate = (args) => <Badge {...args} />;
const MixedTemplate = (args) => <Badge {...args} />;
const HighTemplate = (args) => <Badge {...args} />;

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

export const HighBadge = HighTemplate.bind({});
HighBadge.args = {
  type: "high",
  label: "High",
  backgroundColor: "#f2675a",
};
HighBadge.argTypes = {
  type: { control: "radio" },
};
