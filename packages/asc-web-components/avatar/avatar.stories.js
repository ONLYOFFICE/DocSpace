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
Default.args = {
  size: "max",
  role: "admin",
  source: "",
  userName: "",
  editing: false,
};
