import React from "react";
import { storiesOf } from "@storybook/react";
import { withKnobs, text } from "@storybook/addon-knobs/react";
import { BooleanValue } from "react-values";
import styled from "@emotion/styled";
import GroupButtonsMenu from "../group-buttons-menu";
import DropDownItem from "../drop-down-item";
import Button from "../button";

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

storiesOf("EXAMPLES|GroupButtonsMenu", module)
  .addDecorator(withKnobs)
  .add("people", () => (
    <BooleanValue>
      {({ value: visible, toggle }) => (
        <>
          <Button
            label="Show menu"
            onClick={() => {
              toggle(visible);
            }}
          />
          <GroupButtonsMenuContainer>
            <BooleanValue>
              {({ value: checked, toggle }) => (
                <GroupButtonsMenu
                  checked={checked}
                  menuItems={peopleItems}
                  visible={visible}
                  moreLabel={text("moreLabel", "More")}
                  closeTitle={text("closeTitle", "Close")}
                  onClose={() => console.log("Close action")}
                  onChange={() => toggle(checked)}
                  selected={peopleItems[0].label}
                />
              )}
            </BooleanValue>
          </GroupButtonsMenuContainer>
        </>
      )}
    </BooleanValue>
  ));
