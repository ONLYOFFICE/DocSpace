import React from "react";
import { mount, shallow } from "enzyme";
import ContextMenuButton from ".";

const baseData = () => [
  {
    key: "key",
    label: "label",
    onClick: () => jest.fn(),
  },
];

const baseProps = {
  title: "Actions",
  iconName: "static/images/vertical-dots.react.svg",
  size: 16,
  color: "#A3A9AE",
  getData: baseData,
  isDisabled: false,
};

describe("<ContextMenuButton />", () => {
  it("renders without error", () => {
    const wrapper = mount(<ContextMenuButton {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("render with full custom props", () => {
    const wrapper = mount(
      <ContextMenuButton
        color="red"
        hoverColor="red"
        clickColor="red"
        size={20}
        iconName="CatalogFolderIcon"
        iconHoverName="CatalogFolderIcon"
        iconClickName="CatalogFolderIcon"
        isFill={true}
        isDisabled={true}
        onClick={() => jest.fn()}
        onMouseEnter={() => jest.fn()}
        onMouseLeave={() => jest.fn()}
        onMouseOver={() => jest.fn()}
        onMouseOut={() => jest.fn()}
        getData={() => [
          {
            key: "key",
            icon: "CatalogFolderIcon",
            onClick: () => jest.fn(),
          },
          {
            label: "CatalogFolderIcon",
            onClick: () => jest.fn(),
          },
          {},
        ]}
        directionX="right"
        opened={true}
      />
    );

    expect(wrapper).toExist();
  });

  it("disabled", () => {
    const wrapper = mount(
      <ContextMenuButton {...baseProps} isDisabled={true} />
    );

    expect(wrapper.prop("isDisabled")).toEqual(true);
  });

  it("not re-render", () => {
    const wrapper = shallow(<ContextMenuButton {...baseProps} />).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate(
      wrapper.props,
      wrapper.state
    );

    expect(shouldUpdate).toBe(false);
  });

  it("re-render", () => {
    const wrapper = shallow(<ContextMenuButton {...baseProps} />).instance();

    const shouldUpdate = wrapper.shouldComponentUpdate(
      { opened: true },
      wrapper.state
    );

    expect(shouldUpdate).toBe(true);
  });

  it("causes function onDropDownItemClick()", () => {
    const onClick = jest.fn();

    const wrapper = shallow(
      <ContextMenuButton {...baseProps} opened={true} onClick={onClick} />
    );
    const instance = wrapper.instance();

    instance.onDropDownItemClick({
      key: "key",
      label: "label",
      onClick: onClick,
    });

    expect(wrapper.state("isOpen")).toBe(false);
    expect(onClick).toHaveBeenCalled();
  });

  it("causes function onIconButtonClick()", () => {
    const wrapper = shallow(
      <ContextMenuButton {...baseProps} isDisabled={false} opened={true} />
    );
    const instance = wrapper.instance();

    instance.onIconButtonClick();

    expect(wrapper.state("isOpen")).toBe(false);
  });

  it("causes function onIconButtonClick() with isDisabled prop", () => {
    const wrapper = shallow(
      <ContextMenuButton {...baseProps} isDisabled={true} opened={true} />
    );
    const instance = wrapper.instance();

    instance.onIconButtonClick();

    expect(wrapper.state("isOpen")).toBe(true);
  });

  it("componentDidUpdate() state lifecycle test", () => {
    const wrapper = shallow(<ContextMenuButton {...baseProps} />);
    const instance = wrapper.instance();

    wrapper.setState({ isOpen: false });

    instance.componentDidUpdate(wrapper.props(), wrapper.state());

    expect(wrapper.state()).toBe(wrapper.state());
  });

  it("componentDidUpdate() props lifecycle test", () => {
    const wrapper = shallow(<ContextMenuButton {...baseProps} />);
    const instance = wrapper.instance();

    instance.componentDidUpdate({ opened: true }, wrapper.state());

    expect(wrapper.props()).toBe(wrapper.props());
  });

  it("accepts id", () => {
    const wrapper = mount(<ContextMenuButton {...baseProps} id="testId" />);

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(
      <ContextMenuButton {...baseProps} className="test" />
    );

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <ContextMenuButton {...baseProps} style={{ color: "red" }} />
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });
});
