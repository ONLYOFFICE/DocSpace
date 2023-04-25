import React from "react";
import MainButton from ".";
import CatalogFolderReactSvgUrl from "PUBLIC_DIR/images/catalog.folder.react.svg?url";

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
      key: 0,
      label: "New document",
      icon: CatalogFolderReactSvgUrl,
    },
    {
      key: 1,
      label: "New spreadsheet",
      icon: CatalogFolderReactSvgUrl,
    },
    {
      key: 2,
      label: "New presentation",
      icon: CatalogFolderReactSvgUrl,
    },
    {
      key: 3,
      label: "Master form",
      icon: CatalogFolderReactSvgUrl,
      items: [
        {
          key: 4,
          label: "From blank",
        },
        {
          key: 5,
          label: "From an existing text file",
        },
      ],
    },
    {
      key: 6,
      label: "New folder",
      icon: CatalogFolderReactSvgUrl,
    },
    {
      key: 7,
      separator: true,
    },
    {
      key: 8,
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
      ></MainButton>
    </div>
  );
};

export const Default = Template.bind({});
Default.args = {
  isDisabled: false,
  isDropdown: true,
  text: "Actions",
};
