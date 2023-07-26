import React, { useState } from "react";

import DropDown from "./";
import DropDownItem from "../drop-down-item";

export default {
  title: "Components/DropDown",
  component: DropDown,
  subcomponents: { DropDownItem },
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

  return (
    <div style={{ height: "200px", position: "relative", padding: "20px" }}>
      <DropDown
        {...args}
        open={isOpen}
        isDefaultMode={false}
        clickOutsideAction={() => {}}
        style={{ top: "20px", left: "20px" }}
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

export const Default = Template.bind({});

Default.args = { open: true };
