import React from "react";
import { mount } from "enzyme";
import RoomLogo from ".";

const baseProps = {
  type: "custom",
  isPrivacy: false,
  isArchive: false,
};

describe("<RoomLogo />", () => {
  it("renders without error", () => {
    const wrapper = mount(<RoomLogo {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("accepts id", () => {
    const wrapper = mount(<RoomLogo {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<RoomLogo {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(<RoomLogo {...baseProps} style={{ color: "red" }} />);

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });

  it("accepts isPrivacy prop", () => {
    const wrapper = mount(<RoomLogo {...baseProps} isPrivacy={true} />);

    expect(wrapper.prop("isPrivacy")).toEqual(true);
  });

  it("accepts isPrivacy prop", () => {
    const wrapper = mount(<RoomLogo {...baseProps} isArchive={true} />);

    expect(wrapper.prop("isArchive")).toEqual(true);
  });
});
