import React, { useState } from "react";
import FillingRoleSelector from ".";

export default {
  title: "Components/FillingRoleSelector",
  component: FillingRoleSelector,
  argTypes: {
    onAddUser: { action: "onAddUser" },
    onRemoveUser: { action: "onRemoveUser" },
  },
};

const mockRoles = [
  { id: 3, name: "Director", order: 3, color: "#BB85E7" },
  { id: 2, name: "Accountant", order: 2, color: "#70D3B0" },
  {
    id: 1,
    name: "Employee",
    order: 1,
    color: "#FBCC86",
    everyone: "@Everyone",
  },
];

const mockUsers = [
  {
    id: 1,
    displayName: "Makenna Lipshutz",
    role: "Accountant",
    avatar: "/images/user.avatar.example.react.svg",
    hasAvatar: true,
  },
  {
    id: 2,
    displayName: "Randy Korsgaard",
    role: "Director",
    hasAvatar: false,
  },
];

//TODO: Fix translations to correct ones when they appear on layouts
const textRoleEveryone =
  "The form is available for filling out by all participants of this room.";
const textTooltip =
  "Each form filled out by users from the first role will go in turn to the next users listed below.";

const Template = ({ onAddUser, ...args }) => {
  const onAddUserHandler = () => {
    onAddUser();
  };

  return (
    <FillingRoleSelector
      {...args}
      style={{ width: "480px", padding: "16px" }}
      onAddUser={onAddUserHandler}
    />
  );
};

export const Default = Template.bind({});

Default.args = {
  roles: mockRoles,
  descriptionEveryone: textRoleEveryone,
  descriptionTooltip: textTooltip,
};

const TemplateRolesFilledUsers = ({
  users,
  onAddUser,
  onRemoveUser,
  ...args
}) => {
  const [usersAssigned, setUsersAssigned] = useState(mockUsers);

  const onRemoveUserHandler = (id) => {
    const newUsersAssigned = usersAssigned.filter((item) => item.id !== id);
    setUsersAssigned(newUsersAssigned);
    onRemoveUser();
  };

  const onAddUserHandler = () => {
    onAddUser();
  };

  return (
    <FillingRoleSelector
      {...args}
      style={{ width: "480px", padding: "16px" }}
      users={usersAssigned}
      onRemoveUser={onRemoveUserHandler}
      onAddUser={onAddUserHandler}
    />
  );
};

export const rolesFilledUsers = TemplateRolesFilledUsers.bind({});

rolesFilledUsers.args = {
  roles: mockRoles,
  users: mockUsers,
  descriptionEveryone: textRoleEveryone,
  descriptionTooltip: textTooltip,
};
