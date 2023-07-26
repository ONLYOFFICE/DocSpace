import React from "react";
import DragAndDrop from "./";
import Text from "../text";

export default {
  title: "Components/DragAndDrop",
  component: DragAndDrop,
  argTypes: {
    onDrop: { action: "onDrop" },
    targetFile: { action: "File: ", table: { disable: true } },
    onMouseDown: { action: "onMouseDown" },
  },
  parameters: {
    docs: {
      description: {
        component: `Drag And Drop component can be used as Dropzone.

See documentation: https://github.com/react-dropzone/react-dropzone
        `,
      },
    },
  },
};

const Template = (args) => {
  const onDrop = (items) => {
    args.onDrop(items);
    for (let file of items) {
      if (file) {
        return args.targetFile(file.name);
      }
    }
  };

  const dropDownStyles = { margin: 16, width: 200, height: 200 };
  const textStyles = { textAlign: "center", lineHeight: "9.5em" };

  return (
    <DragAndDrop {...args} isDropZone onDrop={onDrop} style={dropDownStyles}>
      <Text style={textStyles} color="#999" fontSize="20px">
        Drop items here
      </Text>
    </DragAndDrop>
  );
};

export const Default = Template.bind({});
