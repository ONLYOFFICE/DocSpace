import React, { useState } from "react";

import DropDown from "./";
import DropDownItem from "../drop-down-item";
import GroupButton from "../group-button";

export default {
  title: "Components/DropDown",
  component: DropDown,
  subcomponents: { DropDownItem, GroupButton },
  argTypes: {
    onClick: { action: "onClickItem", table: { disable: true } },
  },
  parameters: {
    docs: {
      description: {
        component: `Is a dropdown with any number of action
        By default, it is used with DropDownItem elements in role of children.

If you want to display something custom, you can put it in children, but take into account that all stylization is assigned to the implemented component.

When using component, it should be noted that parent must have CSS property _position: relative_. Otherwise, DropDown will appear outside parent's border.
`,
      },
    },
  },
};

const Template = (args) => {
  const [isOpen, setIsOpen] = useState(args.open);

  const clickOutsideAction = (e) => {
    setIsOpen(!isOpen);
  };

  return (
    <div style={{ height: "200px", position: "relative" }}>
      <DropDown
        {...args}
        open={isOpen}
        clickOutsideAction={clickOutsideAction}
        style={{ top: 0, left: 0 }}
        onClick={() => {}}
      >
        <DropDownItem isHeader label="Category 1" />

        <DropDownItem
          label="Button 1"
          onClick={() => args.onClick("Button 1 clicked")}
        />
        <DropDownItem
          label="Button 2"
          onClick={() => args.onClick("Button 2 clicked")}
        />
        <DropDownItem
          label="Button 3"
          onClick={() => args.onClick("Button 3 clicked")}
        />
        <DropDownItem
          label="Button 4"
          onClick={() => args.onClick("Button 4 clicked")}
          disabled={true}
        />
        <DropDownItem isSeparator />
        <DropDownItem
          label="Button 5"
          onClick={() => args.onClick("Button 5 clicked")}
        />
        <DropDownItem
          label="Button 6"
          onClick={() => args.onClick("Button 6 clicked")}
        />
      </DropDown>
    </div>
  );
};

const WithButtonTemplate = (args) => {
  return (
    <div style={{ height: "200px", position: "relative" }}>
      <GroupButton
        label="Dropdown demo"
        style={{ top: 0, left: 0 }}
        isDropdown={true}
        opened={args.open}
      >
        <DropDownItem
          label="Button 1"
          onClick={() => args.onClick("Button 2 clicked")}
        />
        <DropDownItem
          label="Button 2"
          onClick={() => args.onClick("Button 2 clicked")}
        />
        <DropDownItem
          label="Button 3"
          onClick={() => args.onClick("Button 3 clicked")}
        />
      </GroupButton>
    </div>
  );
};

export const Default = Template.bind({});

export const WithButton = WithButtonTemplate.bind({});

Default.args = { open: true };

WithButton.args = {
  open: true,
};
