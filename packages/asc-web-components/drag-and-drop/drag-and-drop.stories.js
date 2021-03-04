import React from "react";
import DragAndDrop from "./";
import Text from "../text";

export default {
  title: "Components/DragAndDrop",
  component: DragAndDrop,
  argTypes: {
    dragging: { description: "Show that the item is being dragged now." },
    isDropZone: { description: "Sets the component as a dropzone" },
    onDrop: {
      action: "onDrop",
      description:
        "Occurs when the dragged element is dropped on the drop target",
    },
    targetFile: { action: "File: ", table: { disable: true } },
    className: { description: "Accepts class" },
    onMouseDown: {
      description: "Occurs when the mouse button is pressed",
      action: "onMouseDown",
    },
    children: { table: { disable: true } },
  },
  parameters: {
    docs: {
      description: {
        component: `Drag And Drop component can be used as Dropzone
        See documentation: https://github.com/react-dropzone/react-dropzone
        `,
      },
      source: {
        code: `
        import DragAndDrop from "@appserver/components/drag-and-drop";

<DragAndDrop onDrop={onDrop} style={width: 200, height: 200, border: "5px solid #999"}>
  <Text style={textStyles} color="#999" fontSize="20px">
    Drop items here
  </Text>
</DragAndDrop>
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
        args.targetFile(file.name);
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

