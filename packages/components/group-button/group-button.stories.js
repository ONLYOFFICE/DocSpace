import React from "react";
import GroupButton from "./";
import DropDownItem from "../drop-down-item";

export default {
  title: "Components/GroupButton",
  component: GroupButton,
  subcomponents: { DropDownItem },
  parameters: {
    docs: {
      description: {
        component: `Base Button is used for a group action on a page

It can be used as selector with checkbox for this following properties are combined: _isDropdown_, _isSeparato
_isSeparator_ will add vertical bar after button.

_isDropdown_ allows adding items to dropdown list in children.

For health of checkbox, button inherits part of properties of this component.       
`,
      },
    },
  },
};

const Template = (args) => {
  return (
    <div style={{ height: "200px", position: "relative" }}>
      <GroupButton {...args}>
        <DropDownItem label="Action 1" />
        <DropDownItem label="Action 2" />
        <DropDownItem label="Action 3" />
      </GroupButton>
    </div>
  );
};

export const Default = Template.bind({});
Default.args = {
  label: "Base group button",
  disabled: false,
  isDropdown: true,
  opened: true,
};

const AllTemplate = (args) => {
  const rowStyle = { marginTop: 8 };
  const headerStyle = { marginLeft: 16 };
  return (
    <div
      style={{
        padding: "8px 0 0 40px",
        display: "grid",
        gridTemplateColumns: "1fr 1fr 1fr 1fr",
        height: "200px",
        position: "relative",
      }}
    >
      <div style={rowStyle}>
        <div style={headerStyle}>Active</div>
        <div>
          <GroupButton />
        </div>
        <div>
          <GroupButton isDropdown>
            <DropDownItem label="Action 1" />
          </GroupButton>
        </div>
      </div>
      <div style={rowStyle}>
        <div style={headerStyle}>Hover</div>
        <div>
          <GroupButton hovered />
        </div>
        <div>
          <GroupButton isDropdown hovered>
            <DropDownItem label="Action 2" />
          </GroupButton>
        </div>
      </div>
      <div style={rowStyle}>
        <div style={headerStyle}>Click*(Press)</div>
        <div>
          <GroupButton activated />
        </div>
        <div>
          <GroupButton isDropdown activated>
            <DropDownItem label="Action 3" />
          </GroupButton>
        </div>
      </div>
      <div style={rowStyle}>
        <div style={headerStyle}>Disable</div>
        <div>
          <GroupButton disabled />
        </div>
        <div>
          <GroupButton isDropdown disabled>
            <DropDownItem label="Action 4" />
          </GroupButton>
        </div>
      </div>
    </div>
  );
};

export const All = AllTemplate.bind({});
