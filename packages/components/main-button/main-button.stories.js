import React from "react";
import MainButton from ".";
import CatalogFolderReactSvgUrl from "../../../public/images/catalog.folder.react.svg?url";

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
      icon: CatalogFolderReactSvgUrl,
    },
    {
      label: "New spreadsheet",
      icon: CatalogFolderReactSvgUrl,
    },
    {
      label: "New presentation",
      icon: CatalogFolderReactSvgUrl,
    },
    {
      label: "Master form",
      icon: CatalogFolderReactSvgUrl,
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
      icon: CatalogFolderReactSvgUrl,
    },
    { separator: true },
    {
      label: "Upload",
      icon: CatalogFolderReactSvgUrl,
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
  iconName: CatalogFolderReactSvgUrl,
};
