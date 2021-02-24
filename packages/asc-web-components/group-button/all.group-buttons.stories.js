import React from "react";
import { storiesOf } from "@storybook/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import GroupButton from ".";
import DropDownItem from "../drop-down-item";

const rowStyle = { marginTop: 8 };
const headerStyle = { marginLeft: 16 };

storiesOf("Components|GroupButton", module)
  .addDecorator(withReadme(Readme))
  .addParameters({ options: { showAddonPanel: false } })
  .add("all", () => (
    <>
      <div
        style={{
          padding: "8px 0 0 40px",
          display: "grid",
          gridTemplateColumns: "1fr 1fr 1fr 1fr",
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
    </>
  ));
