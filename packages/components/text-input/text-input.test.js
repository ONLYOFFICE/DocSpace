import React from "react";
import { mount, shallow } from "enzyme";
import TextInput from ".";

describe("<TextInput />", () => {
  it("renders without error", () => {
    const wrapper = mount(<TextInput value="text" onChange={jest.fn()} />);

    expect(wrapper).toExist();
  });

  it("not re-render test", () => {
    const onChange = jest.fn();

    const wrapper = shallow(
      <TextInput value="text" onChange={onChange} />
    ).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate(wrapper.props);

    expect(shouldUpdate).toBe(false);
  });

  it("re-render test by value", () => {
    const onChange = jest.fn();

    const wrapper = shallow(
      <TextInput value="text" onChange={onChange} />
    ).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate({
      ...wrapper.props,
      value: "another text",
    });

    expect(shouldUpdate).toBe(true);
  });

  it("accepts id", () => {
    const wrapper = mount(
      <TextInput value="text" onChange={jest.fn()} id="testId" />
    );

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(
      <TextInput value="text" onChange={jest.fn()} className="test" />
    );

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <TextInput value="text" onChange={jest.fn()} style={{ color: "red" }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
