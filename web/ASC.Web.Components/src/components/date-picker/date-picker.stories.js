import React from "react";
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import {
  withKnobs,
  boolean,
  color,
  select,
  date,
  number,
  text,
} from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import DatePicker from ".";
import Section from "../../../.storybook/decorators/section";

function myDateKnob(name, defaultValue) {
  const stringTimestamp = date(name, defaultValue);
  return new Date(stringTimestamp);
}

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

const displayType = ["dropdown", "aside", "auto"];

storiesOf("Components|DatePicker", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => (
    <Section>
      <DatePicker
        onChange={(date) => {
          action("Selected date")(date);
        }}
        selectedDate={myDateKnob("selectedDate", new Date())}
        minDate={myDateKnob("minDate", new Date("1970/01/01"))}
        maxDate={myDateKnob(
          "maxDate",
          new Date(new Date().getFullYear() + 1 + "/01/01")
        )}
        isDisabled={boolean("isDisabled", false)}
        isReadOnly={boolean("isReadOnly", false)}
        hasError={boolean("hasError", false)}
        isOpen={boolean("isOpen", false)}
        themeColor={color("themeColor", "#ED7309")}
        locale={select("locale", locales, "en")}
        displayType={select("displayType", displayType, "auto")}
        calendarSize={select("calendarSize", ["base", "big"], "base")}
        zIndex={number("zIndex", 310)}
        calendarHeaderContent={text("headerContent", "Select Date")}
      />
    </Section>
  ));
