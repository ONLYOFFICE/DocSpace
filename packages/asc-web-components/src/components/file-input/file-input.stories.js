import React from "react";
import { storiesOf } from "@storybook/react";
import { text, boolean, withKnobs, select } from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import FileInput from ".";
import Section from "../../../.storybook/decorators/section";
import { action } from "@storybook/addon-actions";

const sizeInput = ["base", "middle", "big", "huge", "large"];

storiesOf("Components|Input", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("file input", () => {
    const placeholder = text("placeholder", "Input file");
    const size = select("size", sizeInput, "base");
    const scale = boolean("scale", false);
    const isDisabled = boolean("isDisabled", false);
    const id = text("id", "fileInputId");
    const name = text("name", "demoFileInputName");
    const hasError = boolean("hasError", false);
    const hasWarning = boolean("hasWarning", false);
    const accept = text("accept", ".doc, .docx");

    return (
      <Section>
        <FileInput
          id={id}
          name={name}
          placeholder={placeholder}
          size={size}
          scale={scale}
          isDisabled={isDisabled}
          hasError={hasError}
          hasWarning={hasWarning}
          accept={accept}
          onInput={(file) => {
            action("onInput")(file);
            console.log(
              `name: ${file.name}`,
              `lastModified: ${file.lastModifiedDate}`,
              `size: ${file.size}`
            );
          }}
        />
      </Section>
    );
  });
