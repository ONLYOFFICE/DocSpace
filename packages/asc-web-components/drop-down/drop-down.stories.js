import React, { useState } from "react";

import DropDown from "./";
import DropDownItem from "../drop-down-item";
import GroupButton from "../group-button";

export default {
  title: "Components/DropDown",
  component: DropDown,
  subcomponents: { DropDownItem, GroupButton },
  decorators: [
    (Story) => (
      <div style={{ height: "200px", position: "relative" }}>
        <Story />
      </div>
    ),
  ],
  argTypes: {
    open: { description: "Tells when the dropdown should be opened" },
    className: { description: " Accepts class" },
    clickOutsideAction: {
      description:
        "Required for determining a click outside DropDown with the withBackdrop parameter",
    },
    directionX: {
      description: "Sets the opening direction relative to the parent",
    },
    directionY: {
      description: "Sets the opening direction relative to the parent",
    },
    id: { description: "Accepts id " },
    manualWidth: {
      description:
        "Required if you need to specify the exact width of the component, for example 100%",
    },
    manualX: {
      description:
        "Required if you need to specify the exact distance from the parent component",
    },
    manualY: {
      description:
        "Required if you need to specify the exact distance from the parent component",
    },
    maxHeight: { description: "Required if the scrollbar is displayed" },
    style: { description: "Accepts css style" },
    withBackdrop: { description: "Used to display backdrop" },
    showDisabledItems: { description: "Display disabled items or not" },
    children: { table: { disable: true } },
    columnCount: { table: { disable: true } },
    disableOnClickOutside: { table: { disable: true } },
    enableOnClickOutside: { table: { disable: true } },
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
  );
};

const WithButtonTemplate = (args) => {
  return (
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
  );
};

export const Default = Template.bind({});

export const WithButton = WithButtonTemplate.bind({});

Default.args = { open: true };
Default.parameters = {
  docs: {
    source: {
      code: `import DropDown from "@appserver/components/drop-down";
import DropDownItem from "@appserver/components/drop-down-item";

<DropDown {...props}>
  <DropDownItem isHeader label="Category 1" />
  <DropDownItem
    label="Button 1"
    onClick={() => action("Button 1 clicked")}
  />
  <DropDownItem
    label="Button 2"
    onClick={() => action("Button 2 clicked")}
  />
  <DropDownItem
    label="Button 3"
    onClick={() => action("Button 3 clicked")}
  />
  <DropDownItem
    label="Button 4"
    onClick={() => action("Button 4 clicked")}
    disabled={true}
  />
  <DropDownItem isSeparator />
  <DropDownItem
    label="Button 5"
    onClick={() => action("Button 5 clicked")}
  />
  <DropDownItem
    label="Button 6"
    onClick={() => action("Button 6 clicked")}
  />
</DropDown>`,
    },
  },
};

WithButton.args = {
  open: true,
};
WithButton.parameters = {
  docs: {
    source: {
      code: `import GroupButton from "@appserver/components/group-button";
import DropDownItem from "@appserver/components/drop-down-item";

<GroupButton label="Dropdown demo" isDropdown={true}>
  <DropDownItem
    label="Button 1"
    onClick={() => console.log("Button 1 clicked")}
  />
  <DropDownItem
    label="Button 2"
    onClick={() => console.log("Button 2 clicked")}
  />
  <DropDownItem
    label="Button 3"
    onClick={() => console.log("Button 3 clicked")}
  />
</GroupButton>`,
    },
  },
};
