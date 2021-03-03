import React from "react";

import ComboBox from "./";
import RadioButton from "../radio-button";
import DropDownItem from "../drop-down-item";
import NavLogoIcon from "../../../public/images/nav.logo.opened.react.svg";

export default {
  title: "Components/ComboBox",
  component: ComboBox,
  argTypes: {
    advancedOptions: {
      description: "If you need display options not basic options",
    },
    className: { description: "Accepts class" },
    displayType: { description: "Component Display Type" },
    dropDownMaxHeight: { description: "Height of Dropdown" },
    id: { description: "Accepts id" },
    isDisabled: { description: "Indicates that component is disabled" },
    noBorder: {
      description: "Indicates that component is displayed without borders",
    },
    onSelect: {
      description: "Will be triggered whenever an ComboBox is selected option",
      action: "onSelect",
    },
    options: { description: "Combo box options" },
    scaledOptions: {
      description:
        "Indicates that component`s options is scaled by ComboButton",
    },
    scaled: { description: "Indicates that component is scaled by parent" },
    selectedOption: { description: "Selected option" },
    size: { description: "Select component width, one of default" },
    style: { description: "Accepts css style" },
    toggleAction: {
      description:
        "The event will be raised when using `displayType: toggle` when clicking on a component",
    },
    showDisabledItems: {
      description: "Display disabled items or not when displayType !== toggle ",
    },
    children: { description: "Children element" },
    directionX: { description: "X direction selection" },
    directionY: { description: "Y direction selection" },
    opened: { description: "Tells when a component is open" },
    textOverflow: { description: "Accepts css text-overflow" },
    disableIconClick: { description: "Вisables clicking on the icon" },
  },
  parameters: {
    docs: {
      description: { component: "Custom combo box input" },
      source: {
        code: `
          ### Usage

import ComboBox from "@appserver/components/combobox";
import NavLogoIcon from "../../../../../public/images/nav.logo.react.svg";

const options = [
  {
    key: 1,
    icon: "static/images/catalog.employee.react.svg", // optional item
    label: "Option 1",
    disabled: false, // optional item
    onClick: clickFunction, // optional item
  },
];

#### Options have options:

- key - Item key, may be a string or a number
- label - Display text
- icon - Optional name of icon that will be displayed before label
- disabled - Make option disabled
- onClick - On click function

ComboBox perceives all property's for positioning from DropDown!

If you need to display a custom list of options, you must use advancedOptions property. Like this:

const advancedOptions = (
  <Meta>
    <DropDownItem>
      <RadioButton value="asc" name="first" label="A-Z" isChecked={true} />
    </DropDownItem>
    <DropDownItem>
      <RadioButton value="desc" name="first" label="Z-A" />
    </DropDownItem>
    <DropDownItem isSeparator />
    <DropDownItem>
      <RadioButton value="first" name="second" label="First name" />
    </DropDownItem>
    <DropDownItem>
      <RadioButton
        value="last"
        name="second"
        label="Last name"
        isChecked={true}
      />
    </DropDownItem>
  </Meta>
);

<ComboBox
  options={[]} // An empty array will enable advancedOptions
  advancedOptions={advancedOptions}
  onSelect={(option) => console.log("Selected option", option)}
  selectedOption={{
    key: 0,
    label: "Select",
  }}
  isDisabled={false}
  scaled={false}
  size="content"
  directionX="right"
>
  <NavLogoIcon size="medium" key="comboIcon" />
</ComboBox>

To use Combobox as a toggle button, you must declare it according to the parameters:

<ComboBox
  options={[]} // Required to display correctly
  selectedOption={{
    key: 0,
    label: "Selected option",
  }}
  scaled={false}
  size="content"
  displayType="toggle"
  toggleAction={alert("action")}
>
  <NavLogoIcon size="medium" key="comboIcon" />
</ComboBox>
          `,
      },
    },
  },
};

const comboOptions = [
  {
    key: 1,
    icon: "static/images/catalog.employee.react.svg",
    label: "Option 1",
  },
  {
    key: 2,
    icon: "CatalogGuestIcon",
    label: "Option 2",
  },
  {
    key: 3,
    disabled: true,
    label: "Option 3",
  },
  {
    key: 4,
    label: "Option 4",
  },
  {
    key: 5,
    icon: "static/images/copy.react.svg",
    label: "Option 5",
  },
  {
    key: 6,
    label: "Option 6",
  },
  {
    key: 7,
    label: "Option 7",
  },
];

let children = [];

const advancedOptions = (
  <>
    <DropDownItem key="1" noHover>
      <RadioButton value="asc" name="first" label="A-Z" isChecked={true} />
    </DropDownItem>
    <DropDownItem key="2" noHover>
      <RadioButton value="desc" name="first" label="Z-A" />
    </DropDownItem>
    <DropDownItem key="3" isSeparator />
    <DropDownItem key="4" noHover>
      <RadioButton value="first" name="second" label="First name" />
    </DropDownItem>
    <DropDownItem key="5" noHover>
      <RadioButton
        value="last"
        name="second"
        label="Last name"
        isChecked={true}
      />
    </DropDownItem>
  </>
);

const childrenItems = children.length > 0 ? children : null;

const Template = (args) => (
  <ComboBox
    {...args}
    options={[
      { key: 1, label: "Option 1" },
      { key: 2, label: "Option 2" },
    ]}
    selectedOption={{
      key: 0,
      label: "Select",
    }}
  />
);

const BaseOptionsTemplate = (args) => (
  <ComboBox
    {...args}
    options={comboOptions}
    onSelect={(option) => args.onSelect(option)}
    selectedOption={{
      key: 0,
      label: "Select",
      default: true,
    }}
  >
    {childrenItems}
  </ComboBox>
);

const AdvancedOptionsTemplate = (args) => (
  <ComboBox
    {...args}
    options={[]}
    advancedOptions={advancedOptions}
    onSelect={(option) => args.onSelect(option)}
    selectedOption={{
      key: 0,
      label: "Select",
      default: true,
    }}
  >
    <NavLogoIcon size="medium" key="comboIcon" />
  </ComboBox>
);

export const Default = Template.bind({});
export const BaseOptions = BaseOptionsTemplate.bind({});
BaseOptions.args = {
  scaledOptions: false,
  scaled: false,
  noBorder: false,
  isDisabled: false,
};
export const AdvancedOptions = AdvancedOptionsTemplate.bind({});
AdvancedOptions.args = {
  isDisabled: false,
  scaled: false,
  size: "content",
  directionX: "right",
};
