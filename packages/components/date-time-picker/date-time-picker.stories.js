import React, { useState } from "react";
import DateTimePicker from "./";
import styled from "styled-components";
import moment from "moment";

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
  const [date, setDate] = useState(null);

  return (
    <Wrapper>
      <DateTimePicker {...args} date={date} onChange={setDate} />
    </Wrapper>
  );
};

export const Default = Template.bind({});
