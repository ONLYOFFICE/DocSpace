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
        component: `File entry field

### Properties

| Props         |      Type      | Required |                  Values                  | Default | Description                                                                        |
| ------------- | :------------: | :------: | :--------------------------------------: | :-----: | ---------------------------------------------------------------------------------- |
| className   |    string    |    -     |                    -                     |    -    | Accepts class                                                                      |
| hasError    |     bool     |    -     |                    -                     | false | Indicates the input field has an error                                             |
| hasWarning  |     bool     |    -     |                    -                     | false | Indicates the input field has a warning                                            |
| id          |    string    |    -     |                    -                     |    -    | Used as HTML 'id' property                                                         |
| isDisabled  |     bool     |    -     |                    -                     | false | Indicates that the field cannot be used (e.g not authorised, or changes not saved) |
| name        |    string    |    -     |                    -                     |    -    | Used as HTML     'name' property                                                       |
| onInput     |     func     |    -     |                    -                     |    -    | Called when a file is selected                                                     |
| placeholder |    string    |    -     |                    -                     |    -    | Placeholder text for the input                                                     |
| scale       |     bool     |    -     |                    -                     | false | Indicates the input field has scale                                                |
| size        |    string    |    -     | base, middle, big, huge, large | base  | Supported size of the input fields.                                                |
| style       | obj, array |    -     |                    -                     |    -    | Accepts css style                                                                  |
| accept      |    string    |    -     |                    -                     |    -    | Specifies files visible for upload                                                 |

        `,
      },
      source: {
        code: `import FileInput from "@appserver/components/file-input";
<FileInput
  placeholder="Input file"
  accept=".doc, .docx"
  onInput={(file) => {
    console.log(
      file,
      "name: ", file.name},
      "lastModified: ", file.lastModifiedDate},
      "size: ", file.size}
    );
  }}
/>`,
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
