import React from "react";
import Calendar from "./";

export default {
  title: "Components/Calendar",
  component: Calendar,
  argTypes: {
    themeColor: { control: "color" },
    selectedDate: { control: "date" },
    openToDate: { control: "date" },
    maxDate: { control: "date" },
    minDate: { control: "date" },
    locale: {
      control: {
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

const Template = (args) => {
  return (
    <Calendar
      {...args}
      maxDate={new Date(args.maxDate)}
      selectedDate={new Date(args.selectedDate)}
      openToDate={new Date(args.openToDate)}
      minDate={new Date(args.minDate)}
      locale="en"
    />
  );
};

export const Default = Template.bind({});
Default.args = {
  maxDate: new Date(new Date().getFullYear() + 1 + "/01/01"),
  minDate: new Date("1970/01/01"),
  selectedDate: new Date(),
  openToDate: new Date(),
};
