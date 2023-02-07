import React from "react";
import { mount } from "enzyme";
import "jest-styled-components";
import ViewSelector from ".";

const baseProps = {
  isDisabled: false,
  isFilter: false,
  onChangeView: jest.fn(),
  viewAs: "row",
  viewSettings: [
    {
      value: "row",
      icon: "",
    },
    {
      value: "tile",
      icon: "",
    },
    {
      value: "some",
      icon: "",
    },
  ],
};

describe("<ViewSelector />", () => {
  it("renders without error", () => {
    const wrapper = mount(<ViewSelector {...baseProps} />);
    expect(wrapper).toExist();
  });

  it("render with disabled", () => {
    const wrapper = mount(
      <ViewSelector onClick={jest.fn()} {...baseProps} isDisabled={true} />
    );
    expect(wrapper).toExist();
  });

  it("id, className, style is exist", () => {
    const wrapper = mount(
      <ViewSelector
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

  it("accepts isDisabled", () => {
    const wrapper = mount(<ViewSelector {...baseProps} isDisabled />);

    expect(wrapper.prop("isDisabled")).toEqual(true);
  });

  it("accepts isFilter", () => {
    const wrapper = mount(<ViewSelector {...baseProps} isFilter />);

    expect(wrapper.prop("isFilter")).toEqual(true);
  });

  it("accepts viewAs", () => {
    const wrapper = mount(<ViewSelector {...baseProps} viewAs="tile" />);
    expect(wrapper.prop("viewAs")).toEqual("tile");
  });
});
