import React from "react";
import { storiesOf } from "@storybook/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import DragAndDrop from "./";
import Text from "../text";

storiesOf("Components| DragAndDrop", module)
  .addDecorator(withReadme(Readme))
  .add("base", () => {
    const onDrop = (items) => {
      console.log("onDrop", items);
      for (let file of items) {
        if (file) {
          console.log("File:", file.name);
        }
      }
    };

    const dropDownStyles = { margin: 16, width: 200, height: 200 };
    const textStyles = { textAlign: "center", lineHeight: "9.5em" };

    return (
      <DragAndDrop isDropZone onDrop={onDrop} style={dropDownStyles}>
        <Text style={textStyles} color="#999" fontSize="20px">
          Drop items here
        </Text>
      </DragAndDrop>
    );
  });
