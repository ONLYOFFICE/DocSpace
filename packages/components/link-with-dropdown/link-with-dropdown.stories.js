import React from "react";
import LinkWithDropdown from ".";

export default {
  title: "Components/LinkWithDropdown",
  component: LinkWithDropdown,
  parameters: { docs: { description: { component: "Link with dropdown" } } },
  argTypes: {
    color: { control: "color" },
    dropdownType: { required: true },
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
  dropdownType: "alwaysDashed",
  fontSize: "13px",
  fontWeight: "400",
  isBold: false,
  isTextOverflow: false,
  isSemitransparent: false,
  linkLabel: "Some text",
};

const AllTemplate = (args) => {
  const headerStyle = {
    paddingLeft: 20,
    fontSize: 16,
  };

  const rowStyle = {
    marginTop: 8,
    paddingLeft: 28,
    fontSize: 12,
  };

  const data = [
    {
      key: "key1",
      label: "Base button1",
      onClick: () => console.log("Base button1 clicked"),
    },
    {
      key: "key2",
      label: "Base button2",
      onClick: () => console.log("Base button2 clicked"),
    },
    { key: "key3", isSeparator: true },
    {
      key: "key4",
      label: "Base button3",
      onClick: () => console.log("Base button3 clicked"),
    },
  ];

  return (
    <>
      <div
        style={{
          padding: "8px 0 0 20px",
          display: "grid",
          gridTemplateColumns: "1fr 1fr",
        }}
      >
        <div>
          <label style={headerStyle}>Type - alwaysDashed:</label>
          <div style={rowStyle}>
            <LinkWithDropdown
              isBold={true}
              dropdownType="alwaysDashed"
              data={data}
            >
              Bold alwaysDashed
            </LinkWithDropdown>
          </div>
          <div style={rowStyle}>
            <LinkWithDropdown dropdownType={"alwaysDashed"} data={data}>
              {"alwaysDashed"}
            </LinkWithDropdown>
          </div>
          <div style={rowStyle}>
            <LinkWithDropdown
              isSemitransparent={true}
              dropdownType="alwaysDashed"
              data={data}
            >
              Semitransparent alwaysDashed
            </LinkWithDropdown>
          </div>
        </div>
        <div>
          <label style={headerStyle}>Type - appearDashedAfterHover:</label>
          <div style={rowStyle}>
            <LinkWithDropdown
              isBold={true}
              dropdownType="appearDashedAfterHover"
              data={data}
            >
              Bold appearDashedAfterHover
            </LinkWithDropdown>
          </div>
          <div style={rowStyle}>
            <LinkWithDropdown
              dropdownType={"appearDashedAfterHover"}
              data={data}
            >
              appearDashedAfterHover
            </LinkWithDropdown>
          </div>
          <div style={rowStyle}>
            <LinkWithDropdown
              isSemitransparent={true}
              dropdownType="appearDashedAfterHover"
              data={data}
            >
              Semitransparent appearDashedAfterHover
            </LinkWithDropdown>
          </div>
        </div>
      </div>
    </>
  );
};

export const AllLinks = AllTemplate.bind({});
