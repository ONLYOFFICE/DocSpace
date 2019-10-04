import React from "react";
import { mount } from "enzyme";
import QuestionIcon from "./";
import { Text } from "../text";

const dropDownBody = (
  <Text.Body style={{ padding: "16px" }}>
    {`Время существования сессии по умолчанию составляет 20 минут.
    Отметьте эту опцию, чтобы установить значение 1 год. 
    Чтобы задать собственное значение, перейдите в настройки.`}
  </Text.Body>
);

describe("<QuestionIcon />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <QuestionIcon
        dropDownBody={dropDownBody}
        dropDownDirectionY="bottom"
        dropDownDirectionX="left"
        dropDownManualY={0}
        dropDownManualX={0}
        dropDownManualWidth={300}
        backgroundColor="#fff"
        isOpen={false}
        size={12}
        onClick={jest.fn()}
      />
    );

    expect(wrapper).toExist();
  });
});
