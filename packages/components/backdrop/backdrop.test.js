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

  it("accepts className string", () => {
    const wrapper = mount(<Backdrop {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts className array", () => {
    const testArr = ["test", "backdrop-active not-selectable"];
    const wrapper = mount(<Backdrop {...baseProps} className={["test"]} />);

    expect(wrapper.prop("className")).toEqual(expect.arrayContaining(testArr));
  });

  it("accepts style", () => {
    const wrapper = mount(
      <Backdrop {...baseProps} style={{ color: "red" }} visible={true} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
