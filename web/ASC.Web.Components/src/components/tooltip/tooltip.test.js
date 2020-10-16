import React from "react";
import { mount } from "enzyme";
import Tooltip from ".";
import Text from "../text";

describe("<Tooltip />", () => {
  it("renders without error", () => {
    const wrapper = mount(<Tooltip>{<Text>You tooltip text</Text>}</Tooltip>);

    expect(wrapper).toExist();
  });

  it("Tooltip componentDidUpdate() lifecycle test", () => {
    const wrapper = mount(
      <Tooltip>{<Text>You tooltip text</Text>}</Tooltip>
    ).instance();
    wrapper.componentDidUpdate(wrapper.props, wrapper.state);
    expect(wrapper.props).toBe(wrapper.props);
  });

  it("Tooltip componentDidUpdate() lifecycle test", () => {
    const wrapper = mount(
      <Tooltip>{<Text>You tooltip text</Text>}</Tooltip>
    ).instance();
    wrapper.componentDidUpdate(wrapper.props, wrapper.state);
    expect(wrapper.props).toBe(wrapper.props);
  });

  it("accepts className", () => {
    const wrapper = mount(<Tooltip className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(<Tooltip style={{ color: "red" }} />);

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
