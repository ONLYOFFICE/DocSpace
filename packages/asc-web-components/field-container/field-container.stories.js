import React, { useState } from "react";
import FieldContainer from ".";
import TextInput from "../text-input";

export default {
  title: "Components/FieldContainer",
  component: FieldContainer,
  argTypes: {
    errorColor: { control: "color" },
  },
  parameters: {
    docs: {
      description: {
        component: `Responsive form field container
        
### Properties

| Props                     |       Type        | Required | Values |  Default  | Description                                      |
| ------------------------- | :---------------: | :------: | :----: | :-------: | ------------------------------------------------ |
| className               |     string      |    -     |   -    |     -     | Accepts class                                    |
| errorColor              |     string      |    -     |   -    | ![#C96C27](https://placehold.it/15/C96C27/000000?text=+) #C96C27 | Error text color                                 |
| errorMessageWidth       |     string      |    -     |   -    |  320px  | Error text width                                 |
| errorMessage            |     string      |    -     |   -    |     -     | Error message text                               |
| hasError                |      bool       |    -     |   -    |  false  | Indicates that the field is incorrect            |
| helpButtonHeaderContent |     string      |    -     |   -    |     -     | Tooltip header content (tooltip opened in aside) |
| id                      |     string      |    -     |   -    |     -     | Accepts id                                       |
| isRequired              |      bool       |    -     |   -    |  false  | Indicates that the field is required to fill     |
| isVertical              |      bool       |    -     |   -    |  false  | Vertical or horizontal alignment                 |
| labelText               |     string      |    -     |   -    |     -     | Field label text                                 |
| labelVisible            |      bool       |    -     |   -    |  true   | Sets visibility of field label section           |
| maxLabelWidth           |     string      |    -     |   -    |  110px  | Max label width in horizontal alignment          |
| style                   |  obj, array   |    -     |   -    |     -     | Accepts css style                                |
| tooltipContent          | object, string |    -     |   -    |     -     | Tooltip content                                  |
`,
      },
      source: {
        code: `import FieldContainer from "@appserver/components/field-container";

<FieldContainer labelText="Name:">
  <TextInput value="" onChange={(e) => console.log(e.target.value)} />
</FieldContainer>`,
      },
    },
  },
};

const Template = (args) => {
  const [value, setValue] = useState("");
  return (
    <FieldContainer {...args}>
      <TextInput
        value={value}
        hasError={args.hasError}
        className="field-input"
        onChange={(e) => {
          setValue(e.target.value);
        }}
      />
    </FieldContainer>
  );
};

export const Default = Template.bind({});
Default.args = {
  isVertical: false,
  isRequired: false,
  hasError: false,
  labelVisible: true,
  labelText: "Name:",
  maxLabelWidth: "110px",
  tooltipContent: "Paste you tooltip content here",
  helpButtonHeaderContent: "Tooltip header",
  place: "top",
  errorMessage:
    "Error text. Lorem ipsum dolor sit amet, consectetuer adipiscing elit",
  errorColor: "#C96C27",
  errorMessageWidth: "293px",
};
Default.parameters = {
  decorators: [
    (Story) => (
      <div style={{ marginTop: 100, marginLeft: 50 }}>
        <Story />
      </div>
    ),
  ],
};
/*
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import { StringValue } from "react-values";
import { text, boolean, withKnobs, color } from "@storybook/addon-knobs/react";
import FieldContainer from ".";
import TextInput from "../text-input";
import Section from "../../../.storybook/decorators/section";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";

storiesOf("Components|FieldContainer", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => (
    <StringValue
      onChange={(e) => {
        action("onChange")(e);
      }}
    >
      {({ value, set }) => (
        <Section>
          <div style={{ marginTop: 100, marginLeft: 50 }}>
            <FieldContainer
              isVertical={boolean("isVertical", false)}
              isRequired={boolean("isRequired", false)}
              hasError={boolean("hasError", false)}
              labelVisible={boolean("labelVisible", true)}
              labelText={text("labelText", "Name:")}
              maxLabelWidth={text("maxLabelWidth", "110px")}
              tooltipContent={text(
                "tooltipContent",
                "Paste you tooltip content here"
              )}
              helpButtonHeaderContent={text(
                "helpButtonHeaderContent",
                "Tooltip header"
              )}
              place="top"
              errorMessage={text(
                "errorMessage",
                "Error text. Lorem ipsum dolor sit amet, consectetuer adipiscing elit"
              )}
              errorColor={color("errorColor", "#C96C27")}
              errorMessageWidth={text("errorMessageWidth", "293px")}
            >
              <TextInput
                value={value}
                hasError={boolean("hasError", false)}
                className="field-input"
                onChange={(e) => {
                  set(e.target.value);
                }}
              />
            </FieldContainer>
          </div>
        </Section>
      )}
    </StringValue>
  ));
*/
