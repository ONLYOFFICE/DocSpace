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
    design: {
      type: "figma",
      url: "https://www.figma.com/file/ZiW5KSwb4t7Tj6Nz5TducC/UI-Kit-DocSpace-1.0.0?type=design&node-id=651-4406&mode=design&t=RrB9MOQGCnUPghij-0",
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
  maxDate: new Date(new Date().getFullYear() + 10 + "/01/01"),
  minDate: new Date("1970/01/01"),
  initialDate: new Date(),
};
