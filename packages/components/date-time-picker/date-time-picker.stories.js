import React from "react";
import DateTimePicker from "./";
import styled from "styled-components";

export default {
  title: "Components/DateTimePicker",
  component: DateTimePicker,
  argTypes: {
    onChange: { action: "onChange" },
  },
  parameters: {
    docs: {
      description: {
        component: "Date-time input",
      },
    },
  },
};

const Wrapper = styled.div`
  height: 500px;
`;

const Template = ({ ...args }) => {
  return (
    <Wrapper>
      <DateTimePicker {...args} />
    </Wrapper>
  );
};

export const Default = Template.bind({});
