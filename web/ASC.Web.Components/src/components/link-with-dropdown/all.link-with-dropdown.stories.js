import React from "react";
import { storiesOf } from "@storybook/react";
import LinkWithDropdown from ".";

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

storiesOf("Components|LinkWithDropdown", module)
  .addParameters({ options: { showAddonPanel: false } })
  .add("all", () => (
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
  ));
