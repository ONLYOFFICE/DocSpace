import React from "react";
import { mount, shallow, render } from "enzyme";
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
    expect(wrapper.state.isOpen).toEqual(false);

    wrapper.setState({ isOpen: true });
    wrapper.afterHide();
    expect(wrapper.state.isOpen).toEqual(false);
  });
});
