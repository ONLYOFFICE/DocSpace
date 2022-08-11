import React from "react";

import Label from ".";

export default {
  title: "Components/Label",
  component: Label,
  parameters: {
    docs: {
      description: {
        component: "Component displays the field name in the form",
      },
    },
  },
};

const Template = (args) => {
  return <Label {...args} />;
};

export const Default = Template.bind({});
Default.args = {
  isRequired: false,
  error: false,
  isInline: false,
  title: "Fill the first name field",
  truncate: false,
  htmlFor: "htmlFor",
  text: "First name:",
  display: "display",
};
