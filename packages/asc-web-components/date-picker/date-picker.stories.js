import React from "react";
import DatePicker from "./";

export default {
  title: "Components/DatePicker",
  component: DatePicker,
  decorators: [
    (Story) => (
      <div style={{ height: "380px" }}>
        <Story />
      </div>
    ),
  ],
  argTypes: {
    themeColor: { description: "Color of the selected day", control: "color" },
    selectedDate: { description: "Selected date value", control: "date" },
    openToDate: { description: "Opened date value", control: "date" },
    minDate: {
      description: "Minimum date that the user can select.",
      control: "date",
    },
    maxDate: {
      description: "Maximum date that the user can select.",
      control: "date",
    },
    calendarHeaderContent: {
      description: "Calendar header content (calendar opened in aside)",
    },
    calendarSize: { description: "Calendar size" },
    className: { description: "Accepts class " },
    displayType: { description: "Calendar display type " },
    hasError: { description: "Set error date-input style" },
    id: { description: "Accepts id " },
    isDisabled: { description: "Disabled react-calendar" },
    isOpen: { description: "Opens calendar" },
    isReadOnly: { description: "Set input type is read only" },
    locale: { description: "Browser locale" },
    onChange: {
      description: "Function called when the user select a day ",
      action: "onChange",
    },
    scaled: { description: "Selected calendar size" },
    style: { description: "Accepts css style" },
    zIndex: { description: "Calendar css z-index" },
  },
  parameters: {
    docs: {
      description: {
        component: "Base DatePicker component",
      },
      source: {
        code: `
        import DatePicker from "@appserver/components/date-picker";

<DatePicker
  onChange={(date) => {
    console.log("Selected date", date);
  }}
  selectedDate={new Date()}
  minDate={new Date("1970/01/01")}
  maxDate={new Date(new Date().getFullYear() + 1 + "/01/01")}
  isDisabled={false}
  isReadOnly={false}
  hasError={false}
  isOpen={false}
  themeColor="#ED7309"
  locale="en"
/>
        `,
      },
    },
  },
};

const Template = (args) => {
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
  return (
    <DatePicker
      {...args}
      onChange={(date) => {
        args.onChange(date);
      }}
      selectedDate={new Date(args.selectedDate)}
      minDate={new Date(args.minDate)}
      maxDate={new Date(args.maxDate)}
      openToDate={new Date(args.openToDate)}
    />
  );
};

export const Default = Template.bind({});
Default.args = {
  isOpen: true,
  calendarHeaderContent: "Select Date",
  themeColor: "#ED7309",
  minDate: new Date("1970/01/01"),
  selectedDate: new Date(),
  maxDate: new Date(new Date().getFullYear() + 1 + "/01/01"),
  openToDate: new Date(),
  calendarSize: "base",
};
