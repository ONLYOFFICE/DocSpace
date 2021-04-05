import React from "react";
import { mount } from "enzyme";
import Label from ".";

const baseProps = {
  text: "First name:",
  title: "first name",
  htmlFor: "firstNameField",
  display: "block",
};

describe("<Label />", () => {
  it("renders without error", () => {
    const wrapper = mount(<Label {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("display error", () => {
    const wrapper = mount(<Label {...baseProps} error />);

    expect(wrapper.prop("error")).toBe(true);
  });

  it("display required", () => {
    const wrapper = mount(<Label {...baseProps} isRequired />);

    expect(wrapper.prop("isRequired")).toBe(true);
  });

  it("accepts id", () => {
    const wrapper = mount(<Label {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<Label {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(<Label {...baseProps} style={{ color: "red" }} />);

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
