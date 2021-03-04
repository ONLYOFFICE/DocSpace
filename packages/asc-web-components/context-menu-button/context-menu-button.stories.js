import React from "react";

import ContextMenuButton from "./";

export default {
  title: "Components/ContextMenuButton",
  component: ContextMenuButton,
  argTypes: {
    className: { description: "Accepts class" },
    clickColor: {
      description: "Specifies the icon click color",
      control: "color",
    },
    color: { description: "Specifies the icon color", control: "color" },
    data: { description: "Array of options for display " },
    directionX: {
      description: "What the button will trigger when mouse out of button",
    },
    getData: { description: "Function for converting to inner data " },
    hoverColor: {
      description: "Specifies the icon hover color",
      control: "color",
    },
    iconClickName: { description: "Specifies the icon click name" },
    iconHoverName: { description: "Specifies the icon hover name" },
    iconName: { description: "Specifies the icon name" },
    id: { description: "Accepts id" },
    isDisabled: {
      description: "Tells when the button should present a disabled state",
    },
    onMouseEnter: {
      description: "What the button will trigger when mouse hovered",
      action: "onMouseEnter",
    },
    onMouseLeave: {
      description: "What the button will trigger when mouse leave",
      action: "onMouseLeave",
    },
    onMouseOut: {
      description: "What the button will trigger when mouse out of button",
      action: "onMouseOut",
    },
    onMouseOver: {
      description: "What the button will trigger when mouse over button",
      action: "onMouseOver",
    },
    opened: {
      description: "Tells when the button should present a opened state",
    },
    size: { description: "Specifies the icon size" },
    style: { description: "Accepts css style" },
    title: { description: "Specifies the icon title" },
    iconOpenName: { description: "Specifies the icon open name" },
    directionY: { description: "Direction Y" },
    columnCount: { description: "Set the number of columns" },
    displayType: { description: "Set the display type" },
    onClickLabel: { action: "onClickLabel", table: { disable: true } },
  },
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

const Template = (args) => {
  function getData() {
    console.log("getData");
    return [
      {
        key: "key1",
        label: "label1",
        onClick: () => args.onClickLabel("label1"),
      },
      {
        key: "key2",
        label: "label2",
        onClick: () => args.onClickLabel("label2"),
      },
    ];
  }
  return (
    <ContextMenuButton
      {...args}
      title={"Actions"}
      iconName={"/static/images/vertical-dots.react.svg"}
      size={16}
      color={"#A3A9AE"}
      getData={getData}
      isDisabled={false}
    />
  );
};

export const Default = Template.bind({});