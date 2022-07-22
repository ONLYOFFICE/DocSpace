import React from "react";
import Avatar from "./";

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
};

Picture.args = {
  size: "max",
  role: "admin",
  source:
    "https://images.unsplash.com/photo-1623949444573-4811dfc64771?ixlib=rb-1.2.1&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=880&q=80",
  userName: "",
  editing: false,
};

Initials.args = {
  size: "max",
  role: "guest",
  source: "",
  userName: "John Doe",
  editing: false,
};

Icon.args = {
  size: "max",
  role: "user",
  source: "/static/images/@.react.svg",
  userName: "",
  editing: false,
};
