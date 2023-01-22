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

  it("accepts fontSize", () => {
    const wrapper = mount(
      <Textarea
        placeholder="Add comment"
        onChange={jest.fn()}
        value="value"
        className="test"
        fontSize={12}
      />
    );

    expect(wrapper.prop("fontSize")).toEqual(12);
  });

  it("accepts heightTextArea", () => {
    const wrapper = mount(
      <Textarea
        placeholder="Add comment"
        onChange={jest.fn()}
        value="value"
        className="test"
        heightTextArea={54}
      />
    );

    expect(wrapper.prop("heightTextArea")).toEqual(54);
  });

  it("accepts copyInfoText", () => {
    const wrapper = mount(
      <Textarea
        placeholder="Add comment"
        onChange={jest.fn()}
        value="value"
        className="test"
        copyInfoText='text was copied'
      />
    );

    expect(wrapper.prop("copyInfoText")).toEqual('text was copied');
  });

  it("accepts isJSONField", () => {
    const wrapper = mount(
      <Textarea
        placeholder="Add comment"
        onChange={jest.fn()}
        value="value"
        className="test"
        isJSONField={true}
      />
    );

    expect(wrapper.prop("isJSONField")).toEqual(true);
  });
});
