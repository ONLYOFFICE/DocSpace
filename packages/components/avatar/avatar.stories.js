import React from "react";
import Avatar from "./";
import AtReactSvgUrl from "PUBLIC_DIR/images/@.react.svg?url";

const editAction = () => console.log("Edit action");

export default {
  title: "Components/Avatar",
  component: Avatar,
  argTypes: {
    editAction: { action: "editAction" },
  },
  parameters: {
    docs: {
      description: {
        component: "Used to display an avatar or brand.",
      },
    },
    design: {
      type: "figma",
      url: "https://www.figma.com/file/ZiW5KSwb4t7Tj6Nz5TducC/UI-Kit-DocSpace-1.0.0?type=design&node-id=878-37278&mode=design&t=TBNCKMQKQMxr44IZ-0",
    },
  },
};

const Template = (args) => <Avatar {...args} />;
export const Default = Template.bind({});

const PictureTemplate = (args) => <Avatar {...args} />;
export const Picture = PictureTemplate.bind({});

const InitialsTemplate = (args) => <Avatar {...args} />;
export const Initials = InitialsTemplate.bind({});

const IconTemplate = (args) => <Avatar {...args} />;
export const Icon = IconTemplate.bind({});

Default.args = {
  size: "max",
  role: "owner",
  source: "",
  userName: "",
  editing: false,
  hideRoleIcon: false,
  tooltipContent: "",
  withTooltip: false,
};

Picture.args = {
  size: "max",
  role: "admin",
  source:
    "https://images.unsplash.com/photo-1623949444573-4811dfc64771?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=880&q=80",
  userName: "",
  editing: false,
  hideRoleIcon: false,
  tooltipContent: "",
  withTooltip: false,
};

Initials.args = {
  size: "max",
  role: "guest",
  source: "",
  userName: "John Doe",
  editing: false,
  hideRoleIcon: false,
  tooltipContent: "",
  withTooltip: false,
};

Icon.args = {
  size: "max",
  role: "user",
  source: AtReactSvgUrl,
  userName: "",
  editing: false,
  hideRoleIcon: false,
  tooltipContent: "",
  withTooltip: false,
};
