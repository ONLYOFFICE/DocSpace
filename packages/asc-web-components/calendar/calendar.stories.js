import React from "react";
import Calendar from "./";

export default {
  title: "Components/Calendar",
  component: Calendar,
  argTypes: {
    themeColor: { control: "color", description: "Color of the selected day" },
    maxDate: {
      control: "date",
      description: "Maximum date that the user can select",
    },
    selectedDate: { control: "date", description: "Selected date value" },
    openToDate: {
      control: "date",
      description:
        "The beginning of a period that shall be displayed by default",
    },
    minDate: {
      control: "date",
      description: "Minimum date that the user can select.",
    },
    locale: {
      description: "Browser locale",
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
    onChange: {
      description: "Function called when the user select a day",
      action: "onChange",
    },
    size: { description: "Calendar size" },
    className: { description: "Accepts class" },
    id: { description: "Accepts id" },
    style: { description: "Accepts css style" },
  },
  parameters: {
    docs: {
      description: {
        component: "Used to display custom calendar",
      },
      source: {
        code: `
      import Calendar from "@appserver/components/calendar";

<Calendar
  onChange={(date) => {
    console.log("Selected date:", date);
  }}
  disabled={false}
  themeColor="#ED7309"
  selectedDate={new Date()}
  openToDate={new Date()}
  minDate={new Date("1970/01/01")}
  maxDate={new Date("3000/01/01")}
  locale="ru"
/>
`,
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
