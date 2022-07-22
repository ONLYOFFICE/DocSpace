import React from "react";
import FileInput from "./";

export default {
  title: "Components/FileInput",
  component: FileInput,
  argTypes: {
    onInput: { action: "onInput" },
  },
  parameters: {
    docs: {
      description: {
        component: "File entry field",
      },
    },
  },
};

const Template = (args) => {
  return (
    <FileInput
      {...args}
      onInput={(file) => {
        args.onInput(
          `File: ${file},
          name: ${file.name},
          lastModified: ${file.lastModifiedDate},
          size: ${file.size}`
        );
      }}
    />
  );
};

export const Default = Template.bind({});
Default.args = {
  placeholder: "Input file",
  size: "base",
  scale: false,
  isDisabled: false,
  id: "file-input-id",
  name: "demoFileInputName",
  hasError: false,
  hasWarning: false,
  accept: ".doc, .docx",
};
