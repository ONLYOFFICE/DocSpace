import React from "react";
import { mount, shallow } from "enzyme";
import HelpButton from ".";

const tooltipContent = "You tooltip content";
describe("<HelpButton />", () => {
  it("HelpButton renders without error", () => {
    const wrapper = mount(<HelpButton tooltipContent={tooltipContent} />);
    expect(wrapper).toExist();
  });

  it("HelpButton componentWillUnmount  test", () => {
    const wrapper = mount(<HelpButton tooltipContent={tooltipContent} />);
    const componentWillUnmount = jest.spyOn(
      wrapper.instance(),
      "componentWillUnmount"
    );
    wrapper.unmount();
    expect(componentWillUnmount).toHaveBeenCalled();
  });

  it("HelpButton test afterHide function", () => {
    const wrapper = shallow(
      <HelpButton tooltipContent={tooltipContent} />
    ).instance();
    wrapper.afterHide();
    expect(wrapper.state.hideTooltip).toEqual(false);

    wrapper.setState({ hideTooltip: false });
    wrapper.afterHide();
    expect(wrapper.state.hideTooltip).toEqual(false);
  });

  it("accepts id", () => {
    const wrapper = mount(
      <HelpButton tooltipContent={tooltipContent} id="testId" />
    );

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(
      <HelpButton tooltipContent={tooltipContent} className="test" />
    );

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <HelpButton tooltipContent={tooltipContent} style={{ color: "red" }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
