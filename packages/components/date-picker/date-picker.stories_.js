import React from "react";
import DatePicker from "./";

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
    selectedDate: { control: "date" },
    initialDate: { control: "date" },
    minDate: { control: "date" },
    maxDate: { control: "date" },
    onChange: { action: "onChange" },
    locale: { control: { type: "select", options: locales } },
  },
  parameters: {
    docs: {
      description: {
        component: "Base DatePicker component",
      },
    },
  },
};

const Template = (args) => {
  console.log(args.locale);
  return (
    <div style={{ height: "380px" }}>
      <DatePicker
        {...args}
        onChange={(date) => {
          args.onChange(date);
        }}
        selectedDate={new Date(args.selectedDate)}
        minDate={new Date(args.minDate)}
        maxDate={new Date(args.maxDate)}
        initialDate={new Date(args.initialDate)}
      />
    </div>
  );
};

export const Default = Template.bind({});
Default.args = {
  isOpen: true,
  calendarHeaderContent: "Select Date",
  minDate: new Date("1970/01/01"),
  selectedDate: new Date(),
  maxDate: new Date(new Date().getFullYear() + 1 + "/01/01"),
  initialDate: new Date(),
  locale: "en",
};
