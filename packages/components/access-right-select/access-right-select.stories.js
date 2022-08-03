import React from "react";

import AccessRightSelect from "./";
import { data } from "./data";

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
  options: data,
  selectedOption: data[0],
};
