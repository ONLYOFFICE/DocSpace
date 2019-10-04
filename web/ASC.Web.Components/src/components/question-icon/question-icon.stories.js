import React from "react";
import { storiesOf } from "@storybook/react";
import {
  withKnobs,
  boolean,
  number,
  color,
  select
} from "@storybook/addon-knobs/react";
import { action } from "@storybook/addon-actions";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import QuestionIcon from "./";
import { Text } from "../text";
import Section from "../../../.storybook/decorators/section";

const dropDownBody = (
  <Text.Body style={{ padding: "16px" }}>
    {`Время существования сессии по умолчанию составляет 20 минут.
    Отметьте эту опцию, чтобы установить значение 1 год. 
    Чтобы задать собственное значение, перейдите в настройки.`}
  </Text.Body>
);

storiesOf("Components|QuestionIcon", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => {
    return (
      <Section>
        <QuestionIcon
          dropDownBody={dropDownBody}
          dropDownDirectionY={select(
            "dropDownDirectionY",
            ["top", "bottom"],
            "bottom"
          )}
          dropDownDirectionX={select(
            "dropDownDirectionX",
            ["left", "right"],
            "left"
          )}
          dropDownManualY={number("dropDownManualY", 0)}
          dropDownManualX={number("dropDownManualX", 0)}
          dropDownManualWidth={number("dropDownWidth", 300)}
          backgroundColor={color("backgroundColor", "#fff")}
          isOpen={boolean("isOpen", false)}
          size={number("size", 12)}
          onClick={() => {
            action("QuestionIcon clicked")();
          }}
        />
      </Section>
    );
  });
