import React from "react";
import { mount } from "enzyme";
import RadioButtonGroup from ".";

const baseProps = {
  name: "fruits",
  selected: "banana",
  options: [
    { value: "apple", label: "Sweet apple" },
    { value: "banana", label: "Banana" },
    { value: "Mandarin" },
  ],
};

describe("<RadioButtonGroup />", () => {
  it("renders without error", () => {
    const wrapper = mount(<RadioButtonGroup {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(<RadioButtonGroup {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<RadioButtonGroup {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <RadioButtonGroup {...baseProps} style={{ color: "red" }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });

  it("accepts isDisabled prop", () => {
    const wrapper = mount(<RadioButtonGroup {...baseProps} isDisabled />);

    expect(wrapper.prop("isDisabled")).toEqual(true);
  });
});
