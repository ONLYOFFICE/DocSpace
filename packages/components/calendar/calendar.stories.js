import React, { useState } from "react";
import Calendar from "./";
import moment from "moment";

export default {
  title: "Components/Calendar",
  component: Calendar,
  argTypes: {
    maxDate: { control: "date" },
    minDate: { control: "date" },
    initialDate: { control: "date" },
    locale: {
      type: "select",
      options: [
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
      ],
    },
    onChange: { action: "onChange" },
  },
  parameters: {
    docs: {
      description: {
        component: "Used to display custom calendar",
      },
    },
  },
};

const Template = ({ locale, minDate, maxDate, ...args }) => {
  const [selectedDate, setSelectedDate] = useState(moment());
  return (
    <Calendar
      selectedDate={selectedDate}
      setSelectedDate={setSelectedDate}
      minDate={minDate}
      maxDate={maxDate}
      locale={locale}
      {...args}
    />
  );
};

export const Default = Template.bind({});
Default.args = {
  locale: "en",
  maxDate: new Date(new Date().getFullYear() + 1 + "/01/01"),
  minDate: new Date("1970/01/01"),
  id: "",
  className: "",
  initialDate: new Date(),
};
