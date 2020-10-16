import React from "react";
import { mount, shallow } from "enzyme";
import GroupButtonsMenu from ".";
import DropDownItem from "../drop-down-item";

beforeEach(() => {
  const div = document.createElement("div");
  div.setAttribute("id", "container");
  document.body.appendChild(div);
});

afterEach(() => {
  const div = document.getElementById("container");
  if (div) {
    document.body.removeChild(div);
  }
});

const defaultMenuItems = [
  {
    label: "Select",
    isDropdown: true,
    isSeparator: true,
    isSelect: true,
    fontWeight: "bold",
    children: [
      <DropDownItem key="aaa" label="aaa" />,
      <DropDownItem key="bbb" label="bbb" />,
      <DropDownItem key="ccc" label="ccc" />,
    ],
    onSelect: () => {},
  },
  {
    label: "Menu item 1",
    disabled: false,
    onClick: () => {},
  },
  {
    label: "Menu item 2",
    disabled: true,
    onClick: () => {},
  },
];

const getMenuItems = (count = 10) =>
  new Array(count).fill({
    label: "test item",
    disabled: false,
    onClick: jest.fn(),
  });

const baseProps = {
  checked: false,
  menuItems: defaultMenuItems,
  visible: true,
  moreLabel: "More",
  closeTitle: "Close",
};

describe("<GroupButtonsMenu />", () => {
  it("renders without error", () => {
    const wrapper = mount(<GroupButtonsMenu {...baseProps} />);

    expect(wrapper).toExist();
  });

  it("applies checked prop", () => {
    const wrapper = mount(<GroupButtonsMenu {...baseProps} checked={true} />);

    expect(wrapper.prop("checked")).toEqual(true);
  });

  it("applies visible prop", () => {
    const wrapper = mount(<GroupButtonsMenu {...baseProps} visible={true} />);

    expect(wrapper.prop("visible")).toEqual(true);
  });

  it("causes function closeMenu()", () => {
    const onClose = jest.fn();
    const wrapper = mount(
      <GroupButtonsMenu {...baseProps} onClose={onClose} />
    );
    const instance = wrapper.instance();

    instance.closeMenu(new Event("click"));

    expect(wrapper.state("visible")).toBe(false);
    expect(onClose).toBeCalled();
  });

  it("causes function groupButtonClick()", () => {
    const onClick = jest.fn();
    const item = {
      label: "Menu item 1",
      disabled: false,
      onClick,
    };
    const wrapper = mount(
      <GroupButtonsMenu {...baseProps} menuItems={[item]} />
    );
    const instance = wrapper.instance();

    instance.groupButtonClick({
      currentTarget: {
        dataset: {
          index: 0,
        },
      },
    });

    expect(wrapper.state("visible")).toBe(true);
    expect(onClick).toBeCalled();
  });

  it("causes function countMenuItems()", () => {
    const wrapper = mount(<GroupButtonsMenu {...baseProps} />);
    const instance = wrapper.instance();

    instance.countMenuItems(null, 2, 1);
    instance.countMenuItems([], 2, 1);
    instance.countMenuItems([1, 2], 200, 2);
    instance.countMenuItems([1, 2, 3, 4, 5], 10, 100);
    instance.countMenuItems([1, 2, 3, 4], 1, 2);

    expect(wrapper.state("visible")).toBe(true);
  });

  it("causes function groupButtonClick() if disabled", () => {
    const onClick = jest.fn();
    const item = {
      label: "Menu item 1",
      disabled: true,
      onClick,
    };
    const wrapper = mount(
      <GroupButtonsMenu {...baseProps} menuItems={[item]} />
    );
    const instance = wrapper.instance();

    instance.groupButtonClick({
      currentTarget: {
        dataset: {
          index: 0,
        },
      },
    });

    expect(wrapper.state("visible")).toBe(true);
    expect(onClick).toBeCalledTimes(0);
  });

  it("componentDidUpdate() props lifecycle test", () => {
    const wrapper = shallow(
      <GroupButtonsMenu {...baseProps} visible={false} />
    );
    const instance = wrapper.instance();

    instance.componentDidUpdate(
      { visible: true, menuItems: getMenuItems(3) },
      wrapper.state()
    );

    expect(wrapper.props()).toBe(wrapper.props());
  });

  it("componentWillUnmount() props lifecycle test", () => {
    const wrapper = shallow(<GroupButtonsMenu {...baseProps} />);
    const instance = wrapper.instance();

    instance.componentWillUnmount();

    expect(wrapper).toExist(false);
  });

  it("filled state moreItems", () => {
    const wrapper = shallow(<GroupButtonsMenu {...baseProps} />);

    wrapper.setState({
      moreItems: [
        {
          label: "Menu item 1",
          disabled: false,
          onClick: jest.fn(),
        },
      ],
    });

    expect(wrapper).toExist(false);
  });

  it("render with 100% width", () => {
    const wrapper = mount(
      <div style={{ width: "100%" }}>
        <GroupButtonsMenu {...baseProps} />
      </div>,
      { attachTo: document.getElementById("container") }
    );

    const menuNodeStyle = wrapper.get(0).props.style;
    const menuNode = wrapper.find(GroupButtonsMenu).instance();

    const moreItemsCount = menuNode.state.moreItems.length;
    const priorityItemsCount = menuNode.state.priorityItems.length;

    expect(menuNodeStyle.width).toEqual("100%");
    expect(priorityItemsCount).toEqual(3);
    expect(moreItemsCount).toEqual(0);
  });

  it("render with 1024px width and 3 items", () => {
    const wrapper = mount(
      <div style={{ width: "1024px" }}>
        <GroupButtonsMenu {...baseProps} />
      </div>,
      { attachTo: document.getElementById("container") }
    );

    const menuNodeStyle = wrapper.get(0).props.style;
    const menuNode = wrapper.find(GroupButtonsMenu).instance();

    const moreItemsCount = menuNode.state.moreItems.length;
    const priorityItemsCount = menuNode.state.priorityItems.length;

    expect(menuNodeStyle.width).toEqual("1024px");
    expect(menuNode.props.menuItems.length).toEqual(3);
    expect(priorityItemsCount).toEqual(3);
    expect(moreItemsCount).toEqual(0);
  });

  it("render with 1024px width and 10 items", () => {
    const wrapper = mount(
      <div style={{ width: "1024px" }}>
        <GroupButtonsMenu {...baseProps} menuItems={getMenuItems(10)} />
      </div>,
      { attachTo: document.getElementById("container") }
    );

    const menuNodeStyle = wrapper.get(0).props.style;
    const menuNode = wrapper.find(GroupButtonsMenu).instance();

    const moreItemsCount = menuNode.state.moreItems.length;
    const priorityItemsCount = menuNode.state.priorityItems.length;

    expect(menuNodeStyle.width).toEqual("1024px");
    expect(menuNode.props.menuItems.length).toEqual(10);
    expect(priorityItemsCount).toEqual(10);
    expect(moreItemsCount).toEqual(0);
  });

  it("render with 1024px width and 100 items", () => {
    const wrapper = mount(
      <div style={{ width: "1024px" }}>
        <GroupButtonsMenu {...baseProps} menuItems={getMenuItems(100)} />
      </div>,
      { attachTo: document.getElementById("container") }
    );

    const menuNodeStyle = wrapper.get(0).props.style;
    const menuNode = wrapper.find(GroupButtonsMenu).instance();

    const moreItemsCount = menuNode.state.moreItems.length;
    const priorityItemsCount = menuNode.state.priorityItems.length;

    expect(menuNodeStyle.width).toEqual("1024px");
    expect(menuNode.props.menuItems.length).toEqual(100);
    expect(priorityItemsCount).toEqual(100);
    expect(moreItemsCount).toEqual(0);
  });

  it("render with 500px width and 3 items", () => {
    const wrapper = mount(
      <div style={{ width: "500px" }}>
        <GroupButtonsMenu {...baseProps} />
      </div>,
      { attachTo: document.getElementById("container") }
    );

    const menuNodeStyle = wrapper.get(0).props.style;
    const menuNode = wrapper.find(GroupButtonsMenu).instance();

    const moreItemsCount = menuNode.state.moreItems.length;
    const priorityItemsCount = menuNode.state.priorityItems.length;

    expect(menuNodeStyle.width).toEqual("500px");
    expect(menuNode.props.menuItems.length).toEqual(3);
    expect(priorityItemsCount).toEqual(3);
    expect(moreItemsCount).toEqual(0);
  });

  it("render with 500px width and 10 items", () => {
    const wrapper = mount(
      <div style={{ width: "500px" }}>
        <GroupButtonsMenu {...baseProps} menuItems={getMenuItems(10)} />
      </div>,
      { attachTo: document.getElementById("container") }
    );

    const menuNodeStyle = wrapper.get(0).props.style;
    const menuNode = wrapper.find(GroupButtonsMenu).instance();

    const moreItemsCount = menuNode.state.moreItems.length;
    const priorityItemsCount = menuNode.state.priorityItems.length;

    expect(menuNodeStyle.width).toEqual("500px");
    expect(menuNode.props.menuItems.length).toEqual(10);
    expect(priorityItemsCount).toEqual(10);
    expect(moreItemsCount).toEqual(0);
  });

  it("render with 500px width and 100 items", () => {
    const wrapper = mount(
      <div style={{ width: "500px" }}>
        <GroupButtonsMenu {...baseProps} menuItems={getMenuItems(100)} />
      </div>,
      { attachTo: document.getElementById("container") }
    );

    const menuNodeStyle = wrapper.get(0).props.style;
    const menuNode = wrapper.find(GroupButtonsMenu).instance();

    const moreItemsCount = menuNode.state.moreItems.length;
    const priorityItemsCount = menuNode.state.priorityItems.length;

    expect(menuNodeStyle.width).toEqual("500px");
    expect(menuNode.props.menuItems.length).toEqual(100);
    expect(priorityItemsCount).toEqual(100);
    expect(moreItemsCount).toEqual(0);
  });
});
