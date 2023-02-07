import React, { useState } from "react";
import styled from "@emotion/styled";
import GroupButtonsMenu from "../group-buttons-menu";
import DropDownItem from "../drop-down-item";
import Button from "../button";

export default {
  title: "Examples/GroupButtonsMenu",
  component: GroupButtonsMenu,
  subcomponents: { DropDownItem, Button },
  parameters: { docs: { description: { component: "Example" } } },
  argTypes: {
    closeAction: { action: "close action", table: { disable: true } },
  },
};

const GroupButtonsMenuContainer = styled.div`
  height: 2000px;
`;

const peopleItems = [
  {
    label: "Select",
    isDropdown: true,
    isSeparator: true,
    isSelect: true,
    fontWeight: "bold",
    children: [
      <DropDownItem key="active" label="Active" />,
      <DropDownItem key="disabled" label="Disabled" />,
      <DropDownItem key="invited" label="Invited" />,
    ],
    onSelect: (a) => console.log(a),
  },
  {
    label: "Make employee",
    onClick: () => console.log("Make employee action"),
  },
  {
    label: "Make guest",
    onClick: () => console.log("Make guest action"),
  },
  {
    label: "Set active",
    onClick: () => console.log("Set active action"),
  },
  {
    label: "Set disabled",
    onClick: () => console.log("Set disabled action"),
  },
  {
    label: "Invite again",
    onClick: () => console.log("Invite again action"),
  },
  {
    label: "Send e-mail",
    onClick: () => console.log("Send e-mail action"),
  },
  {
    label: "Delete",
    onClick: () => console.log("Delete action"),
  },
];

const Template = ({ closeAction, closeTitle, moreLabel, ...args }) => {
  const [visible, setVisible] = useState(false);
  const [checked, setChecked] = useState(false);
  return (
    <>
      <Button
        label="Show menu"
        onClick={() => {
          setVisible(!visible);
        }}
      />
      <GroupButtonsMenuContainer>
        <GroupButtonsMenu
          checked={checked}
          menuItems={peopleItems}
          visible={visible}
          moreLabel={moreLabel}
          closeTitle={closeTitle}
          onClose={closeAction}
          onChange={() => setChecked(!checked)}
          selected={peopleItems[0].label}
        />
      </GroupButtonsMenuContainer>
    </>
  );
};

export const people = Template.bind({});
people.args = {
  closeTitle: "Close",
  moreLabel: "More",
};
