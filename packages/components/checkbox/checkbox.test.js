import React from "react";
import { mount, shallow } from "enzyme";
import Checkbox from ".";

const baseProps = {
  value: "test",
};

describe("<Checkbox />", () => {
  it("renders without error", () => {
    const wrapper = mount(<Checkbox {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(<Checkbox {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<Checkbox {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(<Checkbox {...baseProps} style={{ color: "red" }} />);

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });

  it("accepts isDisabled", () => {
    const wrapper = mount(<Checkbox {...baseProps} isDisabled />);

    expect(wrapper.prop("isDisabled")).toEqual(true);
  });

  it("accepts isIndeterminate", () => {
    const wrapper = mount(<Checkbox {...baseProps} isIndeterminate />);

    expect(wrapper.prop("isIndeterminate")).toEqual(true);
  });

  it("accepts isChecked", () => {
    const wrapper = mount(<Checkbox {...baseProps} isChecked />);

    expect(wrapper.prop("isChecked")).toEqual(true);
  });

  it("accepts isChecked and isDisabled", () => {
    const wrapper = mount(<Checkbox {...baseProps} isChecked isDisabled />);

    expect(wrapper.prop("isChecked")).toEqual(true);
    expect(wrapper.prop("isDisabled")).toEqual(true);
  });
});
