import React from "react";
import { mount } from "enzyme";
import SelectorAddButton from ".";

const baseProps = {
  title: "Add item",
  isDisabled: false,
};

describe("<SelectorAddButton />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <SelectorAddButton {...baseProps}></SelectorAddButton>
    );

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(<SelectorAddButton {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(
      <SelectorAddButton {...baseProps} className="test" />
    );

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <SelectorAddButton {...baseProps} style={{ color: "red" }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
