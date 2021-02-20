import React from "react";
import { mount, shallow } from "enzyme";
import DropDownItem from ".";

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

describe("<DropDownItem />", () => {
  it("renders without error", () => {
    const wrapper = mount(<DropDownItem {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("check disabled props", () => {
    const wrapper = mount(<DropDownItem {...baseProps} disabled={true} />);

    expect(wrapper.prop("disabled")).toEqual(true);
  });

  it("check isSeparator props", () => {
    const wrapper = mount(<DropDownItem {...baseProps} isSeparator={true} />);

    expect(wrapper.prop("isSeparator")).toEqual(true);
  });

  it("check isHeader props", () => {
    const wrapper = mount(<DropDownItem {...baseProps} isHeader={true} />);

    expect(wrapper.prop("isHeader")).toEqual(true);
  });

  it("check noHover props", () => {
    const wrapper = mount(<DropDownItem {...baseProps} noHover={true} />);

    expect(wrapper.prop("noHover")).toEqual(true);
  });

  it("causes function onClick()", () => {
    const onClick = jest.fn();

    const wrapper = shallow(
      <DropDownItem id="test" {...baseProps} onClick={onClick} />
    );

    wrapper.find("#test").simulate("click");

    expect(onClick).toHaveBeenCalled();
  });

  it("render without child", () => {
    const wrapper = shallow(<DropDownItem>test</DropDownItem>);

    expect(wrapper.props.children).toEqual(undefined);
  });

  it("accepts id", () => {
    const wrapper = mount(<DropDownItem {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<DropDownItem {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <DropDownItem {...baseProps} style={{ color: "red" }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
