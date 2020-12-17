import React from "react";
import { mount } from "enzyme";
import Loader from ".";

const baseProps = {
  type: "base",
  color: "black",
  size: "18px",
  label: "Loading",
};

describe("<Loader />", () => {
  it("renders without error", () => {
    const wrapper = mount(<Loader {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("dual-ring type", () => {
    const wrapper = mount(<Loader {...baseProps} type="dual-ring" />);

    expect(wrapper.prop("type")).toEqual("dual-ring");
  });

  it("rombs type", () => {
    const wrapper = mount(<Loader {...baseProps} type="rombs" />);

    expect(wrapper.prop("type")).toEqual("rombs");
  });

  it("accepts id", () => {
    const wrapper = mount(<Loader {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<Loader {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(<Loader {...baseProps} style={{ color: "red" }} />);

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
