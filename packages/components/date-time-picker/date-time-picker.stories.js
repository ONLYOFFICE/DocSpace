import React, { useEffect, useState } from "react";
import DateTimePicker from "./";
import styled from "styled-components";
import moment from "moment";

export default {
  title: "Components/DateTimePicker",
  component: DateTimePicker,
  argTypes: {
    isApplied: { control: "boolean" },
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
  const [isApplied, setIsApplied] = useState(false);
  const [dateSpan, setDateSpan] = useState({
    from: moment(),
    to: moment(),
  });

  const onChange = (dateObj) => {
    const formattedDate = dateObj.date?.format("YYYY-MM-DD");
    setDateSpan({
      from: moment(
        `${formattedDate} ${dateObj.timeFrom.format("HH-mm")}`,
        "YYYY-MM-DD HH-mm"
      ),
      to: moment(
        `${formattedDate} ${dateObj.timeTo.format("HH-mm")}`,
        "YYYY-MM-DD HH-mm"
      ),
    });
  };

  useEffect(() => {
    setIsApplied(args.isApplied);
  }, [args.isApplied]);

  return (
    <Wrapper>
      <DateTimePicker
        {...args}
        isApplied={isApplied}
        setIsApplied={setIsApplied}
        onChange={onChange}
      />
    </Wrapper>
  );
};

export const Default = Template.bind({});
