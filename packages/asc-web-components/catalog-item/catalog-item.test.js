import React from "react";
import { mount } from "enzyme";
import CatalogItem from ".";

const baseProps = {
  icon: "/static/images/catalog.folder.react.svg",
  text: "Documents",
  showText: true,
  onClick: () => jest.fn(),
  showInitial: true,
  showBadge: true,
  isEndOfBlock: true,
  labelBadge: "2",
  iconBadge: "/static/images/catalog.trash.react.svg",
  onClickBadge: () => jest.fn(),
};

describe("<CatalogItem />", () => {
  it("renders without error", () => {
    const wrapper = mount(<CatalogItem {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("render without text", () => {
    const wrapper = mount(<CatalogItem {...baseProps} text="My profile" />);

    expect(wrapper.prop("text")).toEqual("My profile");
  });

  it("render without text", () => {
    const wrapper = mount(<CatalogItem {...baseProps} text="My profile" />);

    expect(wrapper.prop("text")).toEqual("My profile");
  });

  it("render how not end of block", () => {
    const wrapper = mount(<CatalogItem {...baseProps} isEndOfBlock={false} />);

    expect(wrapper.prop("isEndOfBlock")).toEqual(false);
  });

  it("render without badge", () => {
    const wrapper = mount(<CatalogItem {...baseProps} showBadge={false} />);

    expect(wrapper.prop("showBadge")).toEqual(false);
  });

  it("render without initial", () => {
    const wrapper = mount(<CatalogItem {...baseProps} showInitial={false} />);

    expect(wrapper.prop("showInitial")).toEqual(false);
  });

  it("render without icon badge", () => {
    const wrapper = mount(<CatalogItem {...baseProps} iconBadge="" />);

    expect(wrapper.prop("iconBadge")).toEqual("");
  });

  it("render without label badge and icon badge", () => {
    const wrapper = mount(
      <CatalogItem {...baseProps} iconBadge="" iconLabel="" />
    );

    expect(wrapper.prop("iconBadge")).toEqual("");
    expect(wrapper.prop("iconLabel")).toEqual("");
  });

  it("render without icon", () => {
    const wrapper = mount(<CatalogItem {...baseProps} icon="" />);

    expect(wrapper.prop("icon")).toEqual("");
  });

  it("accepts id", () => {
    const wrapper = mount(<CatalogItem {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(<CatalogItem {...baseProps} className="test" />);

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <CatalogItem {...baseProps} style={{ width: "100px" }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("width", "100px");
  });

  it("trigger click", () => {
    const wrapper = mount(
      <CatalogItem {...baseProps} style={{ width: "100px" }} />
    );

    expect(wrapper.simulate("click"));
  });

  it("trigger update", () => {
    const wrapper = mount(
      <CatalogItem {...baseProps} style={{ width: "100px" }} />
    );

    expect(wrapper.simulate("click"));
  });

  it("unmount without errors", () => {
    const wrapper = mount(
      <CatalogItem {...baseProps} style={{ width: "100px" }} />
    );

    wrapper.unmount();
  });
});
