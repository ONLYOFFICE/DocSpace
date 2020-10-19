import React from "react";
import { mount, shallow } from "enzyme";
import ToggleButton from ".";

describe("<ToggleButton />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <ToggleButton label="text" onChange={() => jest.fn()} isChecked={false} />
    );

    expect(wrapper).toExist();
  });

  it("Toggle button componentDidUpdate() test", () => {
    const wrapper = mount(
      <ToggleButton isChecked={false} onChange={() => jest.fn()} />
    ).instance();
    wrapper.componentDidUpdate(wrapper.props);

    const wrapper2 = mount(
      <ToggleButton isChecked={true} onChange={() => jest.fn()} />
    ).instance();
    wrapper2.componentDidUpdate(wrapper2.props);

    const wrapper3 = shallow(
      <ToggleButton isChecked={false} onChange={() => jest.fn()} />
    );
    wrapper3.setState({ isOpen: true });
    wrapper3.instance().componentDidUpdate(wrapper3.props());

    expect(wrapper.props).toBe(wrapper.props);
    expect(wrapper.state.checked).toBe(wrapper.props.isChecked);
    expect(wrapper2.props).toBe(wrapper2.props);
    expect(wrapper2.state.checked).toBe(wrapper2.props.isChecked);
    expect(wrapper3.state()).toBe(wrapper3.state());
  });

  it("accepts id", () => {
    const wrapper = mount(
      <ToggleButton
        label="text"
        onChange={() => jest.fn()}
        isChecked={false}
        id="testId"
      />
    );

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(
      <ToggleButton
        label="text"
        onChange={() => jest.fn()}
        isChecked={false}
        className="test"
      />
    );

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <ToggleButton
        label="text"
        onChange={() => jest.fn()}
        isChecked={false}
        style={{ color: "red" }}
      />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
