import React from "react";
import { mount } from "enzyme";
import RadioButton from ".";

const baseProps = {
  name: "fruits",
  value: "apple",
  label: "Sweet apple",
};

describe("<RadioButton />", () => {
  it("renders without error", () => {
    const wrapper = mount(<RadioButton {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(<RadioButton {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<RadioButton {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <RadioButton {...baseProps} style={{ color: "red" }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });

  it("accepts isDisabled prop", () => {
    const wrapper = mount(<RadioButton {...baseProps} isDisabled />);

    expect(wrapper.prop("isDisabled")).toEqual(true);
  });
});
