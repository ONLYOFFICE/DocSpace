import React from "react";
import { mount, shallow } from "enzyme";
import SocialButton from ".";

import ShareGoogleReactSvgUrl from "../../../public/images/share.google.react.svg?url";

describe("<SocialButton />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <SocialButton iconName={ShareGoogleReactSvgUrl} label={"Test Caption"} />
    );

    expect(wrapper).toExist();
  });

  it("not re-render test", () => {
    // const onClick= () => alert('SocialButton clicked');

    const wrapper = shallow(
      <SocialButton iconName={ShareGoogleReactSvgUrl} label={"Test Caption"} />
    ).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate(wrapper.props);

    expect(shouldUpdate).toBe(false);
  });

  it("disabled click test", () => {
    const testClick = jest.fn();

    const wrapper = mount(
      <SocialButton
        iconName={ShareGoogleReactSvgUrl}
        label={"Test Caption"}
        onClick={testClick}
        isDisabled={true}
      />
    );

    wrapper.simulate("click");

    expect(testClick).toHaveBeenCalledTimes(0);
  });

  it("click test", () => {
    const testClick = jest.fn();

    const wrapper = mount(
      <SocialButton
        iconName={ShareGoogleReactSvgUrl}
        label={"Test Caption"}
        onClick={testClick}
        isDisabled={false}
      />
    );

    wrapper.simulate("click");

    expect(testClick).toHaveBeenCalledTimes(1);
  });

  it("accepts id", () => {
    const wrapper = mount(
      <SocialButton iconName={ShareGoogleReactSvgUrl} id="testId" />
    );

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(
      <SocialButton iconName={ShareGoogleReactSvgUrl} className="test" />
    );

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <SocialButton iconName={ShareGoogleReactSvgUrl} style={{ color: "red" }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
