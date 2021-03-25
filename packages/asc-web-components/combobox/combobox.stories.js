import React from "react";

import ComboBox from "./";
import RadioButton from "../radio-button";
import DropDownItem from "../drop-down-item";
import NavLogoIcon from "../../../public/images/nav.logo.opened.react.svg";

const comboOptions = [
  {
    key: 1,
    icon: "static/images/catalog.employee.react.svg",
    label: "Option 1",
  },
  {
    key: 2,
    icon: "static/images/catalog.guest.react.svg",
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

const Wrapper = (props) => (
  <div style={{ height: "220px" }}>{props.children}</div>
);

const childrenItems = children.length > 0 ? children : null;

const Template = (args) => (
  <Wrapper>
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
  </Wrapper>
);

const BaseOptionsTemplate = (args) => (
  <Wrapper>
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
  </Wrapper>
);

const AdvancedOptionsTemplate = (args) => (
  <Wrapper>
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
  </Wrapper>
);

export const basic = Template.bind({});
basic.args = {
  opened: true,
  scaled: false,
};
export const baseOption = BaseOptionsTemplate.bind({});
baseOption.args = {
  scaledOptions: false,
  scaled: false,
  noBorder: false,
  isDisabled: false,
  opened: true,
};
export const advancedOption = AdvancedOptionsTemplate.bind({});
advancedOption.args = {
  opened: true,
  isDisabled: false,
  scaled: false,
  size: "content",
  directionX: "right",
  directionY: "bottom",
};
