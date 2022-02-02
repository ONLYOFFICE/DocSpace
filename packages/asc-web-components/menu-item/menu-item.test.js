import React from "react";
import { mount, shallow } from "enzyme";
import MenuItem from ".";

const baseProps = {
  isSeparator: false,
  isHeader: false,
  tabIndex: -1,
  label: "test",
  disabled: false,
  icon: "static/images/nav.logo.react.svg",
  noHover: false,
  onClick: jest.fn(),
};

describe("<MenuItem />", () => {
  it("renders without error", () => {
    const wrapper = mount(<MenuItem {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("check isSeparator props", () => {
    const wrapper = mount(<MenuItem {...baseProps} isSeparator={true} />);

    expect(wrapper.prop("isSeparator")).toEqual(true);
  });

  it("check isHeader props", () => {
    const wrapper = mount(<MenuItem {...baseProps} isHeader={true} />);

    expect(wrapper.prop("isHeader")).toEqual(true);
  });

  it("check noHover props", () => {
    const wrapper = mount(<MenuItem {...baseProps} noHover={true} />);

    expect(wrapper.prop("noHover")).toEqual(true);
  });

  it("causes function onClick()", () => {
    const onClick = jest.fn();

    const wrapper = shallow(
      <MenuItem id="test" {...baseProps} onClick={onClick} />
    );

    wrapper.find("#test").simulate("click");

    expect(onClick).toHaveBeenCalled();
  });

  it("render without child", () => {
    const wrapper = shallow(<MenuItem>test</MenuItem>);

    expect(wrapper.props.children).toEqual(undefined);
  });

  it("accepts id", () => {
    const wrapper = mount(<MenuItem {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<MenuItem {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(<MenuItem {...baseProps} style={{ color: "red" }} />);

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
