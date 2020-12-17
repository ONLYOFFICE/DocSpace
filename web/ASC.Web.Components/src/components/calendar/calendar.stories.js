import React from "react";
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import { withKnobs, color, select, date } from "@storybook/addon-knobs/react";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import Calendar from ".";
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

const arraySize = ["base", "big"];

storiesOf("Components|Calendar", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => (
    <Section>
      <Calendar
        onChange={(date) => {
          action("Selected date")(date);
        }}
        themeColor={color("themeColor", "#ED7309")}
        selectedDate={myDateKnob("selectedDate", new Date())}
        openToDate={myDateKnob("openToDate", new Date())}
        minDate={myDateKnob("minDate", new Date("1970/01/01"))}
        maxDate={myDateKnob(
          "maxDate",
          new Date(new Date().getFullYear() + 1 + "/01/01")
        )}
        locale={select("locale", locales, "en")}
        size={select("size", arraySize, "base")}
      />
    </Section>
  ));
