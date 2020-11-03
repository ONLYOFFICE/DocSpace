import React from "react";
import { mount } from "enzyme";
import Backdrop from ".";

const baseProps = {
  visible: false,
};

describe("<Backdrop />", () => {
  it("renders without error", () => {
    const wrapper = mount(<Backdrop {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("visible", () => {
    const wrapper = mount(<Backdrop visible />);

    expect(wrapper.prop("visible")).toBe(true);
  });

  it("accepts id", () => {
    const wrapper = mount(<Backdrop {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<Backdrop {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(<Backdrop {...baseProps} style={{ color: "red" }} />);

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
