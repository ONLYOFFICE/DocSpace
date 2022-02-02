import React from "react";
import InputWithChips from ".";

const Wrapper = (props) => (
  <div
    style={{
      height: "420px",
    }}
  >
    {props.children}
  </div>
);

const Template = () => (
  <Wrapper>
    <InputWithChips
      chips={[
        { label: "Ivan Petrov", value: "myname@gmul.com" },
        { label: "Lisa Cooper", value: "myn348ame@gmul.com" },
        { label: "Donna Cross", value: "myname45@gmul.com" },
        { label: "myname19@gmail.com", value: "myname19@gmail.com" },
        { label: "myname@gmail.com45", value: "myname@gmail.com45" },
      ]}
      placeholder="Invite by email"
    />
  </Wrapper>
);

export const Default = Template.bind({});
Default.args = {};
