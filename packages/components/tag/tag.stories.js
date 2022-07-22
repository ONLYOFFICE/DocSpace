import React from "react";

import Tag from ".";

export default {
  title: "Components/Tag",
  component: Tag,
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
  advancedOptions: [
    {
      key: "0",
      value: "0",
      label: "Option 1",
      onClick: (e) => {
        console.log(e.target);
      },
    },
    {
      key: "1",
      value: "1",
      label: "Option 2",
      onClick: (e) => {
        console.log(e.target);
        ``;
      },
    },
  ],
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
