import React, { useState } from "react";
import styled from "styled-components";

import GroupButtonsMenu from ".";
import Button from "../button";
import DropDownItem from "../drop-down-item";

export default {
  title: "Components/GroupButtonsMenu",
  component: GroupButtonsMenu,
  argTypes: {
    disableOdd: { description: "Disable odd", control: "boolean" },
    disableEven: { description: "Disable even", control: "boolean" },
    onClose: { action: "onClose" },
    onChange: { action: "onChange" },
    onClick: { action: "onClick" },
    onSelectClick: { action: "onSelect", table: { disable: true } },
    onItemClick: { action: "onItemClick", table: { disable: true } },
  },
  parameters: {
    docs: {
      description: {
        component: "Menu for group actions on a page",
      },
    },
  },
};

const GroupButtonsMenuContainer = styled.div`
  height: 2000px;
`;

const Template = ({
  disableEven,
  disableOdd,
  onClose,
  onChange,
  onSelectClick,
  onItemClick,
  ...rest
}) => {
  const [isVisible, setIsVisible] = useState(rest.visible);
  const [checked, setChecked] = useState(rest.checked);
  const [groupItems, setGroupItem] = useState([
    {
      label: "Select",
      isDropdown: true,
      isSeparator: true,
      isSelect: true,
      fontWeight: "bold",
      children: [
        <DropDownItem key="aaa" label="aaa" />,
        <DropDownItem key="bbb" label="bbb" />,
        <DropDownItem key="ccc" label="ccc" />,
      ],
      onSelect: (a) => onSelectClick(a),
    },
    {
      label: "Menu item",
      disabled: disableEven,
      onClick: () => onItemClick("Menu item action"),
    },
    {
      label: "Menu item",
      disabled: disableOdd,
      onClick: () => onItemClick("Menu item action"),
    },
    {
      label: "Menu item",
      disabled: disableEven,
      onClick: () => onItemClick("Menu item action"),
    },
    {
      label: "Menu item",
      disabled: disableOdd,
      onClick: () => onItemClick("Menu item action"),
    },
    {
      label: "Menu item",
      disabled: disableEven,
      onClick: () => onItemClick("Menu item action"),
    },
    {
      label: "Menu item",
      disabled: disableOdd,
      onClick: () => onItemClick("Menu item action"),
    },
    {
      label: "Menu item",
      disabled: disableEven,
      onClick: () => onItemClick("Menu item action"),
    },
    {
      label: "Menu item",
      disabled: disableOdd,
      onClick: () => onItemClick("Menu item action"),
    },
    {
      label: "Menu item",
      disabled: disableEven,
      onClick: () => onItemClick("Menu item action"),
    },
    {
      label: "Menu item",
      disabled: disableOdd,
      onClick: () => onItemClick("Menu item action"),
    },
    {
      label: "Menu item",
      disabled: disableEven,
      onClick: () => onItemClick("Menu item action"),
    },
    {
      label: "Menu item",
      disabled: disableOdd,
      onClick: () => onItemClick("Menu item action"),
    },
    {
      label: "Menu item",
      disabled: disableEven,
      onClick: () => onItemClick("Menu item action"),
    },
    {
      label: "Menu item",
      disabled: disableOdd,
      onClick: () => onItemClick("Menu item action"),
    },
    {
      label: "Menu item",
      disabled: disableEven,
      onClick: () => onItemClick("Menu item action"),
    },
    {
      label: "Menu item",
      disabled: disableOdd,
      onClick: () => onItemClick("Menu item action"),
    },
    {
      label: "Menu item",
      disabled: disableEven,
      onClick: () => onItemClick("Menu item action"),
    },
    {
      label: "Menu item",
      disabled: disableOdd,
      onClick: () => onItemClick("Menu item action"),
    },
  ]);

  const toggleVisible = () => setIsVisible(!isVisible);
  const toggleChecked = () => {
    onChange(!checked);
    setChecked(!checked);
  };

  return (
    <>
      <Button
        label={`${isVisible ? "Hide" : "Show"} menu`}
        primary
        size="small"
        onClick={toggleVisible}
      />

      <GroupButtonsMenuContainer>
        <GroupButtonsMenu
          {...rest}
          checked={checked}
          menuItems={groupItems}
          visible={isVisible}
          onClose={() => {
            toggleVisible();
            onClose("Close action");
          }}
          onChange={toggleChecked}
          selected={groupItems[0].label}
        />
      </GroupButtonsMenuContainer>
    </>
  );
};

export const Default = Template.bind({});
Default.args = {
  disableEven: false,
  disableOdd: false,
  moreLabel: "More",
  closeTitle: "Close",
};
