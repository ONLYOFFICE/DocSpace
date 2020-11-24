import React from "react";
import { storiesOf } from "@storybook/react";
import {
  withKnobs,
  text,
  select,
  number,
  color,
  boolean,
} from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import Section from "../../../.storybook/decorators/section";
import ContextMenuButton from ".";
import { Icons } from "../icons";

const iconNames = Object.keys(Icons);

function getData() {
  console.log("getData");
  return [
    { key: "key1", label: "label1", onClick: () => console.log("label1") },
    { key: "key2", label: "label2", onClick: () => console.log("label2") },
  ];
}

storiesOf("Components|ContextMenuButton", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => (
    <Section>
      <ContextMenuButton
        title={text("title", "Actions")}
        iconName={select("iconName", iconNames, "VerticalDotsIcon")}
        size={number("size", 16)}
        color={color("loaderColor", "#A3A9AE")}
        getData={getData}
        isDisabled={boolean("isDisabled", false)}
      />
    </Section>
  ));
