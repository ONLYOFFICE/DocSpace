import React, { useState } from "react";
import DatePicker from "./";
import styled from "styled-components";

const locales = [
  "az",
  "zh-cn",
  "cs",
  "nl",
  "en-gb",
  "en",
  "fi",
  "fr",
  "de",
  "de-ch",
  "el",
  "it",
  "ja",
  "ko",
  "lv",
  "pl",
  "pt",
  "pt-br",
  "ru",
  "sk",
  "sl",
  "es",
  "tr",
  "uk",
  "vi",
];

export default {
  title: "Components/DatePicker",
  component: DatePicker,

  argTypes: {
    minDate: { control: "date" },
    maxDate: { control: "date" },
    onChange: { action: "onChange" },
    locale: { control: "select", options: locales },
  },
  parameters: {
    docs: {
      description: {
        component: "Date input",
      },
    },
    design: {
      type: "figma",
      url: "https://www.figma.com/file/9AtdOHnhjhZCIRDrj4Unta/Public-room?type=design&node-id=1846-218508&mode=design&t=xSsXehQdoxpp5o7F-4",
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
      <DatePicker {...args} date={date} setDate={setDate} />
    </Wrapper>
  );
};

export const Default = Template.bind({});
