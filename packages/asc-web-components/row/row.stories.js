import React from "react";
import Row from ".";
import Text from "../text";
import Avatar from "../avatar";
import ComboBox from "../combobox";
import CatalogFolderIcon from "../public/static/images/catalog.folder.react.svg";

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
    content: { control: "text" },
    onSelectComboBox: { action: "onSelectComboBox", table: { disable: true } },
    contextItemClick: { action: "contextItemClick", table: { disable: true } },
    checkbox: { description: "Disable checkbox" },
  },
};

const elementAvatar = <Avatar size="min" role="user" userName="Demo Avatar" />;
const elementIcon = <CatalogFolderIcon size="big" />;

const renderElementComboBox = (onSelect) => (
  <ComboBox
    options={[
      {
        key: 1,
        icon: "static/images/item.active.react.svg",
        label: "Open",
      },
      { key: 2, icon: "static/images/check.react.svg", label: "Closed" },
    ]}
    onSelect={(option) => onSelect(option)}
    selectedOption={{
      key: 0,
      icon: "static/images/item.active.react.svg",
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
