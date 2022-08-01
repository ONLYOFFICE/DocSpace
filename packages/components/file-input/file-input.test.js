import React from "react";
import { mount, shallow } from "enzyme";
import FileInput from ".";

describe("<FileInput />", () => {
  it("renders without error", () => {
    const wrapper = mount(<FileInput onInput={jest.fn()} />);

    expect(wrapper).toExist();
  });

  it("not re-render test", () => {
    const onInput = jest.fn();

    const wrapper = shallow(<FileInput onInput={onInput} />).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate(
      wrapper.props,
      wrapper.state
    );

    expect(shouldUpdate).toBe(false);
  });

  it("accepts id", () => {
    const wrapper = mount(<FileInput onInput={jest.fn()} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<FileInput onInput={jest.fn()} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <FileInput onInput={jest.fn()} style={{ color: "red" }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
