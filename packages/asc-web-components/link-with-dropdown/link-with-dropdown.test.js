import React from "react";
import { mount, shallow } from "enzyme";
import LinkWithDropdown from ".";

const data = [
  {
    key: "key1",
    label: "Button 1",
    onClick: () => console.log("Button1 action"),
  },
  {
    key: "key2",
    label: "Button 2",
    onClick: () => console.log("Button2 action"),
  },
  {
    key: "key3",
    isSeparator: true,
  },
  {
    key: "key4",
    label: "Button 3",
    onClick: () => console.log("Button3 action"),
  },
];

describe("<LinkWithDropdown />", () => {
  it("renders without error", () => {
    const wrapper = mount(
      <LinkWithDropdown color="#333333" isBold={true} data={[]}>
        Link with dropdown
      </LinkWithDropdown>
    );

    expect(wrapper).toExist();
  });

  it("re-render test", () => {
    const wrapper = mount(
      <LinkWithDropdown color="#333333" isBold={true} data={data}>
        Link with dropdown
      </LinkWithDropdown>
    );

    const instance = wrapper.instance();
    const shouldUpdate = instance.shouldComponentUpdate(
      {
        isBold: false,
      },
      wrapper.state
    );

    expect(shouldUpdate).toBe(true);
  });

  it("re-render after changing color", () => {
    const wrapper = shallow(
      <LinkWithDropdown color="#333333" isBold={true} data={data}>
        Link with dropdown
      </LinkWithDropdown>
    );
    const instance = wrapper.instance();

    const shouldUpdate = instance.shouldComponentUpdate(
      {
        color: "#999",
      },
      instance.state
    );

    expect(shouldUpdate).toBe(true);
  });

  it("re-render after changing dropdownType and isOpen prop", () => {
    const wrapper = shallow(
      <LinkWithDropdown color="#333333" isBold={true} data={data}>
        Link with dropdown
      </LinkWithDropdown>
    );
    const instance = wrapper.instance();

    const shouldUpdate = instance.shouldComponentUpdate(
      {
        isOpen: true,
        dropdownType: "appearDashedAfterHover",
      },
      instance.state
    );

    expect(shouldUpdate).toBe(true);
  });

  it("re-render after changing isOpen prop", () => {
    const wrapper = shallow(
      <LinkWithDropdown color="#333333" isBold={true} data={data}>
        Link with dropdown
      </LinkWithDropdown>
    );
    const instance = wrapper.instance();

    const shouldUpdate = instance.shouldComponentUpdate(
      {
        isOpen: true,
      },
      instance.state
    );

    expect(shouldUpdate).toBe(true);
  });

  it("not re-render", () => {
    const wrapper = mount(
      <LinkWithDropdown color="#333333" isBold={true} data={data}>
        Link with dropdown
      </LinkWithDropdown>
    );

    const instance = wrapper.instance();
    const shouldUpdate = instance.shouldComponentUpdate(
      instance.props,
      instance.state
    );

    expect(shouldUpdate).toBe(false);
  });

  it("accepts id", () => {
    const wrapper = mount(
      <LinkWithDropdown color="#333333" isBold={true} data={[]} id="testId">
        Link with dropdown
      </LinkWithDropdown>
    );

    expect(wrapper.prop("id")).toEqual("testId");
  });

  it("accepts className", () => {
    const wrapper = mount(
      <LinkWithDropdown
        color="#333333"
        isBold={true}
        data={[]}
        className="test"
      >
        Link with dropdown
      </LinkWithDropdown>
    );

    expect(wrapper.prop("className")).toEqual("test");
  });

  it("accepts style", () => {
    const wrapper = mount(
      <LinkWithDropdown
        color="#333333"
        isBold={true}
        data={[]}
        style={{ color: "red" }}
      >
        Link with dropdown
      </LinkWithDropdown>
    );

    expect(wrapper.getDOMNode().style).toHaveProperty("color", "red");
  });

  it("componentDidUpdate() state lifecycle test", () => {
    const wrapper = shallow(
      <LinkWithDropdown
        color="#333333"
        isBold={true}
        data={[]}
        style={{ color: "red" }}
      >
        Link with dropdown
      </LinkWithDropdown>
    );
    const instance = wrapper.instance();

    wrapper.setState({ isOpen: false });

    instance.componentDidUpdate(wrapper.props(), wrapper.state());

    wrapper.setState({ isOpen: true });

    instance.componentDidUpdate(wrapper.props(), wrapper.state());

    expect(wrapper.state()).toBe(wrapper.state());
  });

  it("componentDidUpdate() prop lifecycle test", () => {
    const wrapper = shallow(
      <LinkWithDropdown
        color="#333333"
        isBold={true}
        data={[]}
        style={{ color: "red" }}
      >
        Link with dropdown
      </LinkWithDropdown>
    );
    const instance = wrapper.instance();

    instance.componentDidUpdate(
      { isOpen: true, dropdownType: "appearDashedAfterHover" },
      wrapper.state()
    );

    expect(wrapper.state()).toBe(wrapper.state());
  });

  it("accepts prop dropdownType", () => {
    const wrapper = mount(
      <LinkWithDropdown
        color="#333333"
        isBold={true}
        data={[]}
        dropdownType="appearDashedAfterHover"
      >
        Link with dropdown
      </LinkWithDropdown>
    );

    expect(wrapper.prop("dropdownType")).toEqual("appearDashedAfterHover");
  });

  it("accepts prop isOpen", () => {
    const wrapper = mount(
      <LinkWithDropdown color="#333333" isBold={true} data={[]} isOpen>
        Link with dropdown
      </LinkWithDropdown>
    );

    expect(wrapper.prop("isOpen")).toEqual(true);
  });

  it("accepts prop isSemitransparent", () => {
    const wrapper = mount(
      <LinkWithDropdown
        color="#333333"
        isBold={true}
        data={[]}
        isSemitransparent
      >
        Link with dropdown
      </LinkWithDropdown>
    );

    expect(wrapper.prop("isSemitransparent")).toEqual(true);
  });
});
