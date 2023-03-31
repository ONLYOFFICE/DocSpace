import React from "react";
import LinkWithDropdown from ".";
import SettingsReactSvg from "PUBLIC_DIR/images/settings.react.svg?url";

export default {
  title: "Components/LinkWithDropdown",
  component: LinkWithDropdown,
  parameters: { docs: { description: { component: "Link with dropdown" } } },
  argTypes: {
    color: { control: "color" },
    dropdownType: { required: false },
    linkLabel: { control: "text", description: "Link text" },
    onItemClick: { action: "Button action", table: { disable: true } },
  },
};

const Template = ({ linkLabel, onItemClick, ...args }) => {
  const dropdownItems = [
    {
      key: "key1",
      label: "Button 1",
      onClick: () => onItemClick("Button1 action"),
    },
    {
      key: "key2",
      label: "Button 2",
      onClick: () => onItemClick("Button2 action"),
    },
    {
      key: "key3",
      isSeparator: true,
    },
    {
      key: "key4",
      label: "Button 3",
      onClick: () => onItemClick("Button3 action"),
    },
  ];
  return (
    <LinkWithDropdown {...args} data={dropdownItems}>
      {linkLabel}
    </LinkWithDropdown>
  );
};

export const Default = Template.bind({});
Default.args = {
  // dropdownType: "alwaysDashed",
  fontSize: "13px",
  fontWeight: "400",
  isBold: false,
  isTextOverflow: false,
  isSemitransparent: false,
  linkLabel: "Some text",
};
