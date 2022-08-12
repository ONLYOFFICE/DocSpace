import React from "react";
import { mount } from "enzyme";
import CodeInput from ".";

describe("<CodeInput />", () => {
  it("renders without error", () => {
    const wrapper = mount(<CodeInput onSubmit={jest.fn()} />);

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(<CodeInput onSubmit={jest.fn()} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<CodeInput onSubmit={jest.fn()} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <CodeInput onSubmit={jest.fn()} style={{ color: "red" }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
