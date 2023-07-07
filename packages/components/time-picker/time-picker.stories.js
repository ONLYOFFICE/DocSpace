import React, { useState } from "react";
import TimePicker from "./";
import moment from "moment";

export default {
  title: "Components/TimePicker",
  component: TimePicker,
  argTypes: {
    hasError: { control: "boolean" },
    onChange: { action: "onChange" },
  },
  parameters: {
    docs: {
      description: {
        component: "Time input",
      },
    },
  },
};

const Template = ({ ...args }) => {
  const [selectedDate, setSelectedDate] = useState(moment());
  return (
    <TimePicker
      date={selectedDate}
      setDate={setSelectedDate}
      hasError={false}
      onChange={(date) => console.log(date)}
      {...args}
    />
  );
};

export const Default = Template.bind({});
