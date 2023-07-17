import React from "react";

import AccessRightSelect from "./";
import { data } from "./data";

export default {
  title: "Components/AccessRightSelect",
  component: AccessRightSelect,
};

const Wrapper = (props) => (
  <div
    style={{
      height: "420px",
    }}
  >
    {props.children}
  </div>
);

const Template = (args) => (
  <Wrapper>
    <AccessRightSelect {...args} />
  </Wrapper>
);

export const Default = Template.bind({});

Default.args = {
  accessOptions: data,
  selectedOption: data[0],
  scaledOptions: false,
  scaled: false,
  size: "content",
  manualWidth: "fit-content",
};
