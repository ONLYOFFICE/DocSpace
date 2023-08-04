import Submenu from ".";
import React from "react";
import { data, startSelect } from "./data";

export default {
  title: "Components/Submenu",
  component: Submenu,
};

const Wrapper = (props) => (
  <div
    style={{
      height: "170px",
    }}
  >
    {props.children}
  </div>
);

const Template = (args) => (
  <Wrapper>
    <Submenu {...args} />
  </Wrapper>
);

export const Default = Template.bind({});
Default.args = {
  data: data,
  startSelect: startSelect,
};
