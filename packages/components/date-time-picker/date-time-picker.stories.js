import React from "react";
import DateTimePicker from "./";
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
  title: "Components/DateTimePicker",
  component: DateTimePicker,
  argTypes: {
    minDate: { control: "date" },
    maxDate: { control: "date" },
    initialDate: { control: "date" },
    openDate: { control: "date" },
    onChange: { action: "onChange" },
    locale: { control: "select", options: locales },
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

Default.args = {
  locale: "en",
  maxDate: new Date(new Date().getFullYear() + 10 + "/01/01"),
  minDate: new Date("1970/01/01"),
  openDate: new Date(),
};
