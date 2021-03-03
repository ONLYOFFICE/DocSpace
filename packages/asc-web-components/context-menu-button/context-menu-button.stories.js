import React from "react";

import ContextMenuButton from "./";
import icon from "../../../public/images/vertical-dots.react.svg";

export default {
  title: "Components/ContextMenuButton",
  component: ContextMenuButton,
  argTypes: {},
  parameters: {
    docs: {
      description: {
        component: `ContextMenuButton is used for displaying context menu actions on a list's item`,
      },
      source: {
        code: `
        import ContextMenuButton from "@appserver/components/context-menu-button";

<ContextMenuButton
  iconName="static/images/vertical-dots.react.svg"
  size={16}
  color="#A3A9AE"
  isDisabled={false}
  title="Actions"
  getData={() => [
    {
      key: "key",
      label: "label",
      onClick: () => alert("label"),
    },
  ]}
/>
        `,
      },
    },
  },
};

function getData() {
  console.log("getData");
  return [
    { key: "key1", label: "label1", onClick: () => console.log("label1") },
    { key: "key2", label: "label2", onClick: () => console.log("label2") },
  ];
}

const Template = (args) => (
  <ContextMenuButton
    {...args}
    title={"Actions"}
    iconName={icon}
    size={16}
    color={"#A3A9AE"}
    getData={getData}
    isDisabled={false}
  />
);

export const Default = Template.bind({});
/*import { storiesOf } from "@storybook/react";
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
  ));*/
