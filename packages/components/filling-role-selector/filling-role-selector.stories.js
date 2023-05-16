import React from "react";
import FillingRoleSelector from ".";

export default {
  title: "Components/FillingRoleSelector",
  component: FillingRoleSelector,
  argTypes: {
    onClick: { action: "onClick" },
  },
};

const mockRoles = [
  { id: 3, role: "Director", order: 3, color: "#BB85E7" },
  { id: 2, role: "Accountant", order: 2, color: "#70D3B0" },
  { id: 1, role: "Employee", order: 1, color: "#FBCC86", everyone: true },
];

const mockUsers = [
  {
    id: 1,
    firstName: "Makenna",
    lastName: "Lipshutz",
    role: "Accountant",
    avatar: "",
  },
  {
    id: 2,
    firstName: "Randy",
    lastName: "Korsgaard",
    role: "Director",
    avatar: "",
  },
];

const Template = ({ onClick, ...args }) => {
  return (
    <FillingRoleSelector
      {...args}
      style={{ width: "480px", padding: "16px" }}
      onClick={(e) => onClick(e)}
    />
  );
};

export const Default = Template.bind({});
Default.args = {
  roles: mockRoles,
};

const TemplateRolesFilledUsers = ({ onClick, ...args }) => {
  return (
    <FillingRoleSelector
      {...args}
      style={{ width: "480px", padding: "16px" }}
      onClick={(e) => onClick(e)}
    />
  );
};

export const rolesFilledUsers = TemplateRolesFilledUsers.bind({});

rolesFilledUsers.args = {
  users: mockUsers,
  roles: mockRoles,
};
