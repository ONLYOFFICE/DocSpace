import React from "react";

import Tag from ".";

export default {
  title: "Components/Tag",
  component: Tag,
  parameters: {
    design: {
      type: "figma",
      url: "https://www.figma.com/file/ZiW5KSwb4t7Tj6Nz5TducC/UI-Kit-DocSpace-1.0.0?type=design&node-id=62-2597&mode=design&t=TBNCKMQKQMxr44IZ-0",
    },
  },
};

const Template = (args) => <Tag {...args} />;

export const Default = Template.bind({});

Default.args = {
  tag: "script",
  label: "Script",
  isNewTag: false,
  isDisabled: false,
  onDelete: (tag) => console.log(tag),
  onClick: (tag) => console.log(tag),
  advancedOptions: null,
  tagMaxWidth: "160px",
  id: "",
  className: "",
  style: { color: "red" },
};

export const WithDropDown = Template.bind({});

WithDropDown.args = {
  tag: "script",
  label: "Script",
  isNewTag: false,
  isDisabled: false,
  onDelete: (tag) => console.log(tag),
  onClick: (tag) => console.log(tag),
  advancedOptions: ["Option 1", "Option 2"],
};

export const NewTag = Template.bind({});

NewTag.args = {
  tag: "script",
  label: "Script",
  isNewTag: true,
  isDisabled: false,
  onDelete: (tag) => console.log(tag),
  onClick: (tag) => console.log(tag),
  advancedOptions: null,
};

export const DisabledTag = Template.bind({});

DisabledTag.args = {
  tag: "script",
  label: "No tag",
  isNewTag: false,
  isDisabled: true,
  onDelete: (tag) => console.log(tag),
  onClick: (tag) => console.log(tag),
  advancedOptions: null,
};
