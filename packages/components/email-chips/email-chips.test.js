import React from "react";
import { mount } from "enzyme";
import EmailChips from ".";

const baseProps = {
  placeholder: "Placeholder",
  clearButtonLabel: "Clear list ",
  existEmailText: "This email address has already been entered",
  invalidEmailText: "Invalid email",
  onChange: () => {},
};

describe("<InputWithChips />", () => {
  it("accepts id", () => {
    const wrapper = mount(<EmailChips {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<EmailChips {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <EmailChips {...baseProps} style={{ color: "red" }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
