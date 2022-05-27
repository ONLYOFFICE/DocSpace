import React from "react";
import MainButton from ".";

export default {
  title: "Components/MainButton",
  component: MainButton,
  parameters: { docs: { description: { component: "Components/MainButton" } } },
  clickAction: { action: "clickAction" },
  clickActionSecondary: { action: "clickActionSecondary" },
  clickItem: { action: "clickItem", table: { disable: true } },
};

const Template = ({
  clickAction,
  clickActionSecondary,
  clickItem,
  ...args
}) => {
  const clickMainButtonHandler = (e, credentials) => {
    clickAction(e, credentials);
  };

  const clickSecondaryButtonHandler = (e, credentials) => {
    clickActionSecondary(e, credentials);
  };

  const itemsModel = [
    {
      label: "New document",
      icon: "/static/images/catalog.folder.react.svg",
    },
    {
      label: "New spreadsheet",
      icon: "/static/images/catalog.folder.react.svg",
    },
    {
      label: "New presentation",
      icon: "/static/images/catalog.folder.react.svg",
    },
    {
      label: "Master form",
      icon: "/static/images/catalog.folder.react.svg",
      items: [
        {
          label: "From blank",
        },
        {
          label: "From an existing text file",
        },
      ],
    },
    {
      label: "New folder",
      icon: "/static/images/catalog.folder.react.svg",
    },
    { separator: true },
    {
      label: "Upload",
      icon: "/static/images/catalog.folder.react.svg",
    },
  ];

  return (
    <div style={{ width: "280px" }}>
      <MainButton
        {...args}
        clickAction={clickMainButtonHandler}
        clickActionSecondary={clickSecondaryButtonHandler}
        model={itemsModel}
        iconName
      ></MainButton>
    </div>
  );
};

export const Default = Template.bind({});
Default.args = {
  isDisabled: false,
  isDropdown: true,
  text: "Actions",
  iconName: "static/images/people.react.svg",
};
