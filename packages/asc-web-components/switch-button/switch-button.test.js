import React from "react";
import { mount } from "enzyme";
import "jest-styled-components";
import SwitchButton from ".";

const baseProps = {
  checked: false,
  disabled: false,
  onChange: jest.fn(),
};

describe("<SwitchButton />", () => {
  it("renders without error", () => {
    const wrapper = mount(<SwitchButton {...baseProps} />);
    expect(wrapper).toExist();
  });

  it("render with disabled", () => {
    const wrapper = mount(
      <SwitchButton onClick={jest.fn()} disabled={true} checked={false} />
    );
    expect(wrapper).toExist();
  });

  it("render without checked", () => {
    const wrapper = mount(
      <SwitchButton onClick={jest.fn()} disabled={false} />
    );
    expect(wrapper).toExist();
  });

  it("id, className, style is exist", () => {
    const wrapper = mount(
      <SwitchButton
        {...baseProps}
        id="testId"
        className="test"
        style={{ color: "red" }}
      />
    );

    expect(wrapper.prop("id")).toEqual("testId");
    expect(wrapper.prop("className")).toEqual("test");
    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });

  it("accepts disabled", () => {
    const wrapper = mount(<SwitchButton {...baseProps} disabled />);

    expect(wrapper.prop("disabled")).toEqual(true);
  });

  it("accepts checked", () => {
    const wrapper = mount(<SwitchButton {...baseProps} checked />);
    expect(wrapper.prop("checked")).toEqual(true);

    const wrapper2 = mount(<SwitchButton {...baseProps} checked={false} />);
    expect(wrapper2.prop("checked")).toEqual(false);
  });

  it("accepts checked and disabled", () => {
    const wrapper = mount(<SwitchButton {...baseProps} checked disabled />);

    expect(wrapper.prop("checked")).toEqual(true);
    expect(wrapper.prop("disabled")).toEqual(true);
  });

  it("onChange() test", () => {
    const wrapper = mount(<SwitchButton {...baseProps} />);
    expect(wrapper.props().checked).toBe(false);

    const input = wrapper.find('input[type="checkbox"]');
    input.simulate("change");
    expect(baseProps.onChange).toHaveBeenCalled();
  });
});
