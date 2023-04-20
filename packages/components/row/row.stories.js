import React from "react";
import Row from ".";
import Text from "../text";
import Avatar from "../avatar";
import ComboBox from "../combobox";
import CatalogFolderReactSvg from "PUBLIC_DIR/images/catalog.folder.react.svg";
import CheckReactSvgUrl from "PUBLIC_DIR/images/check.react.svg?url";
import ItemActiveReactSvgUrl from "PUBLIC_DIR/images/item.active.react.svg?url";

export default {
  title: "Components/Row",
  component: Row,
  parameters: {
    docs: { description: { component: "Displays content as row" } },
  },
  argTypes: {
    element: {
      control: {
        type: "select",
        options: ["", "Avatar", "Icon", "ComboBox"],
      },
    },
    content: { control: "text", description: "Displays the row content" },
    contextButton: {
      description: "Enables displaying the submenu",
    },
    onSelectComboBox: { action: "onSelectComboBox", table: { disable: true } },
    contextItemClick: { action: "contextItemClick", table: { disable: true } },
    checkbox: { description: "Disable checkbox" },
  },
};

const elementAvatar = <Avatar size="min" role="user" userName="Demo Avatar" />;
const elementIcon = <CatalogFolderReactSvg size="big" />;

const renderElementComboBox = (onSelect) => (
  <ComboBox
    options={[
      {
        key: 1,
        icon: ItemActiveReactSvgUrl,
        label: "Open",
      },
      { key: 2, icon: CheckReactSvgUrl, label: "Closed" },
    ]}
    onSelect={(option) => onSelect(option)}
    selectedOption={{
      key: 0,
      icon: ItemActiveReactSvgUrl,
      label: "",
    }}
    scaled={false}
    size="content"
    isDisabled={false}
  />
);

const Template = ({
  element,
  contextButton,
  content,
  onSelectComboBox,
  contextItemClick,
  checkbox,
  checked,
  ...args
}) => {
  const getElementProps = (element) =>
    element === "Avatar"
      ? { element: elementAvatar }
      : element === "Icon"
      ? { element: elementIcon }
      : element === "ComboBox"
      ? { element: renderElementComboBox(onSelectComboBox) }
      : {};

  const elementProps = getElementProps(element);
  const checkedProps = checkbox ? { checked: checked } : {};
  return (
    <Row
      {...args}
      key="1"
      style={{ width: "20%" }}
      {...checkedProps}
      {...elementProps}
      contextOptions={
        contextButton
          ? [
              {
                key: "key1",
                label: "Edit",
                onClick: () => contextItemClick("Context action: Edit"),
              },
              {
                key: "key2",
                label: "Delete",
                onClick: () => contextItemClick("Context action: Delete"),
              },
            ]
          : []
      }
    >
      <Text truncate={true}>{content}</Text>
    </Row>
  );
};

export const Default = Template.bind({});
Default.args = {
  contextButton: true,
  checked: true,
  element: "",
  content: "Sample text",
  checkbox: true,
};
