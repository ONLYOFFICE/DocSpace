import React from "react";
import { storiesOf } from "@storybook/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import DragAndDrop from "./";
import { Text } from "asc-web-components";

storiesOf("Components| DragAndDrop", module)
  .addDecorator(withReadme(Readme))
  .add("base", () => {

    const traverseFileTree = (item, path) => {
      if (item.isFile) {
        item.file(file => console.log("File:", path + file.name));
      } else if (item.isDirectory) {
        const dirReader = item.createReader();
        dirReader.readEntries(entries => {
          for (let i = 0; i < entries.length; i++) {
            traverseFileTree(entries[i], path + item.name + "/");
          }
        });
      }
    }

    const onDrop = event => {
      console.log("onDrop", event);
      const items = event.dataTransfer.items;
      for (let i = 0; i < items.length; i++) {
        const item = items[i].webkitGetAsEntry();
        if (item) {
          traverseFileTree(item, "");
        }
      }
    }

    const dropDownStyles = { margin: 16, width: 200, height: 200, border: "5px solid #999" };
    const textStyles = {textAlign: "center", lineHeight: "9.5em"};

    return(
      <DragAndDrop onDrop={onDrop} style={dropDownStyles}>
        <Text style={textStyles} color="#999" fontSize="20px">Drop items here</Text>
      </DragAndDrop>
    )});
