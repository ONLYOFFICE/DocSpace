import React from "react";
import { storiesOf } from "@storybook/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import DropDown from ".";
import DropDownItem from "../drop-down-item";
import GroupButton from "../group-button";

storiesOf("Components| DropDown", module)
  .addDecorator(withReadme(Readme))
  .add("base", () => (
    <div
      style={{
        padding: "8px 0 0 60px",
        display: "grid",
        gridTemplateColumns: "1fr 1fr"
      }}
    >
      <div style={{ position: "relative" }}>
        <div>Only dropdown</div>
        <div style={{ marginTop: 8 }}>Without active button</div>
        <DropDown open={true}>
          <DropDownItem isHeader label="Category 1" />
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
          <DropDownItem
            label="Button 4"
            onClick={() => console.log("Button 4 clicked")}
            disabled={true}
          />
          <DropDownItem isSeparator />
          <DropDownItem
            label="Button 5"
            onClick={() => console.log("Button 5 clicked")}
          />
          <DropDownItem
            label="Button 6"
            onClick={() => console.log("Button 6 clicked")}
          />
        </DropDown>
      </div>
      <div style={{ position: "relative" }}>
        <div style={{ marginLeft: 16 }}>With Button</div>
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
        </GroupButton>
      </div>
    </div>
  ));
