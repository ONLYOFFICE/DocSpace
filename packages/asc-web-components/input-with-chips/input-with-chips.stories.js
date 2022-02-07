import React from "react";
import InputWithChips from ".";

const Options = [
  { label: "Ivan Petrov", value: "myname@gmul.com" },
  { label: "Donna Cross", value: "myname45@gmul.com" },
  { label: "myname@gmul.co45", value: "myname@gmul.co45" },
  { label: "Lisa Cooper", value: "myn348ame@gmul.com" },
  { label: "myname19@gmail.com", value: "myname19@gmail.com" },
  { label: "myname@gmail.com", value: "myname@gmail.com" },
  {
    label: "mynameiskonstantine1353434@gmail.com",
    value: "mynameiskonstantine1353434@gmail.com",
  },
];

const Wrapper = (props) => (
  <div
    style={{
      height: "220px",
    }}
  >
    {props.children}
  </div>
);

const Template = (args) => (
  <Wrapper>
    <InputWithChips {...args} />
  </Wrapper>
);

export const Default = Template.bind({});
Default.args = {
  options: Options,
  onChange: (selected) => console.log(selected),
  placeholder: "Invite by email",
};

export const Empty = Template.bind({});
Empty.args = {
  options: [],
  placeholder: "Type your chips...",
};
