import React from "react";
import { mount } from "enzyme";
import Textarea from ".";

describe("<Textarea />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <Textarea placeholder="Add comment" onChange={jest.fn()} value="value" />
    );

    expect(wrapper).toExist();
  });

  it("accepts className", () => {
    const wrapper = mount(
      <Textarea
        placeholder="Add comment"
        onChange={jest.fn()}
        value="value"
        className="test"
      />
    );

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <Textarea
        placeholder="Add comment"
        onChange={jest.fn()}
        value="value"
        style={{ color: "red" }}
      />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
